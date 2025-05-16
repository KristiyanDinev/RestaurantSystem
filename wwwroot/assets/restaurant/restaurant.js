async function choose(restaurantId) {
    document.cookie = restaurantId_header + "=" + Number(restaurantId);
    window.location.href = "/dishes"
}