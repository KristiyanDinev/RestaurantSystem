
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#review').attr('src', e.target.result);
        }

        reader.readAsDataURL(input.files[0]);
    }
}

$("#Image").change(function () {
    readURL(this);
});

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

function submit() {
    let username = document.getElementById("Username")
    let password = document.getElementById("Password")
    let address = document.getElementById("Address")
    let city = document.getElementById("City")
    let country = document.getElementById("Country")
    let email = document.getElementById("Email")
    
    if (username.value.replace(" ", "").length === 0 ||
        password.value.replace(" ", "").length === 0 ||
        address.value.replace(" ", "").length === 0 ||
        city.value.replace(" ", "").length === 0 ||
        country.value.replace(" ", "").length === 0 ||
        email.value.replace(" ", "").length === 0) {
        
        document.getElementById("Stats").innerHTML = "Requred Inputs: Username, Password, Email, Address, City and Country."
       return;
    }

    let state = document.getElementById("State")

    if (address.value.includes(';') || city.value.includes(';') || country.value.includes(';') || 
        state.value.includes(';') ||
    
        address.value.includes('|') || city.value.includes('|') || country.value.includes('|') || 
        state.value.includes('|')) {
            document.getElementById("Stats").innerHTML = "No ; or | in the address, city, state or country input."
        return;
    }

    
    let formData = new FormData()
    formData.append("username", username.value)
    formData.append("password", password.value)
    formData.append("email", email.value)
    formData.append("notes", document.getElementById("Notes").value)
    formData.append("fulladdress", address.value + ";"+ city.value+";" + state.value + ";"+ country.value)
    formData.append("phone", document.getElementById("Phone").value)
    formData.append("image", ImageFile)
    formData.append("rememberMe", document.querySelector('#RememberMe:checked') === null ?
                                 "off" : "on")

    fetch(Host + "/register", {
        method: "POST",
        body: formData,
        redirect: 'follow',
        
    }).then((res) => {
        if (res.status === 200) {
            window.location.reload()
        }
    })

}
