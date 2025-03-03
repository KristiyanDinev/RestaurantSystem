

async function submitCreation() {
    let at_date = document.getElementById('at_date').value
    let at_time = document.getElementById('at_time').value

    let notes = document.getElementById('notes').value
    let amount_of_children = document.getElementById('children').value
    let amount_of_adults = document.getElementById('adults').value

    let restorant_id = document.getElementById('restorant_address').value

    let formData = new FormData()
    formData.append('Notes', notes)
    formData.append('Amount_Of_Children', Number(amount_of_children))
    formData.append('Amount_Of_Adults', Number(amount_of_adults))
    formData.append('RestorantId', Number(restorant_id))
    // At_Date

    const res = await fetch(getDataFromLocalStorage("Host")  + "/reservations/create", {
        method: 'POST',
        body: formData,
        redirect: 'follow',
    })

    if (res.status === 200) {
        alert("Created the reservation")
        window.location.reload()

    } else {
        alert("Didn't create the reservation")
    }
}

async function cancelReservation(id) {
    let formData = new FormData()
    formData.append('reservationIdStr', String(id))

    const res = await fetch(getDataFromLocalStorage("Host")  + "/reservations/delete", {
        method: 'POST',
        body: formData,
        redirect: 'follow',
    })

    if (res.status === 200) {
        alert("Cancelled the reservation")
        window.location.reload()

    } else {
        alert("Didn't cancel the reservation")
    }
}

