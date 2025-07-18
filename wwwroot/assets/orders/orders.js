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

// Status color mapping for Bootstrap badges
const StatusColors = {
    Pending: "warning",
    Preparing: "info",
    Ready: "success",
    Served: "success",
    Delivering: "primary",
    Delivered: "success"
};

// Status icon mapping for Bootstrap icons
const StatusIcons = {
    Pending: "hourglass-split",
    Preparing: "fire",
    Ready: "check-circle",
    Served: "check-circle-fill",
    Delivering: "truck",
    Delivered: "check2-circle"
};

// Helper function to get Bootstrap badge color based on status
function getStatusBadgeColor(status) {
    return StatusColors[status] || "secondary";
}

// Helper function to get Bootstrap icon based on status
function getStatusIcon(status) {
    return StatusIcons[status] || "question-circle";
}

// Helper function to update last updated time
function updateLastUpdatedTime() {
    const lastUpdatedElement = document.getElementById('lastUpdated');
    if (lastUpdatedElement) {
        const now = new Date();
        lastUpdatedElement.textContent = now.toLocaleTimeString();
    }
}

// Helper function to update badge styling
function updateBadgeStatus(element, status) {
    // Remove all existing badge color classes
    const badgeColorClasses = ['bg-warning', 'bg-info', 'bg-success', 'bg-primary', 'bg-secondary', 'bg-danger'];
    badgeColorClasses.forEach(cls => element.classList.remove(cls));

    // Add new badge color class
    element.classList.add(`bg-${getStatusBadgeColor(status)}`);

    // Update icon and text
    const icon = getStatusIcon(status);
    element.innerHTML = `<i class="bi bi-${icon} me-1"></i>${status}`;
}

// helper function
function setCancelButton(orderId, status) {
    const btn = document.getElementById(`cancel,${orderId}`);

    if (!btn) return;

    if (status.toLowerCase() !== Status.Pending.toLowerCase()) {
        // Update button to disabled state
        btn.className = "btn btn-secondary";
        btn.innerHTML = '<i class="bi bi-lock me-2"></i> Can\'t Cancel';
        btn.disabled = true;
        btn.onclick = null;
        return;
    }

    // Update button to active state
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

    // Update last updated time
    updateLastUpdatedTime();

    // Update order status
    if (obj.OrderCurrentStatus) {
        const orderStatusElement = document.getElementById(`orderstatus,${obj.OrderId}`);
        if (orderStatusElement) {
            updateBadgeStatus(orderStatusElement, obj.OrderCurrentStatus);
        }
    }

    // Update dish status
    if (obj.DishId && obj.DishCurrentStatus) {
        const dishStatusElement = document.getElementById(`dishstatus,${obj.OrderId},${obj.DishId}`);
        if (dishStatusElement) {
            updateBadgeStatus(dishStatusElement, obj.DishCurrentStatus);
        }
    }

    // Update cancel button state
    setCancelButton(obj.OrderId, obj.OrderCurrentStatus);
}

// WebSocket error event handler (currently empty)
function onerror(event) {
    console.error('WebSocket error:', event);
}

// Start the WebSocket connection
function startWebSocket() {
    socket = startOrderWebSocket(onopen, onclose, onerror, onmessage)
}

// Cancel an order by its ID
async function cancelOrder(id) {
    if (!confirm("Are you sure you want to cancel this order?")) {
        return
    }

    // Show loading state on button
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

    // Restore button state on error
    cancelBtn.innerHTML = originalContent;
    cancelBtn.disabled = false;
}

updateLastUpdatedTime();

