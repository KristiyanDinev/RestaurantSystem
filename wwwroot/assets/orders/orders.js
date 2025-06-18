let socket = null
const registeredOrders = [];


const Status = {
    Pending: "Pending",
    Preparing: "Preparing",
    Ready: "Ready"
};

// helper function
function setCancelButton(orderId, status) {
    const btn = document.getElementById(`cancel,${orderId}`);

    if (status.toLowerCase() !== Status.Pending.toLowerCase()) {
        btn.innerHTML = "Can't Cancel Order";
        return
    }
    btn.innerHTML = "Cancel";
    btn.onclick = () => cancelOrder(orderId);
}



// WebSocket open event handler
function onopen() {
    if (registeredOrders.length == 0) {
        socket.close()
        return
    }
    socket.send(JSON.stringify({ orders: registeredOrders }));
}

// WebSocket close event handler (currently empty)
function onclose() {
}

// WebSocket message event handler
function onmessage(event) {
    const data = event.data
    if (data.length == 0) {
        return
    }

    const obj = JSON.parse(data)

    if (!registeredOrders.includes(String(obj.OrderId))) {
        return
    }

    if (obj.OrderCurrentStatus) {
        document.getElementById(`orderstatus,${obj.OrderId}`)
            .innerHTML = "Current Status: " + obj.OrderCurrentStatus
    }

    if (obj.DishId && obj.DishCurrentStatus) {
        document.getElementById(`dishstatus,${obj.OrderId},${obj.DishId}`)
            .innerHTML = "Current Status: " + obj.DishCurrentStatus
    }

    setCancelButton(obj.OrderId, obj.OrderCurrentStatus)
}

// WebSocket error event handler (currently empty)
function onerror(event) {
}


// Start the WebSocket connection
function startWebSocket() {
    socket = startOrderWebSocket(onopen, onclose, onerror, onmessage)
}

// Cancel an order by its ID
async function cancelOrder(id) {
    if (!confirm("Do you really want to cancel the order?")) {
        return
    }

    try {
        const res = await fetch(`/order/cancel/${id}`, {
            method: 'POST'
        })

        if (res.ok) {
            window.location.reload();
            return
        }

    } catch {
    }

    document.getElementById(`error,${id}`).innerHTML = "Can't cancel order";
}
