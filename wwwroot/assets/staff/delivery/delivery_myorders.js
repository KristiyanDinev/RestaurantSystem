async function delivered(id) {
    if (!confirm("Are you sure you want to mark this order as delivered?")) return;

    try {
        await fetch(`/staff/delivery/delivered/${id}`, {
            method: 'POST',
        })

        window.location.reload()
    } catch {}
}


async function canceled(id) {
    if (!confirm("Are you sure you want to cancel this order?")) return;

    try {
        await fetch(`/staff/delivery/cancel/${id}`, {
            method: 'POST',
        })

        window.location.reload()
    } catch { }
}