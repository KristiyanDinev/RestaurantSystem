

function calculateQuantity(dishId) {
    let count = 0;
    for (let i in getCookie(cart_header).split(_cart_seperator)) {
        if (Number(i) == Number(dishId)) {
            count++;
        }
    }
    document.getElementById('q_' + dishId).innerHTML = "Quantity: " + count;
}


async function startOrder() {
    let cartCookie = getCookie(cart_header)
    if (cartCookie.length == 0) {
        alert("You don't have any dishes to order as of now.")
        window.location.href = "/dishes"
        return;
    }

    if (getCookie(restaurantId_header).length == 0) {
        alert("Select a restorant")
        window.location.href = "/restaurants"
        return;
    }

    let dishes = []
    for (let dish in cartCookie.split(_cart_seperator)) {
        dishes.push(Number(dish))
    }

    var formData = new FormData()
    formData.append("Notes", document.getElementById("notes").value)
    formData.append("CuponCode", document.getElementById("cupon_input").value)
    formData.append("Dishes", dishes)

    const res = await fetch(getDataFromLocalStorage("Host") + '/order/start', {
        method: 'POST',
        body: formData,
        redirect: 'follow',
    })

    if (res.status === 200) {
        alert("Your started your order.")
        window.location.href = "/orders"

    } else {
        alert("Couldn't start your order.")
    }
}