
let isInvalid = 'is-invalid'
async function submitEdit(id) {
    if (!confirm(`Are you sure you want to edit the restaurant you are working on?`)) return;

    let CountryElement = document.getElementById(`country`)
    let AddressElement = document.getElementById(`Address`)
    let PostalCodeElement = document.getElementById(`PostalCode`)
    let ReservationMaxAdultsElement = document.getElementById(`ReservationMaxAdults`)
    let ReservationMinAdultsElement = document.getElementById(`ReservationMinAdults`)
    let ReservationMaxChildrenElement = document.getElementById(`ReservationMaxChildren`)
    let ReservationMinChildrenElement = document.getElementById(`ReservationMinChildren`)

    CountryElement.classList.remove(isInvalid)
    AddressElement.classList.remove(isInvalid)
    PostalCodeElement.classList.remove(isInvalid)
    ReservationMaxAdultsElement.classList.remove(isInvalid)
    ReservationMinAdultsElement.classList.remove(isInvalid)
    ReservationMaxChildrenElement.classList.remove(isInvalid)
    ReservationMinChildrenElement.classList.remove(isInvalid)

    const countryValue = CountryElement.value
    const addressValue = AddressElement.value
    const postalCodeValue = PostalCodeElement.value
    const reservationMaxAdultsValue = ReservationMaxAdultsElement.value
    const reservationMinAdultsValue = ReservationMinAdultsElement.value
    const reservationMaxChildrenValue = ReservationMaxChildrenElement.value
    const reservationMinChildrenValue = ReservationMinChildrenElement.value

    if (!countryValue) {
        CountryElement.classList.add(isInvalid)
        return
    }
    if (!addressValue) {
        AddressElement.classList.add(isInvalid)
        return
    }
    if (!postalCodeValue) {
        PostalCodeElement.classList.add(isInvalid)
        return
    }
    if (!reservationMaxAdultsValue) {
        ReservationMaxAdultsElement.classList.add(isInvalid)
        return
    }
    if (!reservationMinAdultsValue) {
        ReservationMinAdultsElement.classList.add(isInvalid)
        return
    }
    if (!reservationMaxChildrenValue) {
        ReservationMaxChildrenElement.classList.add(isInvalid)
        return
    }
    if (!reservationMinChildrenValue) {
        ReservationMinChildrenElement.classList.add(isInvalid)
        return
    }

    let formData = new FormData()
    formData.append('Id', id)
    formData.append('Country', countryValue)
    formData.append('State', document.getElementById(`state`).value)
    formData.append('City', document.getElementById(`city`).value)
    formData.append('Address', addressValue)
    formData.append('PostalCode', postalCodeValue)
    formData.append('Email', document.getElementById(`Email`).value)
    formData.append('PhoneNumber', document.getElementById(`PhoneNumber`).value)
    formData.append('ReservationMaxAdults', reservationMaxAdultsValue)
    formData.append('ReservationMinAdults', reservationMinAdultsValue)
    formData.append('ReservationMaxChildren', reservationMaxChildrenValue)
    formData.append('ReservationMinChildren', reservationMinChildrenValue)
    formData.append('DoDelivery', document.getElementById(`DoDelivery`).checked)
    formData.append('ServeCustomersInPlace', document.getElementById(`ServeCustomersInPlace`).checked)

    try {
        await fetch(`/staff/manager/restaurant/edit`, {
            method: 'POST',
            body: formData
        })
        
        window.location.reload()
    } catch {}
}
