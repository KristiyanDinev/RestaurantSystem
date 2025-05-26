

async function startOrder() {
    if (getCookie(restaurantId_header).length == 0) {
        alert("Select a restorant")
        window.location.href = "/restaurants"
        return;
    }

    if (getCookie(cart_header).length == 0) {
        alert("You don't have any dishes to order as of now.")
        window.location.href = "/dishes"
        return;
    }

    var formData = new FormData()
    formData.append("Notes", document.getElementById("notes").value)
    formData.append("CuponCode", document.getElementById("cupon_input").value)

    const res = await fetch(getDataFromLocalStorage("Host") + '/order/start', {
        method: 'POST',
        body: formData,
        redirect: 'follow',
    })

    if (res.status == 200) {
        window.location.pathname = "/orders"

    } else {
        document.getElementById("error").value = "Error: Couldn't start your order."
    }
}