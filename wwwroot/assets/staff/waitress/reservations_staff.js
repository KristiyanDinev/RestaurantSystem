async function changeStatus(id, status) {
    let formData = new FormData()
    formData.append('Id', Number(id))
    formData.append('Status', status)

    try {
        await fetch("/staff/reservations", {
            method: 'POST',
            body: formData,
            redirect: 'follow'
        })

        window.location.reload()
    } catch {}
}

async function deleteReservation(id) {
    if (!confirm("Are you sure you want to delete this reservation?")) {
        return;
    }

    try {
        await fetch(`/staff/reservations/delete/${id}`, {
            method: 'POST'
        })

        window.location.reload()

    } catch {}
}

function togglePriceInput() {
    toggleElement('submit_price')
    toggleElement('new_price_input')
}

async function submitPrice(id) {
    if (!confirm("Are you sure you want to set a new price?")) {
        return;
    }

    let priceInput = document.getElementById('set_total_price')
    let newPrice = priceInput.value

    if (newPrice < 0) {
        alert("Please enter a valid price.")
        return
    }

    let formData = new FormData()
    formData.append('Id', id)
    formData.append('TotalPrice', newPrice)

    try {
        await fetch("/staff/reservations/setprice", {
            method: 'POST',
            body: formData
        })

        window.location.reload()
    } catch {}
}


