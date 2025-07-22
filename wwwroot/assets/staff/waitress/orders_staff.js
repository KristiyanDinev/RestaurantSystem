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
        let element = document.getElementById(`orderstatus,${obj.OrderId}`)

        let orderStatusBadgeColor = 'primary'
        if (obj.OrderCurrentStatus == 'Pending') {
            orderStatusBadgeColor = 'warning'

        } else if (obj.OrderCurrentStatus == 'Preparing') {
            orderStatusBadgeColor = 'info'

        } else if (obj.OrderCurrentStatus == 'Ready' || 
                    obj.OrderCurrentStatus == 'Served' ||
                    obj.OrderCurrentStatus == 'Delivered') {
            orderStatusBadgeColor = 'success'
        }

        let orderStatusIcon = ''
        if (obj.OrderCurrentStatus == 'Pending') {
            orderStatusIcon = 'hourglass-split'

        } else if (obj.OrderCurrentStatus == 'Preparing') {
            orderStatusIcon = 'fire'

        } else if (obj.OrderCurrentStatus == 'Ready') {
            orderStatusIcon = 'check2'

        } else if (obj.OrderCurrentStatus == 'Served' ||
                    obj.OrderCurrentStatus == 'Delivered') {
            orderStatusIcon = 'check2-all'
        }


        element.className = `badge bg-${orderStatusBadgeColor} fs-6 px-3 py-2 text-dark`
        element.innerHTML = `<i class="bi bi-${orderStatusIcon} text-dark me-1"></i>${obj.OrderCurrentStatus}`;
    }

    if (obj.DishId && obj.DishCurrentStatus) {
        let element2 = document.getElementById(`dishstatus,${obj.OrderId},${obj.DishId}`)

        let dishStatusBadgeColor = 'primary'
        if (obj.DishCurrentStatus == 'Pending') {
            dishStatusBadgeColor = 'warning'

        } else if (obj.DishCurrentStatus == 'Preparing') {
            dishStatusBadgeColor = 'info'

        } else if (obj.DishCurrentStatus == 'Ready') {
            dishStatusBadgeColor = 'success'
        }

        let dishStatusIcon = 'primary'
        if (obj.DishCurrentStatus == 'Pending') {
            dishStatusIcon = 'hourglass-split'

        } else if (obj.DishCurrentStatus == 'Preparing') {
            dishStatusIcon = 'fire'

        } else if (obj.DishCurrentStatus == 'Ready') {
            dishStatusIcon = 'check2'
        }
        element2.className = `badge bg-${dishStatusBadgeColor} mb-2 text-dark`
        element2.innerHTML = `<i class="bi bi-${dishStatusIcon} me-1 text-dark"></i>${obj.DishCurrentStatus}`;
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