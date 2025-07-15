function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            const preview = document.getElementById('preview');
            preview.src = e.target.result;
            preview.classList.remove('d-none');
        };
        reader.readAsDataURL(input.files[0]);
    }
}

document.getElementById('Image').onchange = (e) => {
    readURL(e.target);
};

function removeImage() {
    document.getElementById('Image').value = "";
    const preview = document.getElementById('preview');
    preview.src = "";
    preview.classList.add('d-none');
}

var invalidClass = "is-invalid"

async function submit() {
    let usernameElement = document.getElementById("Username")
    let passwordElement = document.getElementById("Password")
    let emailElement = document.getElementById("Email")

    let username = usernameElement.value;
    let password = passwordElement.value;
    let email = emailElement.value;

    emailElement.classList.remove(invalidClass)
    passwordElement.classList.remove(invalidClass)
    usernameElement.classList.remove(invalidClass)

    if (!email) {
        emailElement.classList.add(invalidClass)
        return
    }
    if (!password) {
        passwordElement.classList.add(invalidClass)
        return
    } 
    if (!username) {
        usernameElement.classList.add(invalidClass)
        return
    }


    const submitButton = document.querySelector('button[onclick="submit()"]')
    submitButton.innerHTML = '<i class="bi bi-arrow-clockwise me-2"></i>Signing up...'
    submitButton.disabled = true

    let formData = new FormData();
    formData.append("Name", username);
    formData.append("Password", password);
    formData.append("Email", email);
    formData.append("Image", document.getElementById('Image').files[0]);
    formData.append("RememberMe", document.getElementById('RememberMe').checked);

    try {
        await fetch("/register", {
            method: "POST",
            body: formData,
        });

        usernameElement.classList.remove(invalidClass)
        emailElement.classList.remove(invalidClass)
        passwordElement.classList.remove(invalidClass)
        window.location.reload();

    } catch {}
}
