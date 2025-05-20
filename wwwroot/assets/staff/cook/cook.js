var socket = null
var registeredOrders = []

async function setStatus(orderId, dishId, status) {
    if (socket == null) {
        return
    }

    /*

    update only the dish status

        public required int OrderId { get; set; }
        public string? OrderCurrentStatus { get; set; }

        public int? DishId { get; set; }
        public string? DishCurrentStatus { get; set; }
        */

    let formData = new FormData()
    formData.append('OrderId', Number(orderId))
    formData.append('OrderCurrentStatus', null)
    formData.append('DishId', Number(dishId))
    formData.append('DishCurrentStatus', status)

    const res = await fetch(getDataFromLocalStorage("Host") + '/staff/dishes/', {
        method: 'POST',
        body: formData,
        redirect: 'follow'
    })

    if (res.status === 200) {
        // document.getElementById('error').innerHTML = 'Error: ' + res.statusText
        //document.getElementById("error," + orderId + "," + dishId).innerHTML = "Status of dish is not updated!"

    }

    //document.getElementById("dishstatus," + orderId + "," + dishId).innerHTML = "Dish Status: " + status
}

function onopen() {
    socket.send('{"orders": [' + registeredOrders + ']}')
}

function onclose() {
}

function onmessage(event) {
    const data = event.data
    if (data.length == 0) {
        return;
    }

    // convert this to a json.
}

function onerror(event) {
}



function startWebSocket() {
    socket = startOrderWebSocket(onopen, onclose, onerror, onmessage)
}

