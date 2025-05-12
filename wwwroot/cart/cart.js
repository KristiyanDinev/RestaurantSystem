function calculateQuantity(dishId) {
    let cart = getCookie("cart");
    let count = 0;
    for (let i in cart.split("-")) {
        if (Number(i) == Number(dishId)) {
            count++;
        }
    }
    document.getElementById('q_' + dishId).innerHTML = "Quantity: " + count;
}

    
async function startOrder() {
    let cartCookie = getCookie('cart')
    if (cartCookie.length == 0) {
        alert("You don't have any dishes to order as of now.")
        return;
    }

    let restorantId = getCookie('RestorantId')
    if (restorantId === '') {
        alert("Select a restorant")
        return
    }

    var formData = new FormData()
    formData.append("notes", document.getElementById("notes").value)
    formData.append("cuponCode", document.getElementById("cupon_input").value)

    const res = await fetch(getDataFromLocalStorage("Host") + '/order', {
        method: 'POST',
        body: formData,
        redirect: 'follow',
    })

    if (res.status === 200) {
        alert("Your started your order.")
        window.location.reload()

    } else {
        alert("Couldn't start your order.")
    }
}