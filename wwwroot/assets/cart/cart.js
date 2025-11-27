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

    let couponCodeElement = document.getElementById("coupon_input")
    let notesElement = document.getElementById("notes")
    couponCodeElement.classList.remove(isInvalid)
    notesElement.classList.remove(isInvalid)

    let formData = new FormData()
    formData.append("Notes", notesElement.value)
    formData.append("CouponCode", couponCodeElement.value)
    formData.append("AddressId", address_id)

    try {
        await fetch('/order/start', {
            method: 'POST',
            body: formData,
        })

        window.location.pathname = "/orders"
    } catch {}
}




async function applyCouponCode() {
    let couponCodeElement = document.getElementById("coupon_input")
    const code = couponCodeElement.value
    if (code.length == 0) {
        couponCodeElement.classList.add(isInvalid)
        return;
    }

    couponCodeElement.classList.remove(isInvalid)

    let applyButton = document.getElementById("apply")
    const originText = applyButton.innerHTML
    applyButton.disabled = true
    applyButton.innerHTML = '<i class="bi bi-check2"></i> Applying...'

    let formData = new FormData()
    formData.append("CouponCode", code)
    formData.append("Total", document.getElementById("total").value)

    try {
        const res = await fetch(`/coupon/validate`, {
            method: "POST",
            body: formData,
        })

        if (res.ok) {
            const data = await res.json()
            
            // Update input styling to show success
            couponCodeElement.classList.remove("is-invalid")
            couponCodeElement.classList.add("is-valid")
            
            // Update any discount/total display elements
            // Assuming you have elements with these IDs - adjust as needed
            if (data.CouponDiscount) {
                const discountElement = document.getElementById("discount")
                if (discountElement) {
                    discountElement.textContent = `$${data.CouponDiscount.toFixed(2)}`
                }
            }
            
            if (data.CouponTotal) {
                const finalTotalElement = document.getElementById("finalTotal")
                if (finalTotalElement) {
                    finalTotalElement.textContent = `$${data.CouponTotal.toFixed(2)}`
                }
            }
            
            // Re-enable button with success state
            applyButton.disabled = false
            applyButton.innerHTML = originText
            return
        }
        
    } catch {}

    // On error or invalid coupon
    couponCodeElement.classList.remove("is-valid")
    couponCodeElement.classList.add("is-invalid")
    applyButton.disabled = false
    applyButton.innerHTML = originText
}