
async function startOrder() {
    let restorant_add = document.getElementById("restorant_address").value;
    if (restorant_add.length == 0) {
        alert("You can't order without restorant address.")
        return;
    }

    if (restorant_add === undefined || restorant_add.length === 0) {
        document.getElementById("dish_stats").innerHTML = "Restorant Address is required."
        return;
    }

    var formData = new FormData()
    formData.append("notes", document.getElementById("notes").value)
    formData.append("cuponCode", document.getElementById("cupon_input").value)
    formData.append("restorantAddress", restorant_add)

    const res = await fetch(Host + '/order', {
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