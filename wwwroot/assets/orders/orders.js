let socket = null
const registeredOrders = [];

const Status = {
    Pending: "Pending",
    Preparing: "Preparing",
    Ready: "Ready",
    Delivering: "Delivering",
    Delivered: "Delivered",
    Served: "Served"
};

const StatusColors = {
    Pending: "warning",
    Preparing: "info",
    Ready: "success",
    Served: "success",
    Delivering: "primary",
    Delivered: "success"
};

const StatusIcons = {
    Pending: "hourglass-split",
    Preparing: "fire",
    Ready: "check-circle",
    Served: "check-circle-fill",
    Delivering: "truck",
    Delivered: "check2-circle"
};

function getStatusBadgeColor(status) {
    return StatusColors[status] || "secondary";
}

function getStatusIcon(status) {
    return StatusIcons[status] || "question-circle";
}

function updateLastUpdatedTime() {
    const lastUpdatedElement = document.getElementById('lastUpdated');
    if (lastUpdatedElement) {
        const now = new Date();
        lastUpdatedElement.textContent = now.toLocaleTimeString();
    }
}

function updateBadgeStatus(element, status) {
    const badgeColorClasses = ['bg-warning', 'bg-info', 'bg-success', 'bg-primary', 'bg-secondary', 'bg-danger'];
    badgeColorClasses.forEach(cls => element.classList.remove(cls));
    element.classList.add(`bg-${getStatusBadgeColor(status)}`);
    const icon = getStatusIcon(status);
    element.innerHTML = `<i class="bi bi-${icon} me-1"></i>${status}`;
}

function setCancelButton(orderId, status) {
    const btn = document.getElementById(`cancel,${orderId}`);

    if (!btn) return;

    if (status.toLowerCase() !== Status.Pending.toLowerCase()) {
        btn.className = "btn btn-secondary";
        btn.innerHTML = '<i class="bi bi-lock me-2"></i> Can\'t Cancel';
        btn.disabled = true;
        btn.onclick = null;
        return;
    }

    btn.className = "btn btn-outline-danger";
    btn.innerHTML = '<i class="bi bi-x-circle me-2"></i> Cancel Order';
    btn.disabled = false;
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
    updateLastUpdatedTime();

    if (obj.OrderCurrentStatus) {
        const orderStatusElement = document.getElementById(`orderstatus,${obj.OrderId}`);
        if (orderStatusElement) {
            updateBadgeStatus(orderStatusElement, obj.OrderCurrentStatus);
        }
    }

    if (obj.DishId && obj.DishCurrentStatus) {
        const dishStatusElement = document.getElementById(`dishstatus,${obj.OrderId},${obj.DishId}`);
        if (dishStatusElement) {
            updateBadgeStatus(dishStatusElement, obj.DishCurrentStatus);
        }
    }

    setCancelButton(obj.OrderId, obj.OrderCurrentStatus);
}

function onerror(event) {
    console.error('WebSocket error:', event);
}

function startWebSocket() {
    socket = startOrderWebSocket(onopen, onclose, onerror, onmessage)
}

async function cancelOrder(id) {
    if (!confirm("Are you sure you want to cancel this order?")) {
        return
    }

    const cancelBtn = document.getElementById(`cancel,${id}`);
    const originalContent = cancelBtn.innerHTML;
    cancelBtn.innerHTML = '<i class="bi bi-hourglass-split me-2"></i> Cancelling...';
    cancelBtn.disabled = true;

    try {
        await fetch(`/order/cancel/${id}`, {
            method: 'POST'
        })

         window.location.reload();
         return
    } catch {}

    cancelBtn.innerHTML = originalContent;
    cancelBtn.disabled = false;
}

updateLastUpdatedTime();

