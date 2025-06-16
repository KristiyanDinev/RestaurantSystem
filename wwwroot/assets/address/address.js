function goToEditAddress(addressId) {
    window.location.pathname = `/address/update/${addressId}`;
}

function goToAddresses() {
    window.location.pathname = `/addresses`;
}
function goToAddAddress() {
    window.location.pathname = `/address/add`;
}

async function deleteAddress(addressId) {
    if (!confirm('Are you sure you want to delete this address?')) {
        return;
    }

    try {
        const response = await fetch(`/address/${addressId}`, {
            method: 'POST'
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        goToAddresses();

    } catch (error) {
        document.getElementById('status').innerHTML = 'Error while deleting address.';
    }
}

async function updateAddress(addressId) {
    if (!confirm('Are you sure you want to update this address?')) {
        return;
    }

    let country = document.getElementById('country').value;
    let address = document.getElementById('address').value;
    let postalCode = document.getElementById('postalCode').value;
    let phoneNumber = document.getElementById('phoneNumber').value;

    let status = document.getElementById('status');

    if (!country || !address || !postalCode || !phoneNumber) {
        status.innerHTML = 'Please fill in all required fields.';
        return;
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
        const response = await fetch(`/address/update`, {
            method: 'POST',
            body: formData
        });

        if (!response.ok) {
            throw new Error();
        }

        goToAddresses()

    } catch {
        status.innerHTML = 'Error while updating address.';
    }
}

async function addAddress() {
    let country = document.getElementById('country').value; 
    let address = document.getElementById('address').value;
    let postalCode = document.getElementById('postalCode').value;
    let phoneNumber = document.getElementById('phoneNumber').value;

    if (!country || !address || !postalCode || !phoneNumber) {
        document.getElementById('status').innerHTML = 'Please fill in all required fields.';
        return;
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
        const response = await fetch(`/address/add`, {
            method: 'POST',
            body: formData
        });

        if (!response.ok) {
            throw new Error();
        }

        goToAddresses()
    } catch {
        document.getElementById('status').innerHTML = 'Error while adding address.';
    }
}