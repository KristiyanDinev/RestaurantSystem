
function addDishToCart(id, name, isInCart) {
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
}

function goToDish(id) {
    window.location.pathname = "/dish/" + Number(id)
}


