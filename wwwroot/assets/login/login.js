var invalidClass = "is-invalid"

async function submit() {
    let passwordElement = document.getElementById("Password")
    let emailElement = document.getElementById("Email")

    let password = passwordElement.value
    let email = emailElement.value

    emailElement.classList.remove(invalidClass)
    passwordElement.classList.remove(invalidClass)

    if (!email) {
        emailElement.classList.add(invalidClass)
        return
    }
    if (!password) {
        passwordElement.classList.add(invalidClass)
        return
    }


    // Show loading state
    const submitButton = document.querySelector('button[onclick="submit()"]')
    submitButton.innerHTML = '<i class="bi bi-arrow-clockwise me-2"></i>Signing in...'
    submitButton.disabled = true

    let formData = new FormData()
    formData.append("Password", password)
    formData.append("Email", email)
    formData.append("RememberMe", document.getElementById('RememberMe').checked)

    try {
        await fetch("/login", {
            method: "POST",
            body: formData,
        })

        emailElement.classList.remove(invalidClass)
        passwordElement.classList.remove(invalidClass)
        window.location.reload()

    } catch {}
}