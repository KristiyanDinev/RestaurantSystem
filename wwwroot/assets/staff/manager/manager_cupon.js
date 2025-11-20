function toggleCreateCoupon() {
    toggleElement('create_coupon')
}

function toggleEditCoupon(code) {
    toggleElement(`name,${code}`)
    toggleElement(`discount,${code}`)
    toggleElement(`expdate,${code}`)
    toggleElement(`editsub,${code}`)
}

let isInvalid = 'is-invalid'
async function submitCreate() {
    if (!confirm(`Are you sure you want to create this coupon?`)) return;

    let NameElement = document.getElementById(`Name`)
    let CodeElement = document.getElementById(`Code`)
    let DiscountElement = document.getElementById(`Discount`)
    let ExpirationDateElement = document.getElementById(`ExpirationDate`)

    NameElement.classList.remove(isInvalid)
    CodeElement.classList.remove(isInvalid)
    DiscountElement.classList.remove(isInvalid)
    ExpirationDateElement.classList.remove(isInvalid)

    const nameValue = NameElement.value
    const codeValue = CodeElement.value
    const discountValue = DiscountElement.value
    const expDateValue = ExpirationDateElement.value

    if (!nameValue) {
        NameElement.classList.add(isInvalid)
        return
    }
    if (!codeValue) {
        CodeElement.classList.add(isInvalid)
        return
    }
    if (!discountValue || discountValue > 100 || discountValue < 0) {
        DiscountElement.classList.add(isInvalid)
        return
    }
    if (!expDateValue) {
        ExpirationDateElement.classList.add(isInvalid)
        return
    }

    let formData = new FormData()
    formData.append('Name', nameValue)
    formData.append('CouponCode', codeValue)
    formData.append('DiscountPercent', Number(discountValue))
    formData.append('ExpDate', expDateValue)

    try {
        await fetch('/staff/manager/coupons/create', {
            method: 'POST',
            body: formData
        })

        window.location.reload()
    } catch {}
}

async function submitEdit(code) {
    if (!confirm(`Are you sure you want to edit ${code} coupon?`)) return;

    let NameElement = document.getElementById(`name,${code}`)
    let DiscountElement = document.getElementById(`discount,${code}`)
    let ExpirationDateElement = document.getElementById(`expdate,${code}`)

    NameElement.classList.remove(isInvalid)
    DiscountElement.classList.remove(isInvalid)
    ExpirationDateElement.classList.remove(isInvalid)

    const nameValue = NameElement.value
    const discountValue = DiscountElement.value
    const expDateValue = ExpirationDateElement.value

    if (!nameValue) {
        NameElement.classList.add(isInvalid)
        return
    }
    if (!discountValue || discountValue > 100 || discountValue < 0) {
        DiscountElement.classList.add(isInvalid)
        return
    }
    if (!expDateValue) {
        ExpirationDateElement.classList.add(isInvalid)
        return
    }

    let formData = new FormData()
    formData.append('Name', nameValue)
    formData.append('CouponCode', code)
    formData.append('DiscountPercent', Number(discountValue))
    formData.append('ExpDate', expDateValue)

    try {
        await fetch(`/staff/manager/coupons/edit`, {
            method: 'POST',
            body:formData
        })
        
        window.location.reload()
    } catch {}
}

async function submitDelete(code) {
    if (!confirm(`Are you sure you want to delete ${code} coupon?`)) return;

    try {
        await fetch(`/staff/manager/coupons/delete/${code}`, {
            method: 'POST'
        })

        window.location.reload()
    } catch {}
}
