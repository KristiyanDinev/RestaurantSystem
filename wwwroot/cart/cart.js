

    
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