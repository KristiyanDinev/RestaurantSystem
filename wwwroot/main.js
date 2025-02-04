var Host = "https://localhost:7278"


async function getUser(Id) {
    let formData = new FormData();
    formData.append('Id', Id);

    
    const res = await fetch(Host + "/user", {
        method: "POST",
        body: formData,
        redirect: 'follow',

    })
    
    const data = await res.json()
    console.log(data)
    let p = document.createElement("p")
    p.innerHTML = data.id
}

async function main() {

}

main();
