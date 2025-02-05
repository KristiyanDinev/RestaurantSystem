var Host = "https://localhost:7278"


function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
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