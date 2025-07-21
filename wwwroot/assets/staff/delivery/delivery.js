function selectAddress(addressId) {
    document.cookie = delivery_address_header + "=" + addressId + "; path=/";
    window.location.pathname = "/staff/delivery/restaurant";
}

function selectRestaurant(restaurantId) {
    document.cookie = delivery_restaurant_header + "=" + restaurantId + "; path=/";
    window.location.pathname = "/staff/delivery/orders";
}


async function startDelivery(orderId) {
    if (!confirm("Are you sure you want to start this delivery?")) {
        return;
    }

    try {
        await fetch(`/staff/delivery/start/${orderId}`, {
            method: 'POST',
        });

        window.location.reload()

    } catch {}
}

