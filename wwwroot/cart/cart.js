
async function startOrder() {
    let restorant_add = document.getElementById("restorant_address").value;

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

async function rmDish(name, id) {
    if (confirm("Are you sure you want to remove 1 "+name+" from your cart?") != true) {
        return
    }

    let formData = new FormData()
    formData.append("dishId", Number(id))

    const res = await fetch(Host + '/order/remove', {
        method: "POST",
        body: formData,
        redirect: 'follow',
    })

    if (res.status === 200) {
        alert("Removed 1 "+name+" from your cart")
        window.location.reload()

    } else {
        alert("Can't remove 1 "+name+" from your cart")
    }
}

async function addDishToOrder(price, id) {
    var formData = new FormData()
    formData.append("dishId", Number(id))
    formData.append("dishPrice", parseFloat(price))

    const res = await fetch(Host + "/order/add", {
        method: "POST",
        body: formData,
        redirect: 'follow',
    })

    if (res.status === 200) {
        alert('Added 1 '+name+' to your cart')
        window.location.reload()

    } else {
        alert("Can't add 1 "+name+" to your cart")
    }
}

function goToDishByID(id) {
    let params = new URLSearchParams()
    params.append("dishId", String(id))
    window.location.href = "/dish/id?" + params.toString()
}