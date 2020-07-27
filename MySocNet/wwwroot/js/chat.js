"use strict";

let connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (chatId, user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg + " " + chatId;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var token = document.getElementById("tokenInput").value;
    var message = document.getElementById("messageInput").value;
    var chatId = document.getElementById("chatIdInput").value;
    connection.invoke("SendMessage", chatId, token, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});