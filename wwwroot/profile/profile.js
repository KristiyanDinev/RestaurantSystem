function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#review').attr('src', e.target.result);
        }

        reader.readAsDataURL(input.files[0]);
    }
}

$("#_image").change(function () {
    readURL(this);
});


function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            document.getElementById('review').src = e.target.result
        }

        reader.readAsDataURL(input.files[0]);
    }
}

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
    let addr = document.getElementById('address').value
    let city = document.getElementById('city').value
    let state = document.getElementById('state').value
    let country = document.getElementById('country').value

    let status = document.getElementById('Stats')
    status.innerHTML = "Updating..."

    let formData = new FormData()
    formData.append('Username', document.getElementById('username').value)
    formData.append('Email', document.getElementById('email').value)
    formData.append('Address', addr)
    formData.append('City', city)
    formData.append('State', state)
    formData.append('Country', country)
    formData.append('Notes', document.getElementById('notes').value)
    formData.append('PhoneNumber', document.getElementById('phone').value)
    formData.append('Image', ImageFile)
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