function readURL(input, previewId) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            document.getElementById(previewId).src = e.target.result;
        }

        reader.readAsDataURL(input.files[0]);
    }
}


function registerPreview(id) {
    document.getElementById(`inputimage,${id}`).onchange = (e) => {
        readURL(e.target, `preview,${id}`);
    };
}

function removeImage(id) {
    if (id == null) {
        document.getElementById('dish_image').value = ""
        document.getElementById('preview').src = ''
    } else {
        document.getElementById(`inputimage,${id}`).value = ""
        document.getElementById(`preview,${id}`).src = ''
    }
}

function toggleCreateDish() {
    toggleElement('create_dish')
}


function toggleEditDish(Id) {
    toggleElement(`image,${Id}`)
    toggleElement(`name,${Id}`)
    toggleElement(`price,${Id}`)
    toggleElement(`ingredients,${Id}`)
    toggleElement(`grams,${Id}`)
    toggleElement(`type,${Id}`)
    toggleElement(`attc,${Id}`)
    toggleElement(`isavailable,${Id}`)
    toggleElement(`submit,${Id}`)
}

async function createDish() {
    if (!confirm("Are you sure you want to create this dish?")) return;

    let formData = new FormData()
    formData.append("Name", document.getElementById("dish_name").value)
    formData.append("Type", document.getElementById("dish_type").value)
    formData.append("Price", document.getElementById("dish_price").value)
    formData.append("Ingredients", document.getElementById("dish_ingredients").value)
    formData.append("AverageTimeToCook", document.getElementById("dish_attc").value)
    formData.append("Grams", document.getElementById("dish_grams").value)
    formData.append("IsAvailable", document.getElementById("dish_isavailable").checked)
    formData.append("Image", document.getElementById("dish_image").files[0])

    try {
        const res = await fetch("/staff/manager/dishes/create", {
            method: "POST",
            body: formData
        })

        if (res.ok) {
            window.location.reload()
            return
        }

    } catch { }

    document.getElementById("status").innerHTML = "Couldn't create dish"
}


async function editDish(id) {
    if (!confirm("Are you sure you want to edit this dish?")) return;

    let formData = new FormData()
    formData.append("Id", id)
    formData.append("Name", document.getElementById(`name,${id}`).value)
    formData.append("Type", document.getElementById(`type,${id}`).value)
    formData.append("Price", document.getElementById(`price,${id}`).value)
    formData.append("Ingredients", document.getElementById(`ingredients,${id}`).value)
    formData.append("AverageTimeToCook", document.getElementById(`attc,${id}`).value)
    formData.append("Grams", document.getElementById(`grams,${id}`).value)
    formData.append("IsAvailable", document.getElementById(`isavailable,${id}`).checked)
    formData.append("Image", document.getElementById(`inputimage,${id}`).files[0])
    formData.append("DeleteImage", document.getElementById(`deleteimage,${id}`).checked)

    try {
        const res = await fetch("/staff/manager/dishes/edit", {
            method: "POST",
            body: formData
        })

        if (res.ok) {
            window.location.reload()
            return
        }

    } catch { }

    document.getElementById("status").innerHTML = "Couldn't edit that dish"
}

async function deleteDish(id) {
    if (!confirm('Are you sure you want to delete this dish?')) return;

    try {
        const res = await fetch(`/staff/manager/dishes/delete/${id}`, {
            method: 'POST'
        })

        if (res.ok) {
            window.location.reload()
            return
        }
    } catch { }
    document.getElementById("status").innerHTML = "Couldn't delete that dish"
}