"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl(`/AIConnect`).build();

var sendmsgbtn = document.getElementById("formUserQuery");

var formsentbtn = document.getElementById("sendmsgbtn");

var userInitial = document.getElementById("userinitial");

var userName = document.getElementById("UserName");

var aiName = document.getElementById("aiimgname");

var selecedAI = aiName.getAttribute("alt");

var btnIcon = document.getElementById("btnIcon");

var userDetail = document.getElementById("UserDetails").getAttribute('data-my-UserId');


sendmsgbtn.disabled = true;


connection.start().then(function () {
    sendmsgbtn.disabled = false;
    console.log('Connected to AIConnectHub')
}).catch(function (err) {
    return console.error(err.toString());

});

sendmsgbtn.addEventListener('submit', (e) => {

    e.preventDefault();

    var userqueryInput = document.getElementById("userquery");
    var userquery = userqueryInput.value;
    connection.invoke("SendMessage", userquery, selecedAI, userDetail).catch(function (err) {
        return console.error(err.toString());
    })
    
    userqueryInput.value = "";

    connection.invoke("ReplyFromServer").catch(function (err) {
        return console.error(err.toString());
    })

})

const addquerytodisplay = (message, userId) => {
    var newdiv = document.createElement('div');
    var userIconContainer = document.createElement('div');
    var userIcon = document.createElement('p');
    var userMessage = document.createElement('p');
    newdiv.className = 'textdisplay';
    userIcon.className = 'usericon';
    userIconContainer.className='userIconContainer'
    userMessage.className = 'messageviewsize'
    userMessage.textContent = message;
    if (userId === "img") {
        var img = document.createElement("img");
        img.src = aiName.src;
        img.alt = aiName.alt;
        img.className ="aiimageicon"
        userIcon.append(img);
        userIcon.style.backgroundColor = "none";
    }
    else {
        userIcon.textContent = userId;
        userIcon.style.backgroundColor = "black";
    }
    userIconContainer.appendChild(userIcon);
    newdiv.appendChild(userIconContainer);
    newdiv.appendChild(userMessage);
    var mainchatdisplaycontainer = document.getElementById("displaychatarea");
    mainchatdisplaycontainer.appendChild(newdiv);
}

connection.on("ReceiveMessage", function (message) {
    formsentbtn.disabled = true;
    addquerytodisplay(message, userInitial.textContent);
});

connection.on("ReceiveMessageFromServer", function (replyMessage) {
    formsentbtn.disabled = false;
    console.log("server reply");
    addquerytodisplay(replyMessage, "img");
})


connection.on("ReceiveerrorMessage", function (replyErrorMessage) {
    toastr.error(replyErrorMessage);
})