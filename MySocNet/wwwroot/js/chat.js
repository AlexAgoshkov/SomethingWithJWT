function getCook(cookiename) {
// Get name followed by anything except a semicolon
var cookiestring=RegExp(cookiename+"=[^;]+").exec(document.cookie);
// Return everything after the equal sign, or an empty string if the cookie name not found
return decodeURIComponent(!!cookiestring ? cookiestring.toString().replace(/^[^=]+./,"") : "");
}
//Sample usage
var cookieValue = getCook('key');
console.log(cookieValue);

"use strict";
let token = cookieValue;
let connection = new signalR.HubConnectionBuilder().withUrl("/chatHub", { accessTokenFactory: () => token}).build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (chat, user, message) {
    let cht = chat;
    let msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    let encodedMsg = user + " says " + msg;
    let li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    let name = document.getElementById("nameInput").innerHTML;
    let message = document.getElementById("messageInput").value;
    let chat = document.getElementById("chatId").innerHTML;
    let chatId = Number(chat);
    connection.invoke("SendMessage", chatId, name, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
