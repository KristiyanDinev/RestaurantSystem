var Host = "http://127.0.0.1:7278"

function addDishToCart(id) {
    document.cookie = "cart."+id +'='+id;
}

function removeDishFromCart(id) {
    document.cookie = "cart."+id +'=; path=/; domain=127.0.0.1; expires=Thu, 01 Jan 1970 00:00:00 UTC';
}

function goToSalads() {
    let params = new URLSearchParams()
    params.append("type", "salad")
    window.location.href = "/dish?" + params.toString()
}

function goToAppetizers() {
    //window.location.href = Host + '/dishes/appetizers'
    let params = new URLSearchParams()
    params.append("type", "appetizers")
    window.location.href = "/dish?" + params.toString()
}

function goToDishesFood() {
    let params = new URLSearchParams()
    params.append("type", "dishes")
    window.location.href = "/dish?" + params.toString()
}

function goToDrinks() {
    let params = new URLSearchParams()
    params.append("type", "drink")
    window.location.href = "/dish?" + params.toString()
}

function goToDesserts() {
    let params = new URLSearchParams()
    params.append("type", "desserts")
    window.location.href = "/dish?" + params.toString()
}

