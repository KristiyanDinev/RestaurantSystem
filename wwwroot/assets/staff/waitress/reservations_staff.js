async function changeStatus(id, status) {

    let formData = new FormData()
    formData.append('Id', Number(id))
    formData.append('Status', status)

    try {
        const res = await fetch("/staff/reservations", {
            method: 'POST',
            body: formData,
            redirect: 'follow'
        })

        if (res.status == 200) {
            window.location.reload()
            return
        }

        document.getElementById('error,' + id).innerHTML = "Couldn't Update Reservation."

    } catch {
        document.getElementById('error,' + id).innerHTML = "Couldn't Update Reservation."
        return;
    }
}

async function deleteReservation(id) {

    if (!confirm("Are you sure you want to delete this reservation?")) {
        return;
    }

    try {
        const res = await fetch(`/staff/reservations/delete/${id}`, {
            method: 'POST'
        })

        if (res.ok) {
            window.location.reload()
            return
        }

    } catch {}

    document.getElementById('error,' + id).innerHTML = "Couldn't Delete the Reservation."
}
