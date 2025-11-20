var isInvalid = 'is-invalid'

async function submit() {
    if (getCookie(cart_header).length == 0) {
        alert("You have not selected any dishes to order as of now.")
        window.location.pathname = "/staff/orders/dishes"
        return;
    }

    if (!confirm("Are you sure you want to place this order?")) {
        return
    }

    let tableElement = document.getElementById('table')
    tableElement.classList.remove(isInvalid)

    const table = tableElement.value
    if (!table) {
        tableElement.classList.add(isInvalid)
        return
    }

    let couponElement = document.getElementById('coupon_input')
    let notesElement = document.getElementById('notes')
    couponElement.classList.remove(isInvalid)
    notesElement.classList.remove(isInvalid)

    let formData = new FormData()
    formData.append('Notes', notesElement.value)
    formData.append('CouponCode', couponElement.value)
    formData.append('TableNumber', table)

    try {
        await fetch("/staff/orders/details", {
            method: "POST",
            body: formData
        })
    } catch {}

    window.location.pathname = "/staff/orders"
}