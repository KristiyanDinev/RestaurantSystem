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


async function edit() {

    let formData = new FormData()
    // send data to edit user profile


    
}