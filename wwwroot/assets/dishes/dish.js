
function addDishToCart(id, name, isInCart) {
    let restorantId = getCookie(restaurantId_header)
    if (restorantId.length == 0) {
        alert("Select a restaurant first.")
        window.location.href = "/restaurants"
        return
    }

    let cart = getCookie(cart_header)
    if (cart.length == 0) {
        document.cookie = cart_header+"="+id + "; path=/";

    } else {
        document.cookie = cart_header + "=" + cart + _cart_seperator + id + '; path=/';
    }

    alert("Added 1 x " + name + " to your cart")
    if (isInCart) {
        window.location.reload()
    }
}

function removeDishFromCart(id, name, isInCart) {
    let cart = getCookie(cart_header)
    if (cart.length > 0) {
        let dishes = cart.split(_cart_seperator)
        var index = dishes.indexOf(id);
        if (index > -1) {
            dishes.splice(index, 1);
        }
        document.cookie = cart_header + "=" + dishes.join(_cart_seperator) + "; path=/"
    }
    alert("Removed 1 x " + name + " from your cart")
    if (isInCart) {
        window.location.reload()
    }
    //document.cookie = "cart."+id +'=; path=/; domain=127.0.0.1; expires=Thu, 01 Jan 1970 00:00:00 UTC';
}



var params = new URLSearchParams()

function goToDish(id) {
    window.location.href = "/dish/" + Number(id)
}

function goToSalads() {
    window.location.href = "/dishes/salad"
}

function goToAppetizers() {
    window.location.href = "/dishes/appetizers"
}

function goToDishesFood() {
    window.location.href = "/dishes/dishes"
}

function goToDrinks() {
    params.append("type", "drink")
    window.location.href = "/dishes/drink"
}

function goToDesserts() {
    params.append("type", "desserts")
    window.location.href = "/dishes/desserts"
}

