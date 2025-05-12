

function addDishToCart(id, name) {
    let restorantId = getCookie("restaurant_id")
    if (restorantId.length == 0) {
        alert("Select a restaurant first.")
        return
    }

    let cart = getCookie("cart")
    if (cart.length == 0) {
        document.cookie = "cart="+id;

    } else {
        document.cookie = "cart="+cart +"-"+ id;
    }
    alert("Added 1 x "+name+" to your cart")
    window.location.reload()
}

function removeDishFromCart(id, name) {
    let cart = getCookie("cart")
    if (cart.length > 0) {
        let dishes = cart.split('-')
        var index = dishes.indexOf(id);
        if (index > -1) {
            dishes.splice(index, 1);
        }
        document.cookie = "cart="+dishes.join('-')
    }
    alert("Removed 1 x "+name+" from your cart")
    window.location.reload()
    //document.cookie = "cart."+id +'=; path=/; domain=127.0.0.1; expires=Thu, 01 Jan 1970 00:00:00 UTC';
}

function restorantAddressChange() {
    document.cookie = "RestorantId="+document.getElementById('restorant_address').value;
}



var params = new URLSearchParams()

function goToDish(id) {
    params.append("dishId", String(id))
    window.location.href = "/single_dish?" + params.toString()
}

function goToSalads() {
    params.append("type", "salad")
    window.location.href = "/dish?" + params.toString()
}

function goToAppetizers() {
    params.append("type", "appetizers")
    window.location.href = "/dish?" + params.toString()
}

function goToDishesFood() {
    params.append("type", "dishes")
    window.location.href = "/dish?" + params.toString()
}

function goToDrinks() {
    params.append("type", "drink")
    params.append("restorant_id", String(document.getElementById('restorant_address').value))
    window.location.href = "/dish?" + params.toString()
}

function goToDesserts() {
    params.append("type", "desserts")
    window.location.href = "/dish?" + params.toString()
}

