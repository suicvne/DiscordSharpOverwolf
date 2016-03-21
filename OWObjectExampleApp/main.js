var _discordSharpObject;

function onMyEvent(eventData)
{
	console.log("Received my event: " + eventData);
}

function onConnect(eventData)
{
    console.log("ayyyyyyyy we good");
    //console.log("Connected as user: " + _discordSharpObject.GetMeUsername());

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
    //console.log("Message received!");
    _discordSharpObject.GetLastMessageReceived(function callback(value)
    {
        var msg = "--Message received from " + value.AuthorName + " in #" + value.ChannelName + " on " + value.ServerName + ": " + value.Content;
        //var msg = "--Message received: " + value.Content;
        console.log(msg);
        document.getElementById("messageLog").innerHTML += "<li>" + msg + "</li><br/>";
        //window.scrollTo(0, document.body.scrollHeight);
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
	}
	else {
		console.log("DiscordSharp object is null?!?!?!?!");
	}
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
