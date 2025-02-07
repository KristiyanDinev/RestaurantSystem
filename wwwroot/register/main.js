
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
    let email = document.getElementById("Email")
    
    if (username.value.replace(" ", "").length === 0 ||
        password.value.replace(" ", "").length === 0 ||
        address.value.replace(" ", "").length === 0 ||
        email.value.replace(" ", "").length === 0) {
        
        document.getElementById("Stats").innerHTML = "Requred Inputs: Username, Email and Address"
       return;
    }


    let formData = new FormData()
    formData.append("username", username.value)
    formData.append("password", password.value)
    formData.append("email", email.value)
    formData.append("notes", document.getElementById("Notes").value)
    formData.append("address", address.value)
    formData.append("phone", document.getElementById("Phone").value)
    formData.append("rememberMe", document.getElementById("RememberMe").value)
    formData.append("image", ImageFile)

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
