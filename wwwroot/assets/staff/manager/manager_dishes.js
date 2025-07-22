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


var isInvalid = 'is-invalid'
async function createDish() {
    if (!confirm("Are you sure you want to create this dish?")) return;

    let NameElement = document.getElementById("dish_name")
    let TypeElement = document.getElementById("dish_type")
    let PriceElement = document.getElementById("dish_price")
    let IgnrElement = document.getElementById("dish_ingredients")
    let attcElement = document.getElementById("dish_attc")
    let gramsElement = document.getElementById("dish_grams")

    NameElement.classList.remove(isInvalid)
    PriceElement.classList.remove(isInvalid)
    IgnrElement.classList.remove(isInvalid)
    attcElement.classList.remove(isInvalid)
    gramsElement.classList.remove(isInvalid)
    TypeElement.classList.remove(isInvalid)

    const nameValue = NameElement.value
    const priceValue = PriceElement.value
    const IgnrValue = IgnrElement.value
    const attcValue = attcElement.value
    const gramsValue = gramsElement.value
    const typeValue = TypeElement.value

    if (!nameValue) {
        NameElement.classList.add(isInvalid)
        return
    }
    if (!priceValue) {
        PriceElement.classList.add(isInvalid)
        return
    }
    if (!IgnrValue) {
        IgnrElement.classList.add(isInvalid)
        return
    }
    if (!attcValue) {
        attcElement.classList.add(isInvalid)
        return
    }
    if (!gramsValue) {
        gramsElement.classList.add(isInvalid)
        return
    }
    if (!typeValue) {
        TypeElement.classList.add(isInvalid)
        return
    }

    let formData = new FormData()
    formData.append("Name", nameValue)
    formData.append("Type", typeValue)
    formData.append("Price", priceValue)
    formData.append("Ingredients", IgnrValue)
    formData.append("AverageTimeToCook", attcValue)
    formData.append("Grams", gramsValue)
    formData.append("IsAvailable", document.getElementById("dish_isavailable").checked)
    formData.append("Image", document.getElementById("dish_image").files[0])

    try {
        await fetch("/staff/manager/dishes/create", {
            method: "POST",
            body: formData
        })

        window.location.reload()
    } catch { }
}


async function editDish(id) {
    if (!confirm("Are you sure you want to edit this dish?")) return;

    let NameElement = document.getElementById(`name,${id}`)
    let TypeElement = document.getElementById(`type,${id}`)
    let PriceElement = document.getElementById(`price,${id}`)
    let IgnrElement = document.getElementById(`ingredients,${id}`)
    let attcElement = document.getElementById(`attc,${id}`)
    let gramsElement = document.getElementById(`grams,${id}`)

    NameElement.classList.remove(isInvalid)
    PriceElement.classList.remove(isInvalid)
    IgnrElement.classList.remove(isInvalid)
    attcElement.classList.remove(isInvalid)
    gramsElement.classList.remove(isInvalid)
    TypeElement.classList.remove(isInvalid)

    const nameValue = NameElement.value
    const priceValue = PriceElement.value
    const IgnrValue = IgnrElement.value
    const attcValue = attcElement.value
    const gramsValue = gramsElement.value
    const typeValue = TypeElement.value

    if (!nameValue) {
        NameElement.classList.add(isInvalid)
        return
    }
    if (!priceValue) {
        PriceElement.classList.add(isInvalid)
        return
    }
    if (!IgnrValue) {
        IgnrElement.classList.add(isInvalid)
        return
    }
    if (!attcValue) {
        attcElement.classList.add(isInvalid)
        return
    }
    if (!gramsValue) {
        gramsElement.classList.add(isInvalid)
        return
    }
    if (!typeValue) {
        TypeElement.classList.add(isInvalid)
        return
    }

    let formData = new FormData()
    formData.append("Id", id)
    formData.append("Name", nameValue)
    formData.append("Type", typeValue)
    formData.append("Price", priceValue)
    formData.append("Ingredients", IgnrValue)
    formData.append("AverageTimeToCook", attcValue)
    formData.append("Grams", gramsValue)
    formData.append("IsAvailable", document.getElementById(`isavailable,${id}`).checked)
    formData.append("Image", document.getElementById(`inputimage,${id}`).files[0])
    formData.append("DeleteImage", document.getElementById(`deleteimage,${id}`).checked)

    try {
        await fetch("/staff/manager/dishes/edit", {
            method: "POST",
            body: formData
        })

        window.location.reload()
    } catch { }
}

async function deleteDish(id) {
    if (!confirm('Are you sure you want to delete this dish?')) return;

    try {
        await fetch(`/staff/manager/dishes/delete/${id}`, {
            method: 'POST'
        })

        window.location.reload()
    } catch { }
}