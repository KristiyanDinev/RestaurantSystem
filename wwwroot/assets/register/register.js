

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            document.getElementById('preview').src = e.target.result
        }

        reader.readAsDataURL(input.files[0]);
    }
}

document.getElementById('Image').onchange = (e) => {
    readURL(e.target);
}

function removeImage() {
    document.getElementById('Image').value = ""
    document.getElementById('preview').src = ""
}

async function submit() {
    let username = document.getElementById("Username").value
    let password = document.getElementById("Password").value
    let email = document.getElementById("Email").value
    document.getElementById("Stats").innerHTML = ""

    if (!username || !password || !email) {
        document.getElementById("Stats").innerHTML = "Please fill all fields with valid data"
        return
    }

    let formData = new FormData()
    formData.append("Name", username)
    formData.append("Password", password)
    formData.append("Email", email)
    formData.append("Image", document.getElementById('Image').files[0])
    formData.append("RememberMe", document.getElementById('RememberMe').checked)

    try {
        const res = await fetch("/register", {
            method: "POST",
            body: formData,
        })

        if (res.ok) {
            window.location.reload()
            return
        }

        document.getElementById("Stats").innerHTML = "Invalid register"

    } catch (err) {
        document.getElementById("Stats").innerHTML = "An error occurred during registration"
    }
}
