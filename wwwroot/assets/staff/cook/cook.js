let socket = null;
const registeredOrders = [];

const Status = {
    Pending: "Pending",
    Preparing: "Preparing",
    Ready: "Ready"
};

// helper function
function createButton(className, text, onclickHandler) {
    const button = document.createElement("button");
    button.className = className;
    button.textContent = text;
    button.onclick = onclickHandler;
    return button;
}


function setDishButtons(obj) {
    const container = document.getElementById(`dish_buttons,${obj.OrderId},${obj.DishId}`);
    container.innerHTML = "";

    const status = obj.DishCurrentStatus.toLowerCase();

    if (status === Status.Pending.toLowerCase()) {
        const startCookingBtn = createButton(
            "order noselect",
            "Start cooking",
            () => setStatus(obj.OrderId, obj.DishId, Status.Preparing.toLowerCase())
        );
        container.appendChild(startCookingBtn);

    } else if (status === Status.Preparing.toLowerCase()) {
        const rowDiv = document.createElement("div");
        rowDiv.className = "row noselect";

        const undoBtn = createButton(
            "order noselect",
            "Pending",
            () => setStatus(obj.OrderId, obj.DishId, Status.Pending.toLowerCase())
        );
        rowDiv.appendChild(undoBtn);

        const readyBtn = createButton(
            "order noselect",
            "Dish Ready",
            () => setStatus(obj.OrderId, obj.DishId, Status.Ready.toLowerCase())
        );
        rowDiv.appendChild(readyBtn);

        container.appendChild(rowDiv);

    } else if (status === Status.Ready.toLowerCase()) {
        const undoBtn = createButton(
            "order noselect",
            "Preparing",
            () => setStatus(obj.OrderId, obj.DishId, Status.Preparing.toLowerCase())
        );
        container.appendChild(undoBtn);
    }
}

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

        setDishButtons(obj)
    }
}

// WebSocket error event handler (currently empty)
function onerror(event) { }

// Start the WebSocket connection
function startWebSocket() {
    socket = startOrderWebSocket(onopen, onclose, onerror, onmessage);
}
