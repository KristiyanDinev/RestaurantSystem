
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
    
    
    let formData = new FormData()
    formData.append("Username", username.value)
    formData.append("Password", password.value)
    formData.append("Email", email.value)
    formData.append("Notes", document.getElementById("Notes").value)
    formData.append("Address", address.value)
    formData.append("City", city.value)
    formData.append("State", document.getElementById("State").value)
    formData.append("Country", country.value)
    formData.append("PhoneNumber", document.getElementById("Phone").value)
    formData.append("Image", ImageFile)
    formData.append("RememberMe", document.querySelector('#RememberMe:checked') === null ?
                                 "off" : "on")

    fetch(getDataFromLocalStorage("Host") + "/register", {
        method: "POST",
        body: formData,
        redirect: 'follow',
        
    }).then((res) => {
        if (res.status === 200) {
            window.location.reload()
        }
    })

}
