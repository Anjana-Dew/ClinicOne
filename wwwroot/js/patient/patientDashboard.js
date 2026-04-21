function openEditProfile() {
    document.getElementById("editModal").style.display = "block";
}

function closeEditProfile() {
    document.getElementById("editModal").style.display = "none";
}

// CLOSE IF CLICK OUTSIDE
window.onclick = function (event) {
    const modal = document.getElementById("editModal");
    if (event.target === modal) {
        modal.style.display = "none";
    }
};