function openPopup(id) {
    document.getElementById(id).style.display = "flex";
}

function closePopup(id) {
    document.getElementById(id).style.display = "none";
}

async function saveHeight() {

    let value = document.getElementById("heightInput").value;
    let unit = document.getElementById("heightUnit").value;
    let nic = document.getElementById("patientNIC").value;

    if (!value) return;

    value = parseFloat(value);

    if (unit === "in") {
        value = value * 2.54;
    }

    const response = await fetch("/Doctor/PatientMedicalProfile/UpdateHeight", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            nic: nic,
            height: value
        })
    });

    if (response.ok) {
        closePopup("heightPopup");
        location.reload();
    }
}

async function saveWeight() {

    let value = document.getElementById("weightInput").value;
    let unit = document.getElementById("weightUnit").value;
    let nic = document.getElementById("patientNIC").value;

    if (!value) return;

    value = parseFloat(value);

    if (unit === "lbs") {
        value = value * 0.453592;
    }

    const response = await fetch("/Doctor/PatientMedicalProfile/UpdateWeight", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            nic: nic,
            weight: value
        })
    });

    if (response.ok) {
        closePopup("weightPopup");
        location.reload();
    }
}


async function saveBP() {

    let value = document.getElementById("bpInput").value;
    let nic = document.getElementById("patientNIC").value;

    if (!value) return;

    const response = await fetch("/Doctor/PatientMedicalProfile/UpdateBP", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            nic: nic,
            bp: value
        })
    });

    if (response.ok) {
        closePopup("bloodPressurePopup");
        location.reload();
    }
}


async function saveBloodType() {

    let type = document.getElementById("bloodTypeInput").value;
    let nic = document.getElementById("patientNIC").value;

    const response = await fetch("/Doctor/PatientMedicalProfile/UpdateBloodType", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            nic: nic,
            bloodType: type
        })
    });

    if (response.ok) {
        closePopup("bloodTypeConfirmPopup");
        location.reload();
    }
}

function confirmBloodType() {

    closePopup("bloodTypePopup");

    openPopup("bloodTypeConfirmPopup");
}

document.addEventListener("DOMContentLoaded", function () {

    let msg = document.getElementById("tempMessage");

    if (msg) {
        setTimeout(function () {
            msg.style.opacity = "0";

            setTimeout(function () {
                msg.style.display = "none";
            }, 500);
        }, 3000);
    }
});