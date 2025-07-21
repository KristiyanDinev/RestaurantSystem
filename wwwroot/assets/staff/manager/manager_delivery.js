

async function cancelDelivery(orderId) {
    if (!confirm('Are you sure you want to cancel this delivery?')) return;

    try {
        await fetch(`/staff/manager/deliveries/cancel/${orderId}`, {
            method: 'POST'
        })

        window.location.reload()
    } catch { }
}


async function deleteDelivery(orderId) {
    if (!confirm('Are you sure you want to delete this order and delivery?')) return;

    try {
        await fetch(`/staff/manager/deliveries/delete/${orderId}`, {
            method: 'POST'
        })

        window.location.reload()
    } catch { }
}