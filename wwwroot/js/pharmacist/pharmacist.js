let currentSelect = null;
function showPopupError(msg) {
    const box = document.getElementById("popupMsg");
    box.innerText = msg;
    box.style.display = "block";
}

function clearPopupError() {
    const box = document.getElementById("popupMsg");
    box.innerText = "";
    box.style.display = "none";
}

function showError(input, msg) {

    const msgBox = document.getElementById("msg");

    input.classList.add("input-error");
    input.classList.remove("input-success");

    msgBox.className = "form-message show error";
    msgBox.innerText = msg;
}

function showSuccess(input, msg) {

    const msgBox = document.getElementById("msg");

    input.classList.add("input-success");
    input.classList.remove("input-error");

    msgBox.className = "form-message show success";
    msgBox.innerText = msg;
}

function clearError(input) {

    const msgBox = document.getElementById("msg");

    input.classList.remove("input-error", "input-success");

    msgBox.className = "form-message";
    msgBox.innerText = "";
}


function searchPatient() {

    let nicInput = document.getElementById("nicInput");
    let nic = nicInput.value.trim();

    if (!nic) {
        showError(nicInput, "Enter NIC");
        return;
    }

    if (nic.length < 10) {
        showError(nicInput, "Invalid NIC");
        return;
    }

    clearError(nicInput);
    document.getElementById("msg").className = "form-message show";
    document.getElementById("msg").innerText = "Searching...";

    fetch(`/Pharmacist/Prescription/Search?nic=${nic}`)
        .then(res => res.json())
        .then(data => {

            if (!data.success) {

                if (data.message.includes("No prescription")) {
                    showError(nicInput, "No pending prescriptions ");
                } else {
                    showError(nicInput, data.message);
                }

                return;
            }

            openPopup(data);
        })
        .catch(() => showError(nicInput, "Server error"));
}



function openPopup(data) {

    document.getElementById("popup").style.display = "flex";

    document.getElementById("name").innerText = data.patientName;
    document.getElementById("nic").innerText = data.patientNIC;
    document.getElementById("prescriptionId").innerText = data.prescriptionId;

    let table = document.getElementById("table");
    table.innerHTML = "";

    let hasNotGiven = data.medicines.some(m => (m.status ?? "Given").trim() !== "Given");

    document.getElementById("externalBtn").style.display =
        hasNotGiven ? "inline-block" : "none";


    data.medicines.forEach(m => {

        const status = (m.status ?? "Given").trim().toLowerCase();

        table.innerHTML += `
<tr data-id="${m.prescMedID}">
    <td>${m.medicineName}</td>
    <td>${m.dosage}</td>
    <td>${m.duration}</td>
    <td>${m.timesPerDay}</td>
    <td>
        <select class="status-dropdown">
    <option value="Given" ${status === "Given" || status === "" ? "selected" : ""}>Given</option>
<option value="Not Given" ${status === "Not Given" ? "selected" : ""}>Not Given</option>
<option value="Partially Given" ${status === "Partially Given" ? "selected" : ""}>Partially Given</option>
</select>
    </td>
</tr>`;
    });


    document.querySelectorAll(".status-dropdown").forEach(select => {

        select.addEventListener("change", function () {

            const value = this.value;
            const row = this.closest("tr");

            if (value === "Given") {
                this.removeAttribute("data-reason");

                const reasonCell = row.querySelector(".reason-text");
                if (reasonCell) {
                    reasonCell.remove();
                }
            }
            else {
                currentSelect = this;
                document.getElementById("reasonInput").value = "";
                document.getElementById("reasonModal").style.display = "flex";
            }

            updateExternalButton();
        });
    });
}

function closePopup() {
    document.getElementById("popup").style.display = "none";
}


function submitReason() {

    let input = document.getElementById("reasonInput");
    let val = input.value.trim();

    if (!val) {
        input.style.border = "1px solid red";
        input.placeholder = "Reason required";
        return;
    }

    input.style.border = "1px solid #ccc";

    currentSelect.setAttribute("data-reason", val);

    let row = currentSelect.closest("tr");

    let existing = row.querySelector(".reason-text");

    if (!existing) {
        let td = document.createElement("td");
        td.className = "reason-text";
        td.innerText = val;
        row.appendChild(td);
    } else {
        existing.innerText = val;
    }

    closeReason();
}


function closeReason() {
    document.getElementById("reasonModal").style.display = "none";
    document.getElementById("reasonInput").value = "";
}


function saveData() {

    clearPopupError();

    let data = [];
    let hasError = false;

    document.querySelectorAll("#table tr").forEach(row => {

        let select = row.querySelector(".status-dropdown");

        let status = select.value;
        let reason = select.getAttribute("data-reason") || "";

        if (status !== "Given" && !reason.trim()) {
            hasError = true;

            row.style.background = "#fff3f3";
        } else {
            row.style.background = "";
        }

        data.push({
            prescMedID: parseInt(row.getAttribute("data-id")),
            status: status,
            reason: reason
        });
    });

    if (hasError) {
        showPopupError("Please enter reasons for all non-given medicines");
        return;
    }

    window._tempData = data;
    document.getElementById("confirmModal").style.display = "flex";
}

function updateExternalButton() {

    let hasNotGiven = false;

    document.querySelectorAll("#table .status-dropdown").forEach(select => {
        if (select.value !== "Given") {
            hasNotGiven = true;
        }
    });

    const btn = document.getElementById("externalBtn");
    btn.style.display = hasNotGiven ? "inline-block" : "none";
}

function generateExternal() {

    let meds = [];

    let hasNotGiven = false;

    document.querySelectorAll("#table tr").forEach(row => {

        let select = row.querySelector(".status-dropdown");
        let status = select.value;
        let reason = select.getAttribute("data-reason") || "";

        if (status !== "Given") {
            hasNotGiven = true;
        }

        if (status !== "Given" && reason.trim() !== "") {
            meds.push({
                medicineName: row.cells[0].innerText,
                dosage: row.cells[1].innerText,
                duration: row.cells[2].innerText,
                timesPerDay: parseInt(row.cells[3].innerText),
                reason: reason
            });
        }
    });

    if (!hasNotGiven) {
        showPopupError("All medicines already given");
        return;
    }

    if (meds.length === 0) {
        showPopupError("Please add reasons for non-given medicines");
        return;
    }

    fetch('/Pharmacist/Prescription/GenerateExternal', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            patientName: document.getElementById("name").innerText,
            NIC: document.getElementById("nic").innerText,
            medicines: meds
        })
    })
        .then(res => {
            if (!res.ok) {
                return res.text().then(err => { throw new Error(err); });
            }
            return res.blob();
        })
        .then(blob => {
            let url = URL.createObjectURL(blob);
            let a = document.createElement("a");
            a.href = url;
            a.download = "External.pdf";
            a.click();
        })
        .catch(err => {
            showPopupError(err.message);
        });
}

function closeConfirm() {
    document.getElementById("confirmModal").style.display = "none";
}

function confirmSave() {

    const data = window._tempData;

    fetch('/Pharmacist/Prescription/Confirm', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
        .then(res => res.json())
        .then(res => {

            if (res.success) {

                closeConfirm();
                closePopup();

                document.getElementById("table").innerHTML = "";
                document.getElementById("nicInput").value = "";

            } else {
                showPopupError("Save failed. Try again.");
            }
        });
}

function showPopupError(msg) {
    const box = document.getElementById("popupMsg");
    box.innerText = msg;
    box.style.display = "block";

    setTimeout(() => {
        box.style.display = "none";
    }, 3000);
}