// const SerialPort = require('serialport')
// const port = new SerialPort('/dev/tty.wchusbserial1410', {
//     baudRate: 19200
// })

// port.on('readable', function () {
//     console.log('Data:', port.read())
// })

// // Switches the port into "flowing mode"
// port.on('data', function (data) {
//     console.log('Data:', data)
// })

const signalR = require("@aspnet/signalr");
XMLHttpRequest = require("xmlhttprequest").XMLHttpRequest;
WebSocket = require("websocket").w3cwebsocket;

let connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/deviceHub")
    .build();

connection.on("ReceiveMessage", (user, message) => {
    console.log(`${user} ${message}`);
});

connection.start()
    .then(() => connection.invoke("SendMessage", "Hello", "World"));



// Pipe the data into another stream (like a parser or standard out)
// const lineStream = port.pipe(new Readline())