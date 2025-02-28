var restorantAddressIndexes = {}

function addRestorantIndex(index, addr, city, state, country) {
    restorantAddressIndexes[index] = [addr, city, state, country]
}

async function startOrder() {
    let restorantIndex = document.getElementById("restorant_address").value;
    if (restorantIndex.length == 0) {
        alert("You can't order without restorant address.")
        return;
    }

    let cartCookie = getCookie('cart')
    if (cartCookie.length == 0) {
        alert("You don't have any dishes to order as of now.")
        return;
    }

    var formData = new FormData()
    formData.append("notes", document.getElementById("notes").value)
    formData.append("cuponCode", document.getElementById("cupon_input").value)
    formData.append("restorantAddress", restorantAddressIndexes[restorantIndex][0])
    formData.append("restorantCity", restorantAddressIndexes[restorantIndex][1])
    formData.append("restorantState", restorantAddressIndexes[restorantIndex][2])
    formData.append("restorantCountry", restorantAddressIndexes[restorantIndex][3])

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