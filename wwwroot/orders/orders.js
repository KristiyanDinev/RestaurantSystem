var socket = null;
var regiseredOrders = [];

async function onopen(event) {
    console.log('Connected')
    socket.send("subscribtion_ids="+regiseredOrders.join(';'))
}

async function onclose(event) {
    console.log('Closed')
}

async function onmessage(event) {
    const data = event.data
    if (data.length == 0) {
        return;
    }
    console.log('message: '+data)
}

async function onerror(event) {
    console.log('error: '+event.data)
}

function startWebSocket() {
    socket = startOrderWebSocket(onopen, onclose, onerror, onmessage)
}

function closeWebSocket() {
   socket.close(1000, "Closing from client")
}