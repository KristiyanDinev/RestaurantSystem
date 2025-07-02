let socket = null;
const registeredOrders = [];

const Status = {
    Pending: "Pending",
    Preparing: "Preparing",
    Ready: "Ready"
};

// helper function
function createButton(className, text, onclickHandler) {
    const button = document.createElement("button")
    button.className = className
    button.onclick = onclickHandler
    button.innerHTML = text
    button.type = "button"
    return button;
}


function setDishButtons(obj) {
    let container = document.getElementById(`dish_buttons,${obj.OrderId},${obj.DishId}`);
    container.innerHTML = "";

    const status = obj.DishCurrentStatus;
    if (status == Status.Pending) {
        // Status.Pending
        const startCookingBtn = createButton(
            "btn btn-success m-2",
            "Start cooking",
            () => setStatus(obj.OrderId, obj.DishId, Status.Preparing)
        );
        container.appendChild(startCookingBtn);

    } else if (status == Status.Preparing) {
        // Status.Preparin
        const readyBtn = createButton(
            "btn btn-success m-2",
            "Dish Ready",
            () => setStatus(obj.OrderId, obj.DishId, Status.Ready)
        );
        container.appendChild(readyBtn);

        const undoBtn = createButton(
            "btn btn-primary m-2",
            "Pending",
            () => setStatus(obj.OrderId, obj.DishId, Status.Pending)
        );
        container.appendChild(undoBtn);

    } else if (status == Status.Ready) {
        // Status.Ready
        const undoBtn = createButton(
            "btn btn-primary m-2",
            "Preparing",
            () => setStatus(obj.OrderId, obj.DishId, Status.Preparing)
        );
        container.appendChild(undoBtn);
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

        const errorElement = document.getElementById(`error,${orderId},${dishId}`);
        const successElement = document.getElementById(`success,${orderId},${dishId}`);
        errorElement.innerHTML = "";
        successElement.innerHTML = "";

        if (res.ok) {
            successElement.innerHTML = `Successfully updated the dish status to ${status}`;

        } else {
            errorElement.innerHTML = "Can't update dish status";
        }

    } catch (error) {
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

    if (!registeredOrders.includes(String(obj.OrderId))) return;

    if (obj.OrderCurrentStatus) {
        document.getElementById(`orderstatus,${obj.OrderId}`).innerHTML = `Current Status: ${obj.OrderCurrentStatus}`;
    }

    if (obj.DishId && obj.DishCurrentStatus) {
        document.getElementById(`dishstatus,${obj.OrderId},${obj.DishId}`).innerHTML = `Dish Status: ${obj.DishCurrentStatus}`;

        setDishButtons(obj)
    }
}

// WebSocket error event handler (currently empty)
function onerror(event) { }

// Start the WebSocket connection
function startWebSocket() {
    socket = startOrderWebSocket(onopen, onclose, onerror, onmessage);
}
