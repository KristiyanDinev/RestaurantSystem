let socket = null;
const registeredOrders = [];
const Status = {
    Pending: "Pending",
    Preparing: "Preparing",
    Ready: "Ready"
};

// Status badge classes mapping - matches GetStatusBadgeColor and GetStatusBadgeColorDish functions
const orderStatusBadgeClasses = {
    Pending: "warning",
    Preparing: "info", 
    Ready: "success",
    Served: "success",
    Delivering: "primary",
    Delivered: "success"
};

const dishStatusBadgeClasses = {
    Pending: "warning",
    Preparing: "info",
    Ready: "success"
};

// Status icons mapping - matches GetStatusIcon and GetStatusIconDish functions
const orderStatusIcons = {
    Pending: "hourglass-split",
    Preparing: "fire",
    Ready: "check2",
    Served: "check2-all", 
    Delivering: "truck",
    Delivered: "check2-all"
};

const dishStatusIcons = {
    Pending: "hourglass-split",
    Preparing: "fire",
    Ready: "check2"
};

// helper function
function createButton(className, text, onclickHandler, icon = null) {
    const button = document.createElement("button");
    button.className = className;
    button.onclick = onclickHandler;
    button.type = "button";
    
    if (icon) {
        button.innerHTML = `<i class="${icon} me-1"></i>${text}`;
    } else {
        button.innerHTML = text;
    }
    
    return button;
}

function setDishButtons(obj) {
    let container = document.getElementById(`dish_buttons,${obj.OrderId},${obj.DishId}`);
    container.innerHTML = "";
    container.className = "d-grid gap-2";
    
    const status = obj.DishCurrentStatus;
    
    if (status == Status.Pending) {
        const startCookingBtn = createButton(
            "btn btn-success btn-sm",
            "Start Cooking",
            () => setStatus(obj.OrderId, obj.DishId, Status.Preparing),
            "bi-play-fill"
        );
        container.appendChild(startCookingBtn);
    } else if (status == Status.Preparing) {
        const pendingBtn = createButton(
            "btn btn-warning btn-sm mb-1",
            "Mark Pending",
            () => setStatus(obj.OrderId, obj.DishId, Status.Pending),
            "bi-arrow-left-circle"
        );
        container.appendChild(pendingBtn);
        
        const readyBtn = createButton(
            "btn btn-success btn-sm",
            "Dish Ready",
            () => setStatus(obj.OrderId, obj.DishId, Status.Ready),
            "bi-check-circle-fill"
        );
        container.appendChild(readyBtn);
    } else if (status == Status.Ready) {
        const preparingBtn = createButton(
            "btn btn-warning btn-sm",
            "Back to Preparing",
            () => setStatus(obj.OrderId, obj.DishId, Status.Preparing),
            "bi-arrow-left-circle"
        );
        container.appendChild(preparingBtn);
    }
}

// Helper function to show/hide alert messages
function showAlert(elementId, message, isSuccess = true) {
    const element = document.getElementById(elementId);
    if (!element) return;
    
    const span = element.querySelector('span');
    if (span) {
        span.textContent = message;
    } else {
        // Fallback if span doesn't exist
        const icon = element.querySelector('i');
        if (icon) {
            icon.nextSibling.textContent = message;
        }
    }
    
    element.classList.remove('d-none');
    
    // Auto-hide success messages after 5 seconds
    if (isSuccess) {
        setTimeout(() => {
            element.classList.add('d-none');
        }, 5000);
    }
}

function hideAlert(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.classList.add('d-none');
    }
}

// Update only the dish status for a specific order and dish
async function setStatus(orderId, dishId, status) {
    if (!socket) return;
    
    const formData = new FormData();
    formData.append('OrderId', orderId);
    formData.append('OrderCurrentStatus', '');
    formData.append('DishId', dishId);
    formData.append('DishCurrentStatus', status);
    
    try {
        const res = await fetch('/staff/dishes/', {
            method: 'POST',
            body: formData
        });
        
        const errorElementId = `error,${orderId},${dishId}`;
        const successElementId = `success,${orderId},${dishId}`;
        
        // Hide both alerts first
        hideAlert(errorElementId);
        hideAlert(successElementId);
        
        if (res.ok) {
            showAlert(successElementId, `Successfully updated the dish status to ${status}`, true);
        } else {
            showAlert(errorElementId, "Can't update dish status", false);
        }
    } catch (error) {
        const errorElementId = `error,${orderId},${dishId}`;
        hideAlert(`success,${orderId},${dishId}`);
        showAlert(errorElementId, "Can't update dish status", false);
    }
}

// WebSocket open event handler
function onopen() {
    if (registeredOrders.length == 0) {
        socket.close();
        return;
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
    
    // Update order status
    if (obj.OrderCurrentStatus) {
        const orderStatusElement = document.getElementById(`orderstatus,${obj.OrderId}`);
        if (orderStatusElement) {
            const badgeClass = orderStatusBadgeClasses[obj.OrderCurrentStatus] || "secondary";
            const iconClass = orderStatusIcons[obj.OrderCurrentStatus] || "hourglass-split";
            
            orderStatusElement.className = `badge bg-${badgeClass} fs-6 px-3 py-2 text-dark fw-bold`;
            orderStatusElement.innerHTML = `<i class="bi bi-${iconClass} me-1"></i>${obj.OrderCurrentStatus}`;
        }
    }
    
    // Update dish status
    if (obj.DishId && obj.DishCurrentStatus) {
        const dishStatusElement = document.getElementById(`dishstatus,${obj.OrderId},${obj.DishId}`);
        if (dishStatusElement) {
            const badgeClass = dishStatusBadgeClasses[obj.DishCurrentStatus] || "secondary";
            const iconClass = dishStatusIcons[obj.DishCurrentStatus] || "hourglass-split";
            
            dishStatusElement.className = `badge bg-${badgeClass} mb-2 text-dark`;
            dishStatusElement.innerHTML = `<i class="bi bi-${iconClass} me-1 text-dark"></i>${obj.DishCurrentStatus}`;
        }
    }
    
    setDishButtons(obj);
}

// WebSocket error event handler (currently empty)
function onerror(event) { }

// Start the WebSocket connection
function startWebSocket() {
    socket = startOrderWebSocket(onopen, onclose, onerror, onmessage);
}