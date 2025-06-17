

async function startOrder() {
    if (getCookie(restaurantId_header).length == 0) {
        alert("Select a restorant")
        window.location.pathname = "/restaurants"
        return;
    }

    if (getCookie(cart_header).length == 0) {
        alert("You don't have any dishes to order as of now.")
        window.location.pathname = "/dishes"
        return;
    }

    let address_id = document.getElementById("address").value
    if (!address_id) {
        document.getElementById("error").innerHTML = "Please select an address to continue."
        return;
    }

    let formData = new FormData()
    formData.append("Notes", document.getElementById("notes").value)
    formData.append("CuponCode", document.getElementById("cupon_input").value)
    formData.append("AddressId", address_id)

    const res = await fetch('/order/start', {
        method: 'POST',
        body: formData,
    })

    if (res.status == 200) {
        window.location.pathname = "/orders"

    } else {
        document.getElementById("error").innerHTML = "Couldn't start your order."
    }
}