using System;
using DiscordSharp;
using DiscordSharp.Objects;
using System.Reflection;
using System.Collections.Generic;

namespace OWObjectExample
{
    public class DiscordSharpEntryPoint
    {
        private DiscordClient client;

        public void GetUsername(Action<object> action)
        {
            action.Invoke("Test Username");
        }

        #region Events
        #region Full connected
        private bool IsFullyConnected = false;
        public bool GetIsFullyConnected() { return IsFullyConnected; }
        #endregion
        #region Message Received
        public event Action<object> MessageReceived;
        private DiscordMessage LastMessageReceived;
        public void GetLastMessageReceived(Action<object> callback)
        {
            object callbackObject = new
            {
                Content = LastMessageReceived.content,
                AuthorID = LastMessageReceived.author.ID,
                AuthorName = LastMessageReceived.author.Username,
                AuthorAvatar = LastMessageReceived.author.Avatar,
                ChannelID = LastMessageReceived.Channel().ID,
                ChannelName = LastMessageReceived.Channel().Name,
                ServerID = LastMessageReceived.Channel().parent.id,
                ServerName = LastMessageReceived.Channel().parent.name
            };
            callback.Invoke(callbackObject);
        }
        #endregion
        #region Connect event
        public event Action<object> OnConnect;
        public void GetMeUsername(Action<object> callback)
        {
            if (client != null && IsFullyConnected)
            {
                callback.Invoke(client.Me.Username);
            }

            callback.Invoke(null);
        }
        #endregion
        #region Disconnect event
        public event Action<object> OnDisconnect;
        private string ClosedFormattedReason;
        public void GetCloseReason(Action<object> callback)
        {
            callback.Invoke(ClosedFormattedReason);
        }
        #endregion
        #region An exception occurred event
        public event Action<object> ExceptionOccurred;
        public Exception LastException;
        public void GetExceptionOccurred(Action<object> callback)
        {
            object exceptionCallbackObject = new
            {
                Message = LastException.Message,
                StackTrace = LastException.StackTrace
            };
            callback.Invoke(exceptionCallbackObject);
        }
        #endregion
        #endregion

        #region Getters
        private const string ObjectDelimiter = "Discord";
        //Sorry, I just got used to Objective C wordy styled calls :D
        //borked
        private static object ConvertDiscordObjectToJavascriptObject(object discordObject, Type T)
        {
            //Raw name of the object in code
            //DiscordMessage
            string nameofObject = T.ToString();
            string subName = nameofObject.Substring(nameofObject.LastIndexOf(ObjectDelimiter) + ObjectDelimiter.Length); //what the object actually is. Message

            PropertyInfo[] properties = T.GetProperties(); //get instance only public properties
            object returnValue = new object();
            Dictionary<string, object> returnValueAsDict = (Dictionary<string, object>)returnValue.ToDictionary();
            foreach (var info in properties)
            {
                string anonPropName = subName + info.Name;
                Console.WriteLine("CLR: Binding property " + info.Name + " as " + subName + info.Name);
                //returnValueAsDict.AddProperty(anonPropName, info.GetValue(discordObject, null));
                returnValueAsDict.Add(anonPropName, info.GetValue(discordObject, null));
            }
            returnValue = returnValueAsDict;
            return (object)returnValueAsDict;
        }
        public void GetServerByName(Action<object> callback, string name)
        {
            DiscordServer server = client.GetServersList().Find(x => x.name.ToLower() == name.ToLower());
            if(server != null)
            {
                object serverCallbackObject = new
                {
                    ServerName = server.name,
                    ServerID = server.id,
                    ServerIcon = server.IconURL
                };
                callback.Invoke(serverCallbackObject);
            }
        }
        public void GetServerByID(Action<object> callback, string id)
        {
            DiscordServer server = client.GetServersList().Find(x => x.id == id);
            if (server != null)
            {
                object serverCallbackObject = new
                {
                    ServerName = server.name,
                    ServerID = server.id,
                    ServerIcon = server.IconURL
                };
                callback.Invoke(serverCallbackObject);
            }
        }

