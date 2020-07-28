"use strict";

let connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (chatId, user, message) {
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
    let token = document.getElementById("tokenInput").value;
    let message = document.getElementById("messageInput").value;
    let chatId = document.getElementById("chatIdInput").value;
    let id = Number(chatId)
    connection.invoke("SendMessage", id, token, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});