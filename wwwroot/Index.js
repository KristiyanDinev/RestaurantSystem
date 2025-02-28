

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

function setDataToLocalStorage(key, value) {
    window.sessionStorage.setItem(key, value);
}

function getDataFromLocalStorage(key) {
    return window.sessionStorage.getItem(key);
}

function removeDataFromLocalStorage(key) {
    window.sessionStorage.removeItem(key);
}

function clearAllDataFromLocalStorage() {
    window.sessionStorage.clear();
}

var __port = ":"+ (document.location.port.length == 0 ? "80" : document.location.port)

setDataToLocalStorage("WebSocketHost", (document.location.protocol === "https:" ? "wss" : "ws") +
     "://" + document.location.hostname + __port)

setDataToLocalStorage("Host", document.location.protocol+"//" + document.location.hostname + __port)


function startOrderWebSocket(onopen, onclose, onerror, onmessage) {
    return UseWebSockets(getDataFromLocalStorage("WebSocketHost") + "/ws/orders", 
        onopen, onclose, onerror, onmessage)
}

function startReservationWebSocket(onopen, onclose, onerror, onmessage) {
    return UseWebSockets(getDataFromLocalStorage("WebSocketHost") + "/ws/reservations", 
        onopen, onclose, onerror, onmessage)
}


function goToProfile() {
    window.location.href = getDataFromLocalStorage("Host") + "/profile"
}



function goToRegister() {
    window.location.href = getDataFromLocalStorage("Host") + '/register'
}


function goToLogin() {
    window.location.href = getDataFromLocalStorage("Host") + '/login'
}

function goToDishes() {
    window.location.href = getDataFromLocalStorage("Host") + '/dishes'
}

function goToCart() {
    window.location.href = getDataFromLocalStorage("Host") + '/cart'
}

function goToOrders() {
    window.location.href = getDataFromLocalStorage("Host") + '/orders'
}

function Logout() {
    fetch(getDataFromLocalStorage("Host")  + "/logout", {
        method: "POST",
        redirect: 'follow',

    }).then((res) => {
        if (res.status === 200) {
            goToLogin()
        }
    })
}