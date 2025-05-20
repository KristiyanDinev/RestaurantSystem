var socket = null
var registeredOrders = [];

function onopen() {
    if (registeredOrders.length == 0) {
        socket.close()
        return
    }
    socket.send('{"orders": [' + registeredOrders +']}')
}

function onclose() {
}

function onmessage(event) {
    const data = event.data
    if (data.length == 0) {
        return
    }

    // convert this to a json.
    //   public required int OrderId { get; set; }
    //    public string ? OrderCurrentStatus { get; set; }

    //    public int ? DishId { get; set; }
    //    public string ? DishCurrentStatus { get; set; }

    const obj = JSON.parse(data)
    if (!regiseredOrders.includes(Number(obj.OrderId))) {
        return
    }

    if (obj.OrderCurrentStatus !== null && obj.OrderCurrentStatus !== undefined) {
        document.getElementById("orderstatus_" + obj.OrderId).innerHTML = "CurrentStatus: " + obj.OrderCurrentStatus
    }

    if ((obj.DishId !== null && obj.DishId !== undefined) &&
        (obj.DishCurrentStatus !== null && obj.DishCurrentStatus !== undefined)) {
        document.getElementById("dishstatus_" + obj.OrderId + " " + obj.DishId).innerHTML = "CurrentStatus: " + obj.DishCurrentStatus
    }
}

function onerror(event) {
}

function startWebSocket() {
    socket = startOrderWebSocket(onopen, onclose, onerror, onmessage)
}


async function cancelOrder(id) {
    if (!confirm("Do you really want to cancel the order?")) {
        return;
    }

    const res = await fetch(getDataFromLocalStorage("Host") + '/order/stop/'+id, {
        method: 'POST',
        redirect: 'follow'
    })

    if (res.status !== 200) {
        alert("Can't stop the order")
        return;
    }

    alert("Stopped the order")
    registeredOrders = registeredOrders.filter(orderId => orderId !== Number(id));
}
