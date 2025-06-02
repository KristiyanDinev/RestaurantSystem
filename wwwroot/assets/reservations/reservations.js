async function submit() {
    let at_date = document.getElementById('at_date').value
    let at_time = document.getElementById('at_time').value

    let notes = document.getElementById('notes').value
    let amount_of_children = document.getElementById('children').value
    let amount_of_adults = document.getElementById('adults').value

    if (at_date.length == 0 || at_time.length == 0 || Number(amount_of_children) < 0 ||
        Number(amount_of_adults) < 0) {
        document.getElementById('reservation_status').innerHTML = "Invalid inputs. Specify positive numbers."
        return;
    }

    let formData = new FormData()
    formData.append('Notes', notes)
    formData.append('Amount_Of_Children', Number(amount_of_children))
    formData.append('Amount_Of_Adults', Number(amount_of_adults))
    formData.append('At_Date', String(at_date + ' '+at_time))

    try {
        const res = await fetch(getDataFromLocalStorage("Host") + "/reservations/create", {
            method: 'POST',
            body: formData,
            redirect: 'follow',
        })

        if (res.status == 200) {
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

    if (!check("Are you sure you want to cancel this reservation?")) {
        return;
    }

    try {
        const res = await fetch(getDataFromLocalStorage("Host") +
            "/reservation/cancel/" + Number(id), {
            method: 'POST',
            redirect: 'follow'
        })

        if (res.status === 200) {
            window.location.pathname = "/reservations"
            return
        }

        document.getElementById('error,' + id).innerHTML = "Couldn't cancel the reservation."

    } catch {
        document.getElementById('error,' + id).innerHTML = "Couldn't cancel the reservation."
    }
}
