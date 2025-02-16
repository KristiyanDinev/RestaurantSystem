


function displayOrders(data) {
    var mainDoc = document.getElementById('orders')

    var orderDiv = document.createElement('div')
    orderDiv.className = 'order_view'

    let title = document.createElement('p')
    title.className = 'title'
    title


    mainDoc.appendChild(orderDiv)
}


async function getOrders() {


    const res = await fetch(Host + '/orders', {
        method: "POST",
        redirect: 'follow',
    })

    const data = await res.json()
    const orders = data.orders
    if (orders === null || orders === undefined) {
        document.getElementById("order_stats").innerHTML = "Can't get current orders"
        return
    }

    console.log(orders)
    for (let x in orders) {
        displayOrders(orders[x])
    }
}