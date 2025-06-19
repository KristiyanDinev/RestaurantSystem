async function submit() {
    if (!confirm("Are you sure you want to place this order?")) {
        return
    }



}




function toggleDish(type) {
    const dish = document.getElementById(type);
    if (dish.style.display === "none") {
        dish.style.display = "block";

    } else {
        dish.style.display = "none";
    }
}