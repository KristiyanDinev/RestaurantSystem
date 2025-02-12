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

var ImageFile = ""
let fileSelection = document.getElementById("_image")
fileSelection.addEventListener('change', async function (e) {
    let targetFile = e.target.files[0]
    ImageFile = ""
    if (targetFile) {

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
        document.getElementsByClassName('review')[0].src = ""

    } else {
        delete_image_data = "no"
        doc.innerHTML = "Delete Image: No"
    }
}

async function updateUser() {
    let status = document.getElementById('Stats')
    status.innerHTML = "Updating..."

    let formData = new FormData()
    formData.append('username', document.getElementById('username').value)
    formData.append('email', document.getElementById('email').value)
    formData.append('address', document.getElementById('address').value)
    formData.append('notes', document.getElementById('notes').value)
    formData.append('phone', document.getElementById('phone').value)
    formData.append('image', ImageFile)
    formData.append('delete_image', delete_image_data)

    const res = await fetch(Host + '/profile/edit', {
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