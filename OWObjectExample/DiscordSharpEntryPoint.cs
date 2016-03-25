/**

                                DiscordSharp -> Overwolf Bindings

        * DiscordSharp created by Mike Santiago, released under the MIT Public License
            * https://github.com/Luigifan/DiscordSharp
        * The following bindings were made by Mike Santiago for Overwolf. This header will define the coding style.

        Terminology
            * Javascript object - the anonymous object created in C# (new {property = value})

        C# objects being bound to Overwolf's Javascript API (due to a limitation) must only be single level. Objects cannot be nested.

        You should not use just the C# object's property name inside of the Javascript object. Use the name of the object (minus Discord)
            and then its property name to define the property.
                Example: DiscordMessage.Content -> MessageContent

        All events should be redefined as generic Action<object> events.

        You can either setup getters for these events (as I've done) or you could (though, I have not tested) pass in a Javascript
            object to the event.

        Getters should take in an Action<object> for use as a callback. This is equivalent to a Javascript function callback. Due to
            another limitation in Overwolf's C# API, this is how it must be done. Construct a Javascript object that you'd want to
            return and pass it via the callback's Invoke() function.

*/

using System;
using DiscordSharp;
using DiscordSharp.Objects;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordSharp.Overwolf
{
#pragma warning disable CS0649 //object not assigned to has null value by default
    internal struct VoiceConnectionInformation
    {
        //structs are public by default in C++ >.>
        public DiscordServer server;
        public DiscordChannel channel;
    }
#pragma warning restore CS0649

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
        #region Logging message event
        public event Action<object> TextClientLogAdded;
        public LogMessage LastLogMessage;
        public void GetLastLogMessage(Action<object> callback)
        {
            if (LastLogMessage.Message.Contains("No potential server"))
                return;
            object callbackObject = new
            {
                Message = LastLogMessage.Message,
                Timestamp = LastLogMessage.TimeStamp,
                Level = LastLogMessage.Level
            };
            callback.Invoke(callbackObject);
        }
        #endregion
        #region Voice log message event
        public event Action<object> VoiceClientLogAdded;
        public LogMessage LastVoiceLogMessage;
        public void GetLastVoiceLogMessage(Action<object> callback)
        {
            object callbackObject = new
            {
                Message = LastVoiceLogMessage.Message,
                Timestamp = LastVoiceLogMessage.TimeStamp,
                Level = LastVoiceLogMessage.Level.ToString()
            };
            callback.Invoke(callbackObject);
        }
        #endregion

        #endregion

        #region Getters
        //private const string ObjectDelimiter = "Discord";
        ////Sorry, I just got used to Objective C wordy styled calls :D
        ////borked
        //private static object ConvertDiscordObjectToJavascriptObject(object discordObject, Type T)
        //{
        //    //Raw name of the object in code
        //    //DiscordMessage
        //    string nameofObject = T.ToString();
        //    string subName = nameofObject.Substring(nameofObject.LastIndexOf(ObjectDelimiter) + ObjectDelimiter.Length); //what the object actually is. Message

        //    PropertyInfo[] properties = T.GetProperties(); //get instance only public properties
        //    object returnValue = new object();
        //    Dictionary<string, object> returnValueAsDict = (Dictionary<string, object>)returnValue.ToDictionary();
        //    foreach (var info in properties)
        //    {
        //        string anonPropName = subName + info.Name;
        //        Console.WriteLine("CLR: Binding property " + info.Name + " as " + subName + info.Name);
        //        //returnValueAsDict.AddProperty(anonPropName, info.GetValue(discordObject, null));
        //        returnValueAsDict.Add(anonPropName, info.GetValue(discordObject, null));
        //    }
        //    returnValue = returnValueAsDict;
        //    return (object)returnValueAsDict;
        //}

        #region Server Getters
        /// <summary>
        /// Retrieves a server by name.
        /// </summary>
        /// <param name="callback">The Javascript function callback.</param>
        /// <param name="name">The name of the server.</param>
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

        /// <summary>
        /// Retrieves a server by ID.
        /// </summary>
        /// <param name="callback">The Javascript function callback.</param>
        /// <param name="id">The ID of the server.</param>
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
        #endregion

        #region Channel getters
        /// <summary>
        /// Retrieves a channel by its name in the given serverID.
        /// </summary>
        /// <param name="callback">The Javascript function callback.</param>
        /// <param name="serverID">The ID of the server to look in.</param>
        /// <param name="name">The name of the channel.</param>
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

        /// <summary>
        /// Retrieves a channel by its name in the given serverID.
        /// </summary>
        /// <param name="callback">The Javascript function callback.</param>
        /// <param name="serverID">The ID of the server to look in.</param>
        /// <param name="name">The name of the channel.</param>
        /// <param name="voice">If true, only return voice channels.</param>
        public void GetChannelByName(Action<object> callback, string serverID, string name, bool voice)
        {
            DiscordServer server = client.GetServersList().Find(x => x.id == serverID);
            if (server != null)
            {
                DiscordChannel channel = null;
                if(voice)
                    server.channels.Find(x => x.Name.ToLower() == name.ToLower().Trim('#') && x.Type == ChannelType.Voice);
                else
                    server.channels.Find(x => x.Name.ToLower() == name.ToLower().Trim('#'));
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

        /// <summary>
        /// Retrieves a channel by its ID.
        /// </summary>
        /// <param name="callback">The Javascript function callback.</param>
        /// <param name="serverID">The ID the channel belongs in.</param>
        /// <param name="id">The ID of the channel.</param>
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

        #region Member Getters

        public void GetMemberByID(Action<object> callback, string serverID, string memberID)
        {
            DiscordServer server = client.GetServersList().Find(x => x.id == serverID);
            if(server != null)
            {
                DiscordMember member = server.members.Find(x => x.ID == memberID);
                if(member != null)
                {
                    object memberCallback = new
                    {
                        MemberUsername = member.Username,
                        MemberAvatarURL = member.GetAvatarURL().ToString(),
                        MemberID = member.ID,
                        MemberDiscrim = member.Discriminator,
                        MemberStatus = member.Status,
                        MemberCurrentGame = member.CurrentGame
                    };
                    callback.Invoke(memberCallback);
                }
                else
                {
                    object noMemberCallback = new { Message = $"Couldn't find member with ID {memberID} in server {serverID}!" };
                    callback.Invoke(noMemberCallback);
                }
            }
            else
            {
                object noServerCallback = new { Message = "Couldn't find server given ID " + serverID };
                callback.Invoke(noServerCallback);
            }
        }

        #endregion


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
                            OnConnect(null);
                    };
                    client.SocketClosed += (sender, e) =>
                    {
                        ClosedFormattedReason = $"Closed! Code: {e.Code}. Reason: {e.Reason}";
                        IsFullyConnected = false;
                        if (OnDisconnect != null)
                            OnDisconnect(null);
                    };
                    client.MessageReceived += (sender, e) =>
                    {
                        LastMessageReceived = e.message;
                        if (MessageReceived != null)
                            MessageReceived(null);
                    };
                    client.TextClientDebugMessageReceived += (sender, e) =>
                    {
                        LastLogMessage = e.message;
                        if (TextClientLogAdded != null)
                            TextClientLogAdded(null);
                    };
                    client.VoiceClientDebugMessageReceived += (sender, e) =>
                    {
                        LastVoiceLogMessage = e.message;
                        if (VoiceClientLogAdded != null)
                            VoiceClientLogAdded(null);
                    };

                    if (client.SendLoginRequest() != null)
                    {
                        Task.Factory.StartNew(()=>client.Connect());
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

        #region Voice Related
        public void BeginVoiceConnect(Action<object> callback, string serverID, string channelID)
        {
            DiscordServer server = client.GetServersList().Find(x => x.id == serverID);
            if(server != null)
            {
                DiscordChannel channel = server.channels.Find(x => x.ID == channelID);
                if (channel != null)
                {
                    if (channel.Type != ChannelType.Voice)
                    {
                        callback.Invoke(new { Message = "Not a voice channel!" });
                        return;
                    }

                    client.ConnectToVoiceChannel(channel);
                    client.VoiceClientConnected += (sender, e) =>
                    {
                        callback.Invoke(new { Message = "Connected!" });
                    };
                }
                else
                    callback.Invoke(new { Message = "Channel was null! ID: " + channelID });
            }
            else
                callback.Invoke(new { Message = "Server was null! ID: " + serverID });
        }
        #endregion

        #region Misc API Features

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

        public void SendMessageToChannel(string message, DiscordChannel channel)
        {
            client.SendMessageToChannel(message, channel);
        }

        #endregion
    }
}
