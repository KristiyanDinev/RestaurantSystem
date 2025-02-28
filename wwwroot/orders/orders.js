var socket = null
var regiseredOrders = [];

function onopen() {
    console.log('Connected')
    socket.send("subscribtion_ids;"+regiseredOrders.join(';'))
}

function onclose() {
    console.log('Closed')
}

function onmessage(event) {
    const data = event.data
    if (data.length == 0) {
        return;
    }
    console.log('message: '+data)
}

function onerror(event) {
    console.log('error: '+event.data)
}

function startWebSocket() {
    socket = startOrderWebSocket(onopen, onclose, onerror, onmessage)
}
