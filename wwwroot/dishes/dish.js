

function addDishToCart(id, name) {
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

function goToDish(id) {
    let params = new URLSearchParams()
    params.append("dishId", String(id))
    window.location.href = "/dish/id?" + params.toString()
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