        public void GetChannelByName(Action<object> callback, string serverID, string name)
        {
            DiscordServer server = client.GetServersList().Find(x => x.id == serverID);
            if(server != null)
            {
                DiscordChannel channel = server.channels.Find(x => x.Name.ToLower() == name.ToLower().Trim('#'));
                if(channel != null)
                {
                    object channelCallbackObject = new
                    {
                        ChannelName = channel.Name,
                        ChannelID = channel.ID,
                        ChannelTopic = channel.Topic
                    };
                    callback.Invoke(channelCallbackObject);
                }
            }
        }
        public void GetChannelByID(Action<object> callback, string serverID, string id)
        {
            DiscordServer server = client.GetServersList().Find(x => x.id == serverID);
            if (server != null)
            {
                DiscordChannel channel = server.channels.Find(x => x.ID == id);
                if (channel != null)
                {
                    object channelCallbackObject = new
                    {
                        ChannelName = channel.Name,
                        ChannelID = channel.ID,
                        ChannelTopic = channel.Topic
                    };
                    callback.Invoke(channelCallbackObject);
                }
            }
        }
        #endregion

        #region Send Messages

        public void SendMessage(Action<object> callback, string serverID, string channelID, string message)
        {
            DiscordServer server = client.GetServersList().Find(x => x.id == serverID);
            if(server != null)
            {
                DiscordChannel channel = server.channels.Find(x => x.ID == channelID);
                if(channel != null)
                {
                    DiscordMessage messageSent = channel.SendMessage(message);
                    object messageCallback = new
                    {
                        MessageID = messageSent.id,
                        MessageContent = messageSent.content,
                        MessageChannelID = messageSent.Channel().ID,
                        MessageServerID = messageSent.Channel().parent.id,
                        MessageAuthorID = messageSent.author.ID
                    };
                    callback.Invoke(messageCallback);
                }
            }
            callback.Invoke(null);
        }

        #endregion

        public DiscordSharpEntryPoint() { } //explicit ctor

        public void Login(string email, string password)
        {
            if (!IsFullyConnected)
            {
                try
                {
                    client = new DiscordClient();
                    client.RequestAllUsersOnStartup = true;
                    client.ClientPrivateInformation.email = email;
                    client.ClientPrivateInformation.password = password;
                    client.Connected += (sender, e) =>
                    {
                        IsFullyConnected = true;
                        if (OnConnect != null)
                            OnConnect(new object());
                    };
                    client.SocketClosed += (sender, e) =>
                    {
                        ClosedFormattedReason = $"Closed! Code: {e.Code}. Reason: {e.Reason}";
                        IsFullyConnected = false;
                        if (OnDisconnect != null)
                            OnDisconnect(new object());
                    };
                    client.MessageReceived += (sender, e) =>
                    {
                        LastMessageReceived = e.message;
                        if (MessageReceived != null)
                            MessageReceived(new object());
                    };

                    if (client.SendLoginRequest() != null)
                    {
                        client.Connect();
                    }
                }
                catch(Exception ex)
                {
                    LastException = ex;
                    if (ExceptionOccurred != null)
                        ExceptionOccurred(new object());
                }
            }
        }

        public void Logout()
        {
            if(IsFullyConnected)
            {
                client.Logout();
                client.Dispose();
                IsFullyConnected = false;
            }
        }

        public void SetCurrentGame(string game)
        {
            if(IsFullyConnected)
            {
                client.UpdateCurrentGame(game);
            }
        }

        public DiscordServer GetDiscordServerByName(string name)
        {
            foreach(var serever in client.GetServersList())
            {
                if(serever.name.ToLower() == name.ToLower())
                {
                    return serever;
                }
            }
            return null;
        }

        public DiscordChannel GetTextChannelByName(DiscordServer server, string name)
        {
            foreach (var channel in server.channels)
            {
                if (channel.Type == ChannelType.Text)
                {
                    if (channel.Name.ToLower() == name.ToLower())
                    {
                        return channel;
                    }
                }
            }
            return null;
        }

        public void SendMessageToChannel(string message, DiscordChannel channel)
        {
            client.SendMessageToChannel(message, channel);
        }
    }
}
