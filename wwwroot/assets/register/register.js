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
    let stats = document.getElementById("Stats");

    statsElement.className = "alert d-none"
    emailElement.classList.remove(invalidClass)
    passwordElement.classList.remove(invalidClass)
    usernameElement.classList.remove(invalidClass)

    if (!username || !password || !email) {
        stats.className = "alert alert-warning";
        stats.innerHTML = `<i class="bi bi-exclamation-triangle-fill me-2"></i>Please fill all fields correctly`;

        if (!email) emailElement.classList.add(invalidClass)
        if (!password) passwordElement.classList.add(invalidClass)
        if (!username) usernameElement.classList.add(invalidClass)
        return;
    }

    const submitButton = document.querySelector('button[onclick="submit()"]')
    const originalText = submitButton.innerHTML
    submitButton.innerHTML = '<i class="bi bi-arrow-clockwise me-2"></i>Signing up...'
    submitButton.disabled = true

    let formData = new FormData();
    formData.append("Name", username);
    formData.append("Password", password);
    formData.append("Email", email);
    formData.append("Image", document.getElementById('Image').files[0]);
    formData.append("RememberMe", document.getElementById('RememberMe').checked);

    try {
        const res = await fetch("/register", {
            method: "POST",
            body: formData,
        });

        if (res.ok) {
            usernameElement.classList.remove(invalidClass)
            emailElement.classList.remove(invalidClass)
            passwordElement.classList.remove(invalidClass)
            stats.innerHTML = ""
            stats.className = ""
            window.location.reload();
            return;
        }

    } catch (err) {
        console.error('Register error:', err)
    }
    // Reset button state
    submitButton.innerHTML = originalText
    submitButton.disabled = false

    // Show error message
    stats.className = "alert alert-danger";
    stats.innerHTML = `<i class="bi bi-x-circle-fill me-2"></i>Invalid register`;

    // Add visual feedback to input fields
    usernameElement.classList.add(invalidClass)
    emailElement.classList.add(invalidClass)
    passwordElement.classList.add(invalidClass)

    // Clear password field for security
    passwordElement.value = ""
}
