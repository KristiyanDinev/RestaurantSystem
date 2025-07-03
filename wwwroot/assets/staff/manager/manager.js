function page(pageNumber, curPage) {
    let elements = document.getElementsByClassName("page-row")
    if (elements.length < 10 && pageNumber > curPage) {
        alert("You are already on the last page.")
        return
    }

    window.location.href = window.location.pathname +
        `?${new URLSearchParams({ page: pageNumber }).toString()}`
}


function toggleElement(id) {
    let ele = document.getElementById(id)
    if (ele.style.display == 'none') {
        ele.style.display = 'block'

    } else {
        ele.style.display = 'none'
    }
}