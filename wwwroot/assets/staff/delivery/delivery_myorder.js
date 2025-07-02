async function delivered() {
    if (!confirm("Are you sure you want to mark this order as delivered?")) return;

    try {
        const response = await fetch(`/staff/delivery/delivered`, {
            method: 'POST',
        })

        if (response.ok) {
            window.location.pathname = "/staff/delivery/orders";
            return;
        }
    } catch {}
}


async function canceled() {
    if (!confirm("Are you sure you want to cancel this order?")) return;

    try {
        const response = await fetch(`/staff/delivery/cancel`, {
            method: 'POST',
        })

        if (response.ok) {
            window.location.pathname = "/staff/delivery/orders";
            return;
        }
    } catch { }
}