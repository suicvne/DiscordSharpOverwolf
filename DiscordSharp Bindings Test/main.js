var _discordSharpObject;

function onMyEvent(eventData)
{
	console.log("Received my event: " + eventData);
}

function onLog(eventData)
{
    _discordSharpObject.GetLastLogMessage(function callback(value) {
        if(value != null)
        {
            //Debug, Unecessary, Critical, Error
            if (value.Level == "Critical" || value.Level == "Error") {
                console.log(value.Level + ": " + value.Message);
                window.alert("An error has occurred in DiscordSharp!\n\n" + value.Message);
            }
        }
    });
}

function onVoiceLog(eventData)
{
    _discordSharpObject.GetLastVoiceLogMessage(function callback(value) {
        if(value != null)
        {
            console.log("[Voice Client " + value.Level + "] " + value.Message);
        }
    });
}

function onConnect(eventData)
{
    _discordSharpObject.GetMeUsername(function callback(value)
    {
        if (value != null)
        {
            console.log("Connected as user: " + value);
            document.getElementById("kek").innerText = "Connected as " + value;
        }
    });

    _discordSharpObject.SetCurrentGame("Overwolf Binding");
    _discordSharpObject.MessageReceived.addListener(onMessage);
    _discordSharpObject.OnDisconnect.addListener(onClose);
}

function onClose()
{
    console.log("closed!");
    _discordSharpObject.GetCloseReason(function callback(value)
    {
        document.getElementById("kek").innerText = value;
    });
}

function onMessage()
{
    _discordSharpObject.GetLastMessageReceived(function callback(value)
    {
        var msg = "--Message received from " + value.AuthorName + " in #" + value.ChannelName + " on " + value.ServerName + ": " + value.Content;
        console.log(msg);
        document.getElementById("messageLog").innerHTML += "<li>" + msg + "</li><br/>";
    });
}

function onException()
{
    console.log("Exception!!!!");
    _discordSharpObject.GetExceptionOccurred(function callback(value)
    {
        document.getElementById("kek").innerText = "Exception occurred! Check log!";
        console.log("Exception: " + value.Message);
    });
}

function testVoice(form)
{
    var serverID, channelID;
    _discordSharpObject.GetServerByName(function callback(value) {
        console.log("voice; getserver: " + value + "; id: " + value.ServerID);
        if (value != null)
            serverID = value.ServerID;
    }, "DiscordSharp Test Server");
    _discordSharpObject.GetChannelByID(function callback(value) {
        console.log("voice; getchannel: " + value);
        if (value != null)
            channelID = value.ChannelID;
    }, serverID, "145308133512708096"); //true for voice channels only

    
        _discordSharpObject.BeginVoiceConnect(function callback(value)
        {
            if(value != null)
            {
                if (value.Message == "Connected!") {
                    console.log("Connected to voice!");
                }
                else
                    console.log(value.Message);
            }
        }, serverID, channelID);
}

function doLogin(form)
{
	var email, password;
	email = form.emailText.value;
	password = form.passwordText.value;
	console.log("Pre login");
	if(_discordSharpObject != null)
	{
		console.log("Logging in");
		_discordSharpObject.Login(email, password);
		window.localStorage.setItem("discordUsername", email);
		window.localStorage.setItem("discordPassword", password);
	}
	else {
		console.log("DiscordSharp object is null?!?!?!?!");
	}
}

function onLoad(form)
{
    form.emailText.value = window.localStorage.getItem("discordUsername");
    form.passwordTe.value = window.localStorage.getItem("discordPassword");
}

function clearMessageInputForm(form, succeeded)
{
    form.messageText.value = "";
}
function sendMessage(form)
{
    //getters
    var serverName, channelName, messageToSend;
    serverName = form.serverName.value;
    channelName = form.channelName.value;
    messageToSend = form.messageText.value;
    //

    //CLR
    var serverID, channelID;
    _discordSharpObject.GetServerByName(function callback(value)
    {
        if (value != null) {
            serverID = value.ServerID;
        }
        else {
            clearMessageInputForm(form, 0);
            return;
        }
    }, serverName);

    _discordSharpObject.GetChannelByName(function callback(value)
    {
        if (value != null) {
            channelID = value.ChannelID;
        }
        else {
            clearMessageInputForm(form, 0);
            return;
        }
    }, serverID, channelName);
    //

    //Let's send
    _discordSharpObject.SendMessage(function callback(value) {
        if(value != undefined) //message succeed
        {
            clearMessageInputForm(form, 1);
        }
        else
        {
            console.error("Failed to send message");
        }
    }, serverID, channelID, messageToSend);
    //
}

function doLogout(form)
{
    _discordSharpObject.Logout();
    console.log("Logged out :)");
}

overwolf.extensions.current.getExtraObject("discordSharpEntryObject", function(result)
{
	if(result.status == "success")
	{
		var discordSharpEntryObject = result.object; //this is our class we did in C#
		discordSharpEntryObject.OnConnect.addListener(onConnect);
		discordSharpEntryObject.GetUsername(function callback(value)
		{});
		discordSharpEntryObject.ExceptionOccurred.addListener(onException);
		discordSharpEntryObject.TextClientLogAdded.addListener(onLog);
		discordSharpEntryObject.VoiceClientLogAdded.addListener(onVoiceLog);
		_discordSharpObject = discordSharpEntryObject;
	}
	else {
		console.log("Finding EntryPoint object failed!!!!");
	}
});
/*
                overwolf.extensions.current.getExtraObject("extraObject1", function(result) {
                    if (result.status == "success") {
                        var extraObject1 = result.object;
                        extraObject1.MyEvent.addListener(onMyEvent);
			extraObject1.InvokeMyEvent("Hello world!");
                    }
                });
								*/
