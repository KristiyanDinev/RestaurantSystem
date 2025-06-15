
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            document.getElementById('review').src = e.target.result
        }

        reader.readAsDataURL(input.files[0]);
    }
}

document.getElementById('Image').onchange = (e) => {
    readURL(e.target);
}


var ImageFile = ""
let fileSelection = document.getElementById("Image")
fileSelection.addEventListener('change', async function (e) {
    let targetFile = e.target.files[0]
    ImageFile = ""
    if (targetFile) {

        ImageFile += targetFile.name + ';'

        var fileReaderForImage = new FileReader()
        fileReaderForImage.onload = function () {

            let array1 = new Uint8Array(fileReaderForImage.result)
            ImageFile += btoa(array1)

        }
        fileReaderForImage.readAsArrayBuffer(targetFile)
    }
})

let countryEle = document.getElementById("country")
countryEle.addEventListener('change', async function (e) {
    document.getElementById("state").style.display = "block"
})

function submit() {
    let username = document.getElementById("Username").value
    let password = document.getElementById("Password").value
    let address = document.getElementById("Address").value
    let city = document.getElementById("city").value
    let country = document.getElementById("country").value
    let state = document.getElementById("state").value
    let email = document.getElementById("Email").value
    
    
    let formData = new FormData()
    formData.append("Name", username.value)
    formData.append("Password", password.value)
    formData.append("Email", email.value)
    formData.append("Notes", document.getElementById("Notes").value)
    formData.append("Address", address.value)
    formData.append("City", city.value)
    formData.append("State", document.getElementById("State").value)
    formData.append("Country", country.value)
    formData.append("PhoneNumber", document.getElementById("Phone").value)
    formData.append("Image", ImageFile)
    formData.append("PostalCode", document.getElementById("PostalCode").value)
    formData.append("RememberMe", document.querySelector('RememberMe').checked)

    try {
        fetch(getDataFromLocalStorage("Host") + "/register", {
            method: "POST",
            body: formData,
            redirect: 'follow',

        }).then((res) => {
            if (res.status == 200) {
                window.location.reload()
                return
            }

            document.getElementById("Stats").innerHTML = "Invalid register"
        })
    } catch {
        console.error("Error during register request");
    }
}
