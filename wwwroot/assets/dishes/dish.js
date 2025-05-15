

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



var params = new URLSearchParams()

function goToDish(id) {
    params.append("dishId", String(id))
    window.location.href = "/single_dish?" + params.toString()
}

function goToSalads() {
    window.location.href = "/Dishes/salad"
}

function goToAppetizers() {
    window.location.href = "/Dishes/appetizers"
}

function goToDishesFood() {
    window.location.href = "/Dishes/dishes"
}

function goToDrinks() {
    params.append("type", "drink")
    window.location.href = "/Dishes/drinks"
}

function goToDesserts() {
    params.append("type", "desserts")
    window.location.href = "/Dishes/desserts"
}

