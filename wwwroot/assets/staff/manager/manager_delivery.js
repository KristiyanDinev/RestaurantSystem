

async function cancelDelivery(orderId) {
    if (!confirm('Are you sure you want to cancel this delivery?')) return;

    try {
        const res = await fetch(`/staff/manager/deliveries/cancel/${orderId}`, {
            method: 'POST'
        })

        if (res.ok) {
            window.location.reload()
            return
        }
    } catch { }
    document.getElementById(`status,${orderId}`).innerHTML = "Can't cancel this delivery"
}


async function deleteDelivery(orderId) {
    if (!confirm('Are you sure you want to delete this order and delivery?')) return;

    try {
        const res = await fetch(`/staff/manager/deliveries/delete/${orderId}`, {
            method: 'POST'
        })

        if (res.ok) {
            window.location.reload()
            return
        }
    } catch { }
    document.getElementById(`status,${orderId}`).innerHTML = "Can't delete this delivery"
}