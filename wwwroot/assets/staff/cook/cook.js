var socket = null
var registeredOrders = []

async function setStatus(orderId, dishId, status) {
    if (socket == null) {
        return
    }

    let formData = new FormData()
    formData.append('OrderId', orderId)
    formData.append('OrderId', orderId)
    formData.append('OrderId', orderId)
    formData.append('OrderId', orderId)

    const res = await fetch(getDataFromLocalStorage("Host") + '/staff/dishes/', {
        method: 'POST',
        body: formData,
        redirect: 'follow'
    })

    if (res.status !== 200) {
        // document.getElementById('error').innerHTML = 'Error: ' + res.statusText
    }
    // Update
}

function onopen() {
    socket.send('{"orders": []}')
}

function onclose() {
}

function onmessage(event) {
    const data = event.data
    if (data.length == 0) {
        return;
    }
}

function onerror(event) {
}



function startWebSocket() {
    socket = startCookWebSocket(onopen, onclose, onerror, onmessage)
}

