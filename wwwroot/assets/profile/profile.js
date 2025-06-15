function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#review').attr('src', e.target.result);
        }

        reader.readAsDataURL(input.files[0]);
    }
}

document.getElementById("_image").onchange = (e) => {
    readURL(e.target);
};


var ImageFile = ""
let fileSelection = document.getElementById("_image")
fileSelection.addEventListener('change', async function (e) {
    let targetFile = e.target.files[0]
    ImageFile = ""
    if (targetFile) {
        readURL(e.target);

        ImageFile += targetFile.name + ';'

        var fileReaderForImage = new FileReader()
        fileReaderForImage.onload = function () {

            let array1 = new Uint8Array(fileReaderForImage.result)
            ImageFile += btoa(array1)

        }
        fileReaderForImage.readAsArrayBuffer(targetFile)
    }
})

var delete_image_data = "no"

function toggleDeleteImage() {
    let doc = document.getElementById('delete_image_status')
    if (delete_image_data === "no") {
        delete_image_data = "yes"
        doc.innerHTML = "Delete Image: Yes"
        document.getElementById("review").src = ""

    } else {
        delete_image_data = "no"
        doc.innerHTML = "Delete Image: No"
    }
}

async function updateUser() {
    let status = document.getElementById('Stats')
    status.innerHTML = "Updating..."

    let addr = document.getElementById('address').value
    let city = document.getElementById('city').value
    let state = document.getElementById('state').value
    let country = document.getElementById('country').value
    let username = document.getElementById('username').value
    let email = document.getElementById('email').value
    let postal_code = document.getElementById('postal_code').value
    let phoneNumber = document.getElementById('phone').value

    if (country && country == '')

    if (!addr || !city || ) {

    }

    let status = document.getElementById('Stats')
    status.innerHTML = "Updating..."

    let formData = new FormData()
    formData.append('Username', username)
    formData.append('Email', email)
    formData.append('Address', addr)
    formData.append('City', city)
    formData.append('State', state)
    formData.append('Country', country)
    formData.append('Notes', document.getElementById('notes').value)
    formData.append('PhoneNumber', phoneNumber)
    formData.append('PostalCode', postal_code)
    formData.append('Image', document.getElementById("_image").value)
    formData.append('DeleteImage', delete_image_data)

    const res = await fetch(getDataFromLocalStorage("Host") + '/profile/edit', {
        method: 'POST',
        body: formData,
        redirect: 'follow',
    })


    if (res.status == 200) {
        status.innerHTML = "Updated Successfully"

    } else {
        status.innerHTML = "Failed to Update"
    }
}