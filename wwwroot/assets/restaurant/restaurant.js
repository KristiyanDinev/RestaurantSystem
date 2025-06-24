async function choose(restaurantId) {
    document.cookie = restaurantId_header + "=" + restaurantId;
    window.location.pathname = "/dishes"
}