let socket = null;
const registeredOrders = [];

// Update only the dish status for a specific order and dish
async function setStatus(orderId, dishId, status) {
    if (!socket) return;

    const formData = new FormData();
    formData.append('OrderId', Number(orderId));
    formData.append('OrderCurrentStatus', '');
    formData.append('DishId', Number(dishId));
    formData.append('DishCurrentStatus', status);

    try {
        const res = await fetch(`${getDataFromLocalStorage("Host")}/staff/dishes/`, {
            method: 'POST',
            body: formData,
            redirect: 'follow'
        });

        const elementId = res.status === 200
            ? `success,${orderId},${dishId}`
            : `error,${orderId},${dishId}`;

        const message = res.status === 200
            ? `Successfully updated the dish status to ${status}`
            : `Can't update dish status`;

        document.getElementById(elementId).innerHTML = message;
    } catch (error) {
        console.error('Error updating dish status:', error);
        document.getElementById(`error,${orderId},${dishId}`).innerHTML = "Can't update dish status";
    }
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
function onclose() { }

// WebSocket message event handler
function onmessage(event) {
    if (!event.data) return;

    const obj = JSON.parse(event.data);

    if (!registeredOrders.includes(Number(obj.OrderId))) return;

    if (obj.OrderCurrentStatus) {
        document.getElementById(`orderstatus,${obj.OrderId}`).innerHTML = `CurrentStatus: ${obj.OrderCurrentStatus}`;
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
