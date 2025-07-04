async function submit() {
    let at_date = document.getElementById('at_date').value
    let at_time = document.getElementById('at_time').value

    let notes = document.getElementById('notes').value
    let amount_of_children = document.getElementById('children').value
    let amount_of_adults = document.getElementById('adults').value
    let phone = document.getElementById('phone').value

    if (at_date.length == 0 || at_time.length == 0 || Number(amount_of_children) < 0 ||
        Number(amount_of_adults) <= 0 || !phone) {
        document.getElementById('reservation_status').innerHTML = "Invalid inputs. Specify positive numbers. Make sure you have at least 1 adult."
        return;
    }

    let formData = new FormData()
    formData.append('Notes', notes)
    formData.append('PhoneNumber', phone)
    formData.append('Amount_Of_Children', Number(amount_of_children))
    formData.append('Amount_Of_Adults', Number(amount_of_adults))
    formData.append('At_Date', String(at_date + 'T'+at_time))

    try {
        const res = await fetch("/reservation", {
            method: 'POST',
            body: formData
        })

        if (res.ok) {
            window.location.pathname = "/reservations"
            return
        }

        document.getElementById('reservation_status').innerHTML = "Couldn't place the reservation."

    } catch {
        document.getElementById('reservation_status').innerHTML = "Couldn't place the reservation."
        return;
    }
}

async function cancelReservation(id) {

    if (!confirm("Are you sure you want to cancel this reservation?")) {
        return;
    }

    try {
        const res = await fetch(`/reservation/cancel/${id}`, {
            method: 'POST'
        })

        if (res.ok) {
            window.location.pathname = "/reservations"
            return
        }


    } catch {
    }

    document.getElementById('error,' + id).innerHTML = "Couldn't cancel the reservation."
}
