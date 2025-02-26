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

function getCookie(cname) {
    let name = cname + "=";
    let ca = document.cookie.split(';');
    for(let i = 0; i < ca.length; i++) {
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

function goToCart() {
    window.location.href = Host + '/cart'
}

function goToOrders() {
    window.location.href = Host + '/orders'
}

