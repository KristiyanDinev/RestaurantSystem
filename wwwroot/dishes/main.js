
function Logout() {

    fetch(Host + "/logout", {
        method: "POST",
        redirect: 'follow',

    }).then((res) => {
        if (res.status === 200) {
            goToLogin()
        }
    })
}

let username = getCookie("Username")

document.getElementById("UserData").innerHTML = username === "" ? "" : "Logged in as: " + username

var divDish = document.getElementById("dishes")

function addDish(data) {
    /*
    avrageTimeToCook: "3"
grams: 440
id: 1
image: ""
isAvailable: true
name: "Salad 1"
price: 10.99
type_Of_Dish: "salad"
    */
    divDish
    
}

async function getSalads() {
    const dish = "salad";
    const res = await fetch(Host + "/dishes/" + dish, {
        method: "GET",
        redirect: 'follow',
    })

    const data = await res.json()
    if (data.dishes === null) {
        document.getElementById("dish_stats").innerHTML = "Can't get " + dish+"s"
        return
    }
    
    for (let i in data.dishes) {
        addDish(data.dishes[i])
    }
}

async function getAppetizers() {
    const dish = "appetizers";
    const res = await fetch(Host + "/dishes/" + dish, {
        method: "GET",
        redirect: 'follow',
    })

    const data = await res.json()
    if (data.dishes === null) {
        document.getElementById("dish_stats").innerHTML = "Can't get " + dish
        return
    }

    for (let i in data.dishes) {
        addDish(data.dishes[i])
    }
}

async function getDishes() {
    const dish = "dishes";
    const res = await fetch(Host + "/dishes/" + dish, {
        method: "GET",
        redirect: 'follow',
    })

    const data = await res.json()
    if (data.dishes === null) {
        document.getElementById("dish_stats").innerHTML = "Can't get " + dish
        return
    }

    for (let i in data.dishes) {
        addDish(data.dishes[i])
    }
}

async function getDrinks() {
    const dish = "drinks";
    const res = await fetch(Host + "/dishes/" + dish, {
        method: "GET",
        redirect: 'follow',
    })

    const data = await res.json()
    if (data.dishes === null) {
        document.getElementById("dish_stats").innerHTML = "Can't get " + dish 
        return
    }

    for (let i in data.dishes) {
        addDish(data.dishes[i])
    }
}

async function getDesserts() {
    const dish = "desserts";
    const res = await fetch(Host + "/dishes/" + dish, {
        method: "GET",
        redirect: 'follow',
    })

    const data = await res.json()
    if (data.dishes === null) {
        document.getElementById("dish_stats").innerHTML = "Can't get " + dish
        return
    }

    for (let i in data.dishes) {
        addDish(data.dishes[i])
    }
}

