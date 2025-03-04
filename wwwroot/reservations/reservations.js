

async function submitCreation() {
    let at_date = document.getElementById('at_date').value
    let at_time = document.getElementById('at_time').value

    let notes = document.getElementById('notes').value
    let amount_of_children = document.getElementById('children').value
    let amount_of_adults = document.getElementById('adults').value

    let restorant_id = document.getElementById('restorant').value

    if (at_date.length == 0 || at_time.length == 0 || Number(amount_of_children) < 0 ||
        Number(amount_of_adults) < 0 || restorant_id.length == 0) {
            document.getElementById('reservation_status').innerHTML = "Some invalid inputs."
            return;
    }

    let formData = new FormData()
    formData.append('Notes', notes)
    formData.append('Amount_Of_Children', Number(amount_of_children))
    formData.append('Amount_Of_Adults', Number(amount_of_adults))
    formData.append('RestorantId', Number(restorant_id))
    formData.append('At_Date', String(at_date + ' '+at_time))

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
        document.getElementById('reservation_status').innerHTML = "Didn't create the reservation."
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


var socket = null
var regiseredReservations = [];

function onopen() {
    console.log('Connected')
    socket.send("subscribtion_ids;"+regiseredReservations.join(';'))
}

function onclose() {
    console.log('Closed')
}

function onmessage(event) {
    const data = event.data
    if (data.length == 0) {
        return;
    }
    console.log('message: '+data)
}

function onerror(event) {
    console.log('error: '+event.data)
}

function startWebSocket() {
    socket = startOrderWebSocket(onopen, onclose, onerror, onmessage)
}

