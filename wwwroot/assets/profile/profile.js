function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            document.getElementById('preview').src = e.target.result;
        }

        reader.readAsDataURL(input.files[0]);
    }
}

document.getElementById("Image").onchange = (e) => {
    readURL(e.target);
};


function removeImage() {
    document.getElementById('Image').value = ""
    document.getElementById('preview').src = ''
}


async function updateUser() {
    if (!confirm('Are you sure you want to update your profile?')) {
        return
    }

    let status = document.getElementById('Stats')
    status.innerHTML = "Updating..."

    let username = document.getElementById('username').value
    let email = document.getElementById('email').value

    if (!username || !email) {
        status.innerHTML = "Please fill all fields with valid data"
        return
    }

    let formData = new FormData()
    formData.append('Name', username)
    formData.append('Email', email)
    formData.append('Image', document.getElementById("Image").files[0])
    formData.append('DeleteImage', document.getElementById("deleteimage").checked)

    try {
        const res = await fetch('/profile/update', {
            method: 'POST',
            body: formData
        })

        if (res.ok) {
            window.location.reload()

        } else {
            status.innerHTML = "Failed to Update"
        }

    } catch {
        status.innerHTML = "Error while updating profile"
    }
}