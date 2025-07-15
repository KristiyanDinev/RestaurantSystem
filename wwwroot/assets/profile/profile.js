function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            let preview = document.getElementById('preview');
            preview.src = e.target.result;
            preview.classList.remove('d-none');
        }

        reader.readAsDataURL(input.files[0]);
    }
}

document.getElementById("Image").onchange = (e) => {
    readURL(e.target);
};


function removeImage() {
    document.getElementById('Image').value = ""
    let preview = document.getElementById('preview');
    preview.src = ''
    preview.classList.add('d-none');
}

var isInvalid = 'is-invalid'
async function updateUser() {
    if (!confirm('Are you sure you want to update your profile?')) {
        return
    }

    let usernameElement = document.getElementById('username')
    let emailElement = document.getElementById('email')
    let username = usernameElement.value
    let email = emailElement.value

    usernameElement.classList.remove(isInvalid)
    emailElement.classList.remove(isInvalid)

    if (!username) {
        usernameElement.classList.add(isInvalid)
        return
    }

    if (!email) {
        emailElement.classList.add(isInvalid)
        return
    }

    let updateButton = document.getElementById('update')
    updateButton.disabled = true
    updateButton.innerHTML = '<i class="bi bi - check - circle me - 2"></i> Updating Profile...'

    let formData = new FormData()
    formData.append('Name', username)
    formData.append('Email', email)
    formData.append('Image', document.getElementById("Image").files[0])
    formData.append('DeleteImage', document.getElementById("deleteimage").checked)

    try {
        await fetch('/profile/update', {
            method: 'POST',
            body: formData
        })
        usernameElement.classList.remove(isInvalid)
        emailElement.classList.remove(isInvalid)
        window.location.reload()
    } catch { }
}