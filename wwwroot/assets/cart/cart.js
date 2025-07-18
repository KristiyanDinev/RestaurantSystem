var isInvalid = 'is-invalid'
async function startOrder() {
    if (getCookie(cart_header).length == 0) {
        alert("You don't have any dishes to order as of now.")
        window.location.pathname = "/dishes"
        return;
    }

    let addressElement = document.getElementById("address")
    addressElement.classList.remove(isInvalid)

    let address_id = addressElement.value
    if (!address_id) {
        addressElement.classList.add(isInvalid)
        return;
    }

    let cuponCodeElement = document.getElementById("cupon_input")
    let notesElement = document.getElementById("notes")
    cuponCodeElement.classList.remove(isInvalid)
    notesElement.classList.remove(isInvalid)

    let formData = new FormData()
    formData.append("Notes", notesElement.value)
    formData.append("CuponCode", cuponCodeElement.value)
    formData.append("AddressId", address_id)

    try {
        await fetch('/order/start', {
            method: 'POST',
            body: formData,
        })

        window.location.pathname = "/orders"
    } catch {}
}




async function applyCuponCode() {
    let cuponCodeElement = document.getElementById("cupon_input")
    const code = cuponCodeElement.value
    if (code.length == 0) {
        cuponCodeElement.classList.add(isInvalid)
        return;
    }

    cuponCodeElement.classList.remove(isInvalid)

    let applyButton = document.getElementById("apply")
    const originText = applyButton.innerHTML
    applyButton.disabled = true
    applyButton.innerHTML = '<i class="bi bi-check2"></i> Applying...'

    let formData = new FormData()
    formData.append("CuponCode", code)
    formData.append("Total", document.getElementById("total").value)

    try {
        const res = await fetch(`/cupon/validate`, {
            method: "POST",
            body: formData,
        })

        if (res.ok) {
            window.location.reload()
            return
        }
        
    } catch {}

    cuponCodeElement.classList.add(isInvalid)
    applyButton.disabled = false
    applyButton.innerHTML = originText
}