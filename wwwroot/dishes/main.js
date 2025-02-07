var Host = "http://127.0.0.1:7278"

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

async function addToOrder(id, price, name) {
    
    var formData = new FormData()
    formData.append("dishId", Number(id))
    formData.append("dishPrice", parseFloat(price))

    const res = await fetch(Host + "/order/add", {
        method: "POST",
        body: formData,
        redirect: 'follow',
    })

    if (res.status === 200) {
        alert("Added "+name+" dish to your cart.")
    }
}


function addDish(data, element) {
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

    let wholeDiv = document.createElement('div')
    wholeDiv.id = "whole_food"

    let foodDiv = document.createElement('div')
    foodDiv.className = "food"
    foodDiv.style.opacity = "50%"

    if (data.isAvailable) {
        foodDiv.style.opacity = "100%"
        foodDiv.onclick = () => {
            window.location.href = Host + "/dishes/id/"+data.id
        }
    }
    

    let nameP = document.createElement('p')
    nameP.innerHTML = data.name
    nameP.className = "food_name"

    let gramsP = document.createElement('p')
    gramsP.innerHTML = "grams: /"+data.grams+"/"
    gramsP.className = "food_grams"

    let priceP = document.createElement('p')
    priceP.innerHTML = data.price + " lv."
    priceP.className = "food_price"

    let orderDiv = document.createElement('div')
    let order = document.createElement('button')
    order.id = "order"
    order.innerHTML = "Add to order"
    order.style.opacity = "50%"
    orderDiv.onclick = () => {}
    if (data.isAvailable) {
        order.style.opacity = "100%"
        order.onclick = async () => {
            await addToOrder(data.id, data.price, data.name)
        }
    }
    orderDiv.appendChild(order)

    foodDiv.appendChild(nameP)
    foodDiv.appendChild(gramsP)
    if (data.image.length > 0) {
        let imageI = document.createElement('img')
        imageI.className = "food_image"
        imageI.src = Host + data.image
        foodDiv.appendChild(imageI)
    }
    foodDiv.appendChild(priceP)

    wholeDiv.appendChild(foodDiv)
    wholeDiv.appendChild(orderDiv)

    element.appendChild(wholeDiv)
    
}

async function goToSalads() {
    window.location.href = Host + '/dishes/salad'
}

async function goToAppetizers() {
    window.location.href = Host + '/dishes/appetizers'
}

async function goToDishes() {
    window.location.href = Host + '/dishes/dishes'
}

async function goToDrinks() {
    window.location.href = Host + '/dishes/drinks'
}

async function goToDesserts() {
    window.location.href = Host + '/dishes/desserts'
}

