var socket = null;
const registeredOrders = [];

// WebSocket open event handler
function onopen() {
    if (registeredOrders.length == 0) {
        socket.close()
        return
    }

    socket.send(JSON.stringify({ orders: registeredOrders }));
}

// WebSocket close event handler (currently empty)
function onclose() { }

// WebSocket message event handler
function onmessage(event) {
    if (!event.data) return;

    const obj = JSON.parse(event.data);

    if (!registeredOrders.includes(String(obj.OrderId))) return;

    if (obj.OrderCurrentStatus) {
        document.getElementById(`orderstatus,${obj.OrderId}`).innerHTML = `Current Status: ${obj.OrderCurrentStatus}`;
    }

    if (obj.DishId && obj.DishCurrentStatus) {
        document.getElementById(`dishstatus,${obj.OrderId},${obj.DishId}`).innerHTML = `Dish Status: ${obj.DishCurrentStatus}`;
    }
}

// WebSocket error event handler (currently empty)
function onerror(event) { }

// Start the WebSocket connection
function startWebSocket() {
    socket = startOrderWebSocket(onopen, onclose, onerror, onmessage);
}


async function deleteOrder(orderId) {
    if (!confirm("Are you sure you want to delete this order?")) return;

    const statusElement = document.getElementById(`stats,${orderId}`);
    try {
        const response = await fetch(`/staff/orders/delete/${orderId}`, {
            method: 'POST'
        })

        if (response.ok) {
            window.location.reload();
            return;
        }

    } catch {
    }

    statusElement.innerHTML = "Couldn't delete the order";
}


async function servedOrder(orderId) {
    if (!confirm("Are you sure you want to mark this order as served?")) return;

    const statusElement = document.getElementById(`stats,${orderId}`);
    let formData = new FormData();
    formData.append("IsServed", true);

    try {
        const response = await fetch(`/staff/orders/serve/${orderId}`, {
            method: 'POST',
            body: formData
        })

        if (response.ok) {
            window.location.reload();
            return;
        }

    } catch {
    }

    statusElement.innerHTML = "Couldn't mark the order as served";
}


async function unserveOrder(orderId) {
    if (!confirm("Are you sure you want to mark this order as unserved?")) return;

    const statusElement = document.getElementById(`stats,${orderId}`);
    let formData = new FormData();
    formData.append("IsServed", false);

    try {
        const response = await fetch(`/staff/orders/serve/${orderId}`, {
            method: 'POST',
            body: formData
        })

        if (response.ok) {
            window.location.reload();
            return;
        }
    } catch {
    }
    statusElement.innerHTML = "Couldn't mark the order as unserved";
}