var socket = null
var regiseredOrders = [];

function onopen() {
    // convert this to a json: {"orders": [1, 2, 3]}
    regiseredOrders
    socket.send()
}

function onclose() {
}

function onmessage(event) {
    const data = event.data
    if (data.length == 0) {
        return;
    }

    // convert this to a json.
}

function onerror(event) {
}

function startWebSocket() {
    socket = startOrderWebSocket(onopen, onclose, onerror, onmessage)
}


async function cancelOrder(id) {
    if (!confirm("Do you really want to cancel the order?")) {
        return;
    }

    const res = await fetch(getDataFromLocalStorage("Host") + '/order/stop/'+id, {
        method: 'POST',
        redirect: 'follow'
    })

    if (res.status !== 200) {
        alert("Can't cancel the order")
        return;
    }

    alert("Cancelled the order")
    window.location.reload()
}
