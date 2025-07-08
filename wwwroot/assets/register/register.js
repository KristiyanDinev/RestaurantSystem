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

async function submit() {
    let username = document.getElementById("Username").value;
    let password = document.getElementById("Password").value;
    let email = document.getElementById("Email").value;
    const stats = document.getElementById("Stats");

    if (!username || !password || !email) {
        stats.className = "alert alert-warning";
        stats.innerHTML = `<i class="bi bi-exclamation-triangle-fill me-2"></i>Please fill all fields with valid data`;
        return;
    }

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
            window.location.reload();
            return;
        }

        stats.className = "alert alert-danger";
        stats.innerHTML = `<i class="bi bi-exclamation-triangle-fill me-2"></i>Invalid register`;

    } catch (err) {
        stats.className = "alert alert-danger";
        stats.innerHTML = `<i class="bi bi-exclamation-triangle-fill me-2"></i>An error occurred during registration`;
    }
}
