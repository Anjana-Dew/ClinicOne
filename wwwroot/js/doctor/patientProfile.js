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


//Clinic scheduling

document.getElementById("clinicDatePicker").addEventListener("change", function () {

    let selectedDate = this.value;

    fetch(`/Doctor/PatientMedicalProfile/GetSessionsForDate?clinicDate=${selectedDate}`)
        .then(res => res.json())
        .then(data => {

            let container = document.querySelector(".session-toggle");
            container.innerHTML = "";

            data.forEach(session => {

                let isFull = session.remainingSlots <= 0;

                let option = `
                    <label class="session-option ${isFull ? "session-full" : ""}">
                        <input type="radio"
                               name="SelectedSessionID"
                               value="${session.sessionID}"
                               ${isFull ? "disabled" : ""}
                               required />

                        <span>
                            ${session.sessionName}
                            <small>
                                (${session.startTime} - ${session.endTime})
                            </small>
                            <br>

                            ${isFull
                        ? `<small class="slot-full">No slots available</small>`
                        : `<small class="slot-remaining">${session.remainingSlots} slots left</small>`
                    }

                        </span>
                    </label>
                `;

                container.insertAdjacentHTML("beforeend", option);
            });
        });
});
document.addEventListener("DOMContentLoaded", function () {

    let datePicker = document.getElementById("clinicDatePicker");

    let today = new Date();
    let yyyy = today.getFullYear();
    let mm = String(today.getMonth() + 1).padStart(2, '0');
    let dd = String(today.getDate()).padStart(2, '0');

    let minDate = `${yyyy}-${mm}-${dd}`;

    datePicker.setAttribute("min", minDate);
});
document.addEventListener("DOMContentLoaded", function () {

    let picker = document.getElementById("clinicDatePicker");

    if (picker) {
        picker.dispatchEvent(new Event("change"));
    }

});