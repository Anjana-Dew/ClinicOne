// close btn
function closeAlert() {
    document.getElementById("successAlert").style.display = "none";
}

function handleEditClick(btn) {
    openEditModal(
        btn.getAttribute("data-id"),
        btn.getAttribute("data-name"),
        btn.getAttribute("data-start"),
        btn.getAttribute("data-ent"),
        btn.getAttribute("data-max"),
        btn.getAttribute("data-type"),
        btn.getAttribute("data-days"),
        btn.getAttribute("data-date"),

    )
}
// Edit popup
function openEditModal(id, name, start, end, maxSlots, type, days, customDate) {
    document.getElementById("editSessionID").value = id;
    document.getElementById("editSessionName").value = name;
    document.getElementById("editStartTime").value = start;
    document.getElementById("editEndTime").value = end;
    document.getElementById("editMaxSlots").value = maxSlots;

    if (type == "Weekly") {
        document.getElementById("editWeekly").checked = true;
    } else {
        document.getElementById("editCustom").checked = true;
    }

    toggleEditScheduleType();

    if (days) {
        let selectedDays = days.split(',');

        document.querySelectorAll('#editWeeklyOptions input[type="checkbox"]').forEach(cb => {
            cb.checked = selectedDays.includes(cb.value);
        });
    }

    if (customDate) {
        document.getElementById("editCustomDate").value = customDate;
    }
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
function toggleEditScheduleType() {
    let type = document.querySelector('input[name="ScheduleType"]:checked')?.value;

    document.getElementById("editWeeklyOptions").style.display = type === "Weekly" ? "block" : "none";

    document.getElementById("editCustomOptions").style.display = type === "Custom" ? "block" : "none";
}
//Clinic Type
function toggleScheduleType() {
    const selecetd = document.querySelector('input[name="ScheduleType"]:checked')?.value;

    const weeklyDiv = document.getElementById("weeklyOptions");
    const customDiv = document.getElementById("customOptions");

    if (selecetd == "Weekly") {
        weeklyDiv.style.display = "block";
        customDiv.style.display = "none";
    } else if (selecetd == "Custom") {
        weeklyDiv.style.display = "none";
        customDiv.style.display = "block";
    } else {
        weeklyDiv.style.display = "none";
        customDiv.style.display = "none";
    }
}