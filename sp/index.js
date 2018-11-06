const SerialPort = require('serialport')
const axios = require('axios');
const apiUrl = "http://192.168.1.105:5000/devices/report"
const tty = "/dev/ttyUSB0";
// const tty = "/dev/tty.wchusbserial1420";
const masterId = '8B4A27E7-267A-46B2-8995-84D42C083FAE'
axios.defaults.headers.post['Content-Type'] = 'application/json;charse=UTF-8'
function send(data) {
    axios.post(apiUrl, data)
        .then(function (response) {
            console.log(response.data);
        })
        .catch(function (error) {
            console.log(error);
        });
}

const port = new SerialPort(tty, {
    baudRate: 19200
})

port.on('readable', function () {
    port.read();
})

// Switches the port into "flowing mode"
port.on('data', function (data) {
    console.log('Data:', data)
    let jsonText = JSON.stringify(data);
    let bufferObj = JSON.parse(jsonText);
    let jsonData = { data: bufferObj.data, masterId: masterId }
    send(jsonData);
})


// const signalR = require("@aspnet/signalr");
// XMLHttpRequest = require("xmlhttprequest").XMLHttpRequest;
// WebSocket = require("websocket").w3cwebsocket;

// let connection = new signalR.HubConnectionBuilder()
//     .withUrl("http://192.168.1.105:5000/deviceHub")
//     .build();

// connection.on("ReceiveMessage", (user, message) => {
//     console.log(`${user} ${message}`);
// });

// connection.start().then(() => {
//     console.log('connected to hub');
// })

// Pipe the data into another stream (like a parser or standard out)
// const lineStream = port.pipe(new Readline())
