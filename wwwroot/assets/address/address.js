var isInvalid = 'is-invalid'
async function deleteAddress(addressId) {
    if (!confirm('Are you sure you want to delete this address?')) {
        return;
    }

    try {
        await fetch(`/address/delete/${addressId}`, {
            method: 'POST'
        });

        window.location.reload()

    } catch {}
}

async function updateAddress(addressId) {
    if (!confirm('Are you sure you want to update this address?')) {
        return;
    }
    let countryElement = document.getElementById('country');
    let addressElement = document.getElementById('address');
    let postalCodeElement = document.getElementById('postalCode');
    let phoneNumberElement = document.getElementById('phoneNumber');

    countryElement.classList.remove(isInvalid);
    addressElement.classList.remove(isInvalid);
    postalCodeElement.classList.remove(isInvalid);
    phoneNumberElement.classList.remove(isInvalid);

    let country = countryElement.value;
    let address = addressElement.value;
    let postalCode = postalCodeElement.value;
    let phoneNumber = phoneNumberElement.value;

    if (!country) {
        countryElement.classList.add(isInvalid);
        return
    }

    if (!address) {
        addressElement.classList.add(isInvalid);
        return
    }

    if (!postalCode) {
        postalCodeElement.classList.add(isInvalid);
        return
    }

    if (!phoneNumber) {
        phoneNumberElement.classList.add(isInvalid);
        return
    }

    let formData = new FormData()
    formData.append('Id', addressId)
    formData.append('Country', country)
    formData.append('State', document.getElementById('state').value)
    formData.append('City', document.getElementById('city').value)
    formData.append('Address', address)
    formData.append('PhoneNumber', phoneNumber)
    formData.append('PostalCode', postalCode)
    formData.append('Notes', document.getElementById('notes').value)

    try {
        await fetch(`/address/update`, {
            method: 'POST',
            body: formData
        });
        window.location.pathname = '/addresses'

    } catch {}
}

async function addAddress() {
    let countryElement = document.getElementById('country');
    let addressElement = document.getElementById('address');
    let postalCodeElement = document.getElementById('postalCode');
    let phoneNumberElement = document.getElementById('phoneNumber');

    countryElement.classList.remove(isInvalid);
    addressElement.classList.remove(isInvalid);
    postalCodeElement.classList.remove(isInvalid);
    phoneNumberElement.classList.remove(isInvalid);

    let country = countryElement.value;
    let address = addressElement.value;
    let postalCode = postalCodeElement.value;
    let phoneNumber = phoneNumberElement.value;

    if (!country) {
        countryElement.classList.add(isInvalid);
        return
    }

    if (!address) {
        addressElement.classList.add(isInvalid);
        return
    }

    if (!postalCode) {
        postalCodeElement.classList.add(isInvalid);
        return
    }

    if (!phoneNumber) {
        phoneNumberElement.classList.add(isInvalid);
        return
    }

    let formData = new FormData()
    formData.append('Country', country)
    formData.append('State', document.getElementById('state').value)
    formData.append('City', document.getElementById('city').value)
    formData.append('Address', address)
    formData.append('PhoneNumber', phoneNumber)
    formData.append('PostalCode', postalCode)
    formData.append('Notes', document.getElementById('notes').value)

    try {
        await fetch(`/address/add`, {
            method: 'POST',
            body: formData
        });

        window.location.pathname = '/addresses'
        
    } catch {}
}