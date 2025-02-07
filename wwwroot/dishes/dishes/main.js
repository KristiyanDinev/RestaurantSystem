

async function getDishes() {
    const dish = "dishes";

    let formData = new FormData()
    formData.append("type", dish)


    const res = await fetch(Host + "/dishes", {
        method: "POST",
        body: formData,
        redirect: 'follow',
    })

    const data = await res.json()
    if (data.dishes === null) {
        document.getElementById("dish_stats").innerHTML = "Can't get " + dish
        return
    }

    for (let i in data.dishes) {
        addDish(data.dishes[i], document.getElementById("dishes"))
    }
}

getDishes()