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
    
            couponCodeElement.classList.remove("is-invalid")
            couponCodeElement.classList.add("is-valid")
            
            const totalContainer = document.querySelector('.text-md-end')
            const subtotalElement = totalContainer.querySelector('.text-warning.fw-bold')
            
            let discountSection = totalContainer.querySelector('.discount-section')
            if (!discountSection) {
                discountSection = document.createElement('div')
                discountSection.className = 'mb-1 discount-section'
                subtotalElement.closest('.mb-1').after(discountSection)
            }
            
            discountSection.innerHTML = `
                <span class="text-muted">Discount (${data.discountPercentage}%):</span>
                <span class="text-success fw-bold">
                    -${data.discountAmount.toFixed(2)} lv.
                </span>
            `
            
            const totalSection = totalContainer.querySelector('.border-top.pt-2.mt-2')
            totalSection.innerHTML = `
                <span class="text-muted">Total:</span>
                <span class="text-warning fw-bold fs-4">${data.finalTotal.toFixed(2)} lv.</span>
            `
            
            applyButton.disabled = false
            applyButton.innerHTML = originText
            return
        }
        
    } catch {}

    // On error or invalid coupon
    couponCodeElement.classList.remove("is-valid")
    couponCodeElement.classList.add(isInvalid)
    
    const totalContainer = document.querySelector('.text-md-end')
    const discountSection = totalContainer.querySelector('.discount-section')
    if (discountSection) {
        discountSection.remove()
    }
    
    const subtotalElement = totalContainer.querySelector('.text-warning.fw-bold')
    const subtotal = parseFloat(subtotalElement.textContent.replace(' lv.', ''))
    
    const totalSection = totalContainer.querySelector('.border-top.pt-2.mt-2')
    totalSection.innerHTML = `
        <span class="text-muted">Total:</span>
        <span class="text-warning fw-bold fs-4">${subtotal.toFixed(2)} lv.</span>
    `
    
    applyButton.disabled = false
    applyButton.innerHTML = originText
}