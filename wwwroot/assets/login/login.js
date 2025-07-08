async function submit() {
    let password = document.getElementById("Password").value
    let email = document.getElementById("Email").value
    let statsElement = document.getElementById("Stats")

    // Clear previous styling
    statsElement.className = "alert d-none"
    document.getElementById("Email").classList.remove("is-invalid")
    document.getElementById("Password").classList.remove("is-invalid")

    if (!password || !email) {
        statsElement.innerHTML = '<i class="bi bi-exclamation-triangle-fill me-2"></i>Please fill in all fields correctly.'
        statsElement.className = "alert alert-warning"

        // Add visual feedback to empty fields
        if (!email) document.getElementById("Email").classList.add("is-invalid")
        if (!password) document.getElementById("Password").classList.add("is-invalid")

        return;
    }

    // Show loading state
    const submitButton = document.querySelector('button[onclick="submit()"]')
    const originalText = submitButton.innerHTML
    submitButton.innerHTML = '<i class="bi bi-arrow-clockwise me-2" style="animation: spin 1s linear infinite;"></i>Signing in...'
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
    document.getElementById("Email").classList.add("is-invalid")
    document.getElementById("Password").classList.add("is-invalid")

    // Clear password field for security
    document.getElementById("Password").value = ""
}

// Add CSS for spinning animation
const style = document.createElement('style');
style.textContent = `
    @keyframes spin {
        0% { transform: rotate(0deg); }
        100% { transform: rotate(360deg); }
    }
`;
document.head.appendChild(style);