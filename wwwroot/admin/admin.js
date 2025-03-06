var socket = null
var dishIds = []

function onopen() {
    console.log('Connected')
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