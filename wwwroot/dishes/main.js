var Host = "https://localhost:7278"

function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

async function getUser(id) {
    let formData = new FormData();
    formData.append("Id", Number(id));

    const res = await fetch(Host + "/user", {
        method: "POST",
        body: formData,
        redirect: 'follow',
    })

    const data = await res.json()
    document.getElementById("UserData").innerHTML = "Logged in as: "+data.username
}

var divDish = document.getElementById("dishes")

getUser(getCookie("UserId"))

function addDish(data) {
    // {}
}

async function getSalads() {
    let formData = new FormData();
    formData.append("type", "salad");

    const res = await fetch(Host + "/user", {
        method: "POST",
        body: formData,
        redirect: 'follow',
    })

    const data = await res.json()
    if (data.dishes === null) {
        document.getElementById("dish_stats").innerHTML = "Can't get salads"
        return
    }
    console.log(data.dishes)
    for (let i in data.dishes) {
        addDish(data.dishes[i])
    }
}

async function getAppetizers() {

}

async function getDishes() {

}

async function getDrinks() {

}

async function getDesserts() {

}