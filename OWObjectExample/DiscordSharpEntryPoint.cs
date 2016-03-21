using System;
using DiscordSharp;
using DiscordSharp.Objects;

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
