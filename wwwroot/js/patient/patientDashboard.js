function openModal() {
    document.getElementById("profileModal").style.display = "flex";
}

function closeModal() {
    document.getElementById("profileModal").style.display = "none";
}

window.onclick = function (event) {
    const modal = document.getElementById("profileModal");
    if (event.target === modal) {
        modal.style.display = "none";
    }
};