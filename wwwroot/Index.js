var Host = "http://127.0.0.1:7278"
var __scheme = document.location.protocol === "https:" ? "wss" : "ws";
var __port = document.location.port ? (":" + document.location.port) : "";

var __connectionUrl = __scheme + "://" + document.location.hostname + __port ;

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


function UseWebSockets(url, onopen, onclose, onerror, onmessage) {
    var socket = new WebSocket(url)

    // socket's  state: socket.readyState
    // WebSocket.CLOSED | WebSocket.CLOSING | WebSocket.CONNECTING | WebSocket.OPEN

    // the functions has `event` paremeter
    socket.onopen = onopen
    socket.onclose = onclose
    socket.onerror = onerror

    // event.data -> retuns the message
    socket.onmessage = onmessage

    return socket
}

function startOrderWebSocket(onopen, onclose, onerror, onmessage) {
    return UseWebSockets(__connectionUrl + "/ws/orders", onopen, onclose, onerror, onmessage)
}

function startReservationWebSocket(onopen, onclose, onerror, onmessage) {
    return UseWebSockets(__connectionUrl + "/ws/reservations", onopen, onclose, onerror, onmessage)
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

