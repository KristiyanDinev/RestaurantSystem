var invalidClass = "is-invalid"

async function submit() {
    let passwordElement = document.getElementById("Password")
    let emailElement = document.getElementById("Email")

    let password = passwordElement.value
    let email = emailElement.value
    let statsElement = document.getElementById("Stats")

    // Clear previous styling
    statsElement.className = "alert d-none"
    emailElement.classList.remove(invalidClass)
    passwordElement.classList.remove(invalidClass)

    if (!password || !email) {
        statsElement.innerHTML = '<i class="bi bi-exclamation-triangle-fill me-2"></i>Please fill in all fields correctly'
        statsElement.className = "alert alert-warning"

        // Add visual feedback to empty fields
        if (!email) emailElement.classList.add(invalidClass)
        if (!password) passwordElement.classList.add(invalidClass)

        return;
    }

    // Show loading state
    const submitButton = document.querySelector('button[onclick="submit()"]')
    const originalText = submitButton.innerHTML
    submitButton.innerHTML = `
    <div class="spinner-border" role="status">
    </div>
    Signing in...`
    submitButton.disabled = true

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
            emailElement.classList.remove(invalidClass)
            passwordElement.classList.remove(invalidClass)

            statsElement.innerHTML = ""
            statsElement.className = ""
            window.location.reload()
            return
        }

    } catch (error) {
        console.error('Login error:', error)
    }

    // Reset button state
    submitButton.innerHTML = originalText
    submitButton.disabled = false

    // Show error message
    statsElement.innerHTML = '<i class="bi bi-x-circle-fill me-2"></i>Invalid email or password. Please try again.'
    statsElement.className = "alert alert-danger"

    // Add visual feedback to input fields
    emailElement.classList.add(invalidClass)
    passwordElement.classList.add(invalidClass)

    // Clear password field for security
    passwordElement.value = ""
}