async function choose(restaurantId) {
    let formData = new FormData()
    formData.append("restaurantId", Number(restaurantId))

    await fetch(getDataFromLocalStorage("Host") + "/restaurant", {
        method: "POST",
        body: formData,
        redirect: 'follow',
    })
}