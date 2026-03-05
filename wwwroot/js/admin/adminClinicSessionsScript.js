// close btn
function closeAlert() {
    document.getElementById("successAlert").style.display = "none";
}

// Edit popup

function openEditModal(id, name, start, end, maxSlots) {
    document.getElementById("editSessionID").value = id;
    document.getElementById("editSessionName").value = name;
    document.getElementById("editStartTime").value = start;
    document.getElementById("editEndTime").value = end;
    document.getElementById("editMaxSlots").value = maxSlots;

    document.getElementById("editModal").style.display = "block";
}

function closeEditModal() {
    document.getElementById("editModal").style.display = "none";
}

window.onclick = function (event){
    var modal = document.getElementById("editModal");
    if (event.target === modal) {
        modal.style.display = "none";
    }
}