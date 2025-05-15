var socket = null
var orderIds = {}

function setStatus(orderId, dishId, status) {
    if (socket == null) {
        return
    }

    socket.send('cook_status;'+orderId+';'+dishId+';'+status)
}

function onopen() {
    console.log('Connected')
    socket.send("subscribtion_ids;"+Object.keys(orderIds).join(';'))
}

function onclose() {
    console.log('Closed')
}

function onmessage(event) {
    const data = event.data
    if (data.length == 0) {
        return;
    }
    console.log('message: '+data)

    const parts = data.split(';')
    if (parts[0] === "reload") {
        window.location.reload()
    }
    /*
    if (parts[0] === "cook_status") {
        const orderId = Number(parts[1])
        const dishId = Number(parts[2])
        const status = parts[3]

        let dishButton = document.getElementById(orderId+'.'+dishId+'btn')  id="{{Dish.OrderId}}.{{Dish.Id}}btn"
        dishButton.onclick = () => {
            dishReady(orderId, dishId)
        };
        dishButton.innerHTML = "Ready"

        document.getElementById(orderId+'.'+dishId+'status').innerHTML = 'Dish Status: '+status id="{{Dish.OrderId}}.{{Dish.Id}}status"
        document.getElementById(orderId+'status').innerHTML = 'CurrentStatus: '+status
    }*/
}

function onerror(event) {
    console.log('error: '+event.data)
}




function startWebSocket() {
    socket = startCookWebSocket(onopen, onclose, onerror, onmessage)
}

