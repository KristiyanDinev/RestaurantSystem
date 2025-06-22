async function submit() {
    if (!confirm("Are you sure you want to place this order?")) {
        return
    }

    const table = document.getElementById('table').value
    if (table === "") {
        document.getElementById('error').innerHTML = "Please specify a table number"
        return
    }

    let formData = new FormData()
    formData.append('Notes', document.getElementById('notes').value)
    formData.append('CuponCode', document.getElementById('cupon').value)
    formData.append('TableNumber', table)

    try {
        const res = await fetch("/staff/orders/create", {
            method: "POST",
            body: formData
        })

        if (!res.ok) {
            document.getElementById('error').innerHTML = "Can't place this order"
            return
        }

        window.location.pathname = "/staff/orders"

    } catch {
        document.getElementById('error').innerHTML = "An error occurred while processing the order"
    }
}