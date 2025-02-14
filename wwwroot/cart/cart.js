var dish_counts = {}
var displayedIDs = []

function displayDish(data, element) {
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

    if (displayedIDs.includes(data.id)) {
        return;
    }

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


    let quantityDiv = document.createElement('div')
    quantityDiv.className = 'quantity'

    let quanitityL = document.createElement('label')
    quanitityL.value = "Quantity"

    quantityDiv.appendChild(quanitityL)

    let counter = document.createElement('input')
    counter.type = "number"
    counter.min = '1'
    counter.value = String(dish_counts[String(data.id)])

    quantityDiv.appendChild(counter)

    wholeDiv.appendChild(quantityDiv)
    
    element.appendChild(wholeDiv)

    displayedIDs.push(data.id)
    
}

async function getDishes() {

    let formData = new FormData()

    const res = await fetch(Host + "/order/dish/current", {
        method: "POST",
        body: formData,
        redirect: 'follow',
    })

    const data = await res.json()
    if (data.dishes_to_order === null) {
        document.getElementById("dish_stats").innerHTML = "Can't get current dishes from cart"
        return
    }


    let dish_ids = []
    for (let i in data.dishes_to_order) {
        let temp_dish_data = data.dishes_to_order[i]
        let id = String(temp_dish_data.id)
        if (dish_ids.includes(id)) {
            dish_counts[id] += 1

        } else {
            dish_ids.push(id)
            dish_counts[id] = 1
        }
    }

    for (let i in data.dishes_to_order) {
        displayDish(data.dishes_to_order[i], document.getElementById("dishes"))
    }
}
 
getDishes()