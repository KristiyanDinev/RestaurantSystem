

async function submit() {
    let password = document.getElementById("Password").value
    let email = document.getElementById("Email").value

    if (!password || !email) {
        document.getElementById("Stats").innerHTML = "Fill all Inputs correctly."
        return;
    }

    let formData = new FormData()
    formData.append("Password", password)
    formData.append("Email", email)
    formData.append("RememberMe", document.getElementById('RememberMe').checked)

    try {
        const res = await fetch("/login", {
            method: "POST",
            body: formData,
        })

        if (res.ok) {
            window.location.reload()
            return
        }

    } catch {
    }

    document.getElementById("Stats").innerHTML = "Invalid login"
}