"use strict";
"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl(`/AIConnect`).build();

var sendmsgbtn = document.getElementById("formUserQuery");

var formsentbtn = document.getElementById("sendmsgbtn");

var userInitial = document.getElementById("userinitial");

var userName = document.getElementById("UserName");

var aiName1 = document.getElementById("aiimgname1");

var selecedAI1 = aiName1.getAttribute("alt");

var aiName2 = document.getElementById("aiimgname2");

var selecedAI2 = aiName2.getAttribute("alt");

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
    connection.invoke("SendMessagetocompareAI", userquery, selecedAI1, selecedAI2, userDetail).catch(function (err) {
        return console.error(err.toString());
    })

    userqueryInput.value = "";

    connection.invoke("ReplyFromServer").catch(function (err) {
        return console.error(err.toString());
    })

})

const addquerytodisplay = (message, userId, aiValue) => {
    var newdiv = document.createElement('div');
    var userIconContainer = document.createElement('div');
    var userIcon = document.createElement('p');
    var userIcontextspan = document.createElement('span');
    var userMessage = document.createElement('span');
    newdiv.className = 'textdisplay';
    userIcon.className = 'usericon';
    userIconContainer.className = 'userIconContainer'
    userMessage.className = 'messageviewsize'
    userIcontextspan.className = 'initialLetter'
    userIcon.appendChild(userIcontextspan);
    if (userId === "img") {
        userMessage.innerHTML = message;
        var img = document.createElement("img");
        if (aiValue == "OpenAi") {
            img.src = aiName1.src;
            img.alt = aiName1.alt;
        } else if (aiValue == "Gemini") {
            img.src = aiName2.src;
            img.alt = aiName2.alt;
        }
        
        img.className = "aiimageicon"
        userIcon.append(img);
        userIcon.style.backgroundColor = "none";
    }
    else {
        userMessage.textContent = message;
        userIcontextspan.textContent = userId;
        userIcon.style.backgroundColor = "#F2F4F4";
    }
    userIconContainer.appendChild(userIcon);
    newdiv.appendChild(userIconContainer);
    newdiv.appendChild(userMessage);
    var mainchatdisplaycontainer1 = document.getElementById("OpenAi");
    var mainchatdisplaycontainer2 = document.getElementById("Gemini");

    if (aiValue == "OpenAi") {
        mainchatdisplaycontainer1.appendChild(newdiv);
    } else if (aiValue == "Gemini") {
        mainchatdisplaycontainer2.appendChild(newdiv);
    }
    
}

connection.on("ReceiveMessage", function (message, AIName1, AIName2) {
    formsentbtn.disabled = true;
    addquerytodisplay(message, userInitial.textContent, AIName1);
    addquerytodisplay(message, userInitial.textContent, AIName2);

});

connection.on("ReceiveMessageFromServer", function (replyMessage, aiName) {   
    formsentbtn.disabled = false;    
    console.log(replyMessage);
    addquerytodisplay(replyMessage, "img", aiName);
})

connection.on("ReceiveerrorMessage", function (replyErrorMessage) {
    toastr.error(replyErrorMessage);
})
