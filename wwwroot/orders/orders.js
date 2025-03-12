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

    const parts = data.split(';')
    if (parts[0] === "cook_status") {
        const orderId = Number(parts[1])
        //const dishId = Number(parts[2])
        const status = parts[3]

        var cancel = document.getElementById(orderId+'cancel')
        if (status !== "pending") {
            cancel.onclick = () => {};
            cancel.style.opacity = '50%'

        } else {
            cancel.onclick = async () => {
                await cancelOrder(orderId)
            };
            cancel.style.opacity = '100%'
        }

        document.getElementById(orderId+'status')
        .innerHTML = 'CurrentStatus: '+status
    }
}

function onerror(event) {
    console.log('error: '+event.data)
}

function startWebSocket() {
    socket = startOrderWebSocket(onopen, onclose, onerror, onmessage)
}
