function page(pageNumber, curPage) {
    let elements = document.getElementsByClassName("page-row")
    if (elements.length < 10 && pageNumber > curPage) {
        alert("You are already on the last page.")
        return
    }

    window.location.href = window.location.pathname +
        `?${new URLSearchParams({ page: pageNumber }).toString()}`
}


function toggleGiveRole() {
    let ele = document.getElementById('give_role')
    if (ele.style.display == 'none') {
        ele.style.display = 'block'

    } else {
        ele.style.display = 'none'
    }
}

function toggleRemoveRole() {
    let ele = document.getElementById('remove_role')
    if (ele.style.display == 'none') {
        ele.style.display = 'block'

    } else {
        ele.style.display = 'none'
    }
}


async function addStaff() {
    const email = document.getElementById("staff_email").value
    if (!email) {
        alert("Fill in with a valid email")
        return
    }
    if (!confirm("Are you sure you want to add " + email + " as staff?")) return;

    const status = document.getElementById("status");
    try {
        let formData = new FormData();
        formData.append("Email", email);

        const res = await fetch("/staff/manager/employees/add", {
            method: 'POST',
            body: formData
        })

        if (res.ok) {
            window.location.reload()
            return;
        }
    } catch { }
    status.innerHTML = `Coudn't add ${email} as staff`
}



async function removeStaff(id, name) {
    if (!confirm(`Are you sure you want to remove ${name} as staff?`)) return;

    const status = document.getElementById("status");
    try {
        let formData = new FormData();
        formData.append("Id", id);

        const res = await fetch("/staff/manager/employees/remove", {
            method: 'POST',
            body: formData
        })

        if (res.ok) {
            window.location.reload()
            return;
        }
    } catch { }
    status.innerHTML = `Coudn't remove ${name} from staff`
}



async function giveRole(id, name) {
    const role = document.getElementById('gi_role').value
    if (!role) {
        alert('Select at least one role')
        return
    }
    if (!confirm(`Are you sure you want to give ${name} a ${role} role?`)) return;

    const status = document.getElementById("status");
    try {
        let formData = new FormData();
        formData.append("Id", id);
        formData.append("Role", role);

        const res = await fetch("/staff/manager/employees/role/give", {
            method: 'POST',
            body: formData
        })

        if (res.ok) {
            window.location.reload()
            return;
        }
    } catch { }
    status.innerHTML = `Coudn't give ${role} role to ${name}`
}



async function removeRole(id, name) {
    const role = document.getElementById('rm_role').value
    if (!role) {
        alert('Select at least one role')
        return
    }
    if (!confirm(`Are you sure you want to remove ${name} a ${role} role?`)) return;

    const status = document.getElementById("status");
    try {
        let formData = new FormData();
        formData.append("Id", id);
        formData.append("Role", role);

        const res = await fetch("/staff/manager/employees/role/remove", {
            method: 'POST',
            body: formData
        })

        if (res.ok) {
            window.location.reload()
            return;
        }
    } catch { }
    status.innerHTML = `Coudn't remove ${role} role to ${name}`
}