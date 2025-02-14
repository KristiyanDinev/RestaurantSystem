var Host = "http://127.0.0.1:7278"

function Logout() {

    fetch(Host + "/logout", {
        method: "POST",
        redirect: 'follow',

    }).then((res) => {
        if (res.status === 200) {
            goToLogin()
        }
    })
}


function goToProfile() {
    window.location.href = Host + "/profile"
}



function goToRegister() {
    window.location.href = Host + '/register'
}


function goToLogin() {
    window.location.href = Host + '/login'
}

function goToDishes() {
    window.location.href = Host + '/dishes'
}

function goToCart() {
    window.location.href = Host + '/cart'
}

