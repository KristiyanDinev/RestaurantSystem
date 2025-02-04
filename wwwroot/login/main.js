var Host = "https://localhost:7278"

async function submit() {
    let password = document.getElementById("Password")
    let email = document.getElementById("Email")

    if (password.value.replace(" ", "").length === 0 ||
        email.value.replace(" ", "").length === 0) {

        document.getElementById("Stats").innerHTML = "Fill all Inputs"
        return;
    }


    let formData = new FormData()
    formData.append("password", password.value)
    formData.append("email", email.value)
    formData.append("rememberMe", document.getElementById("RememberMe").value)

    const res = await fetch(Host + "/login", {
        method: "POST",
        body: formData,
        redirect: 'follow',
    })

    if (res.status === 200) {
        window.location.href = Host + "/dishes"
    }

}