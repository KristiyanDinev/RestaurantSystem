async function choose(restaurantId) {
    let formData = new FormData()
    formData.append("restaurantId", Number(restaurantId))

    let res = await fetch(getDataFromLocalStorage("Host") + "/restaurant", {
        method: "POST",
        body: formData,
        redirect: 'follow',
    })

    // found
    if (res.status == 200) {
        window.location = "/Dishes"
    }
}