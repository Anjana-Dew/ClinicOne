let currentSelect = null;
let lastLoadedPrescriptionId = null;
let isSearching = false;
let lastPrescriptionId = null;
let saved = localStorage.getItem("confirmedPrescriptions");

window.confirmedPrescriptions = new Set(
    saved ? JSON.parse(saved) : []
);

let currentPrescriptionId = null;

function showPopupError(msg) {
    const box = document.getElementById("popupMsg");
    if (!box) return;

    box.innerText = msg;
    box.style.display = "block";

    setTimeout(() => {
        box.style.display = "none";
    }, 3000);
}

function clearPopupError() {
    const box = document.getElementById("popupMsg");
    if (!box) return;

    box.innerText = "";
    box.style.display = "none";
}

function showError(input, msg) {
    const msgBox = document.getElementById("msg");
    if (!msgBox) return;

    input.classList.add("input-error");
    input.classList.remove("input-success");

    msgBox.className = "form-message show error";
    msgBox.innerText = msg;
}

function clearError(input) {
    const msgBox = document.getElementById("msg");
    if (!msgBox) return;

    input.classList.remove("input-error", "input-success");
    msgBox.className = "form-message";
    msgBox.innerText = "";
}


document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("searchForm");

    if (!form) return;

    form.addEventListener("submit", function (event) {
        event.preventDefault();
        searchPatient();
    });
});



window.searchPatient = function (event) {
    let nicInput = document.getElementById("nicInput");
    let nic = nicInput.value.trim();


    document.getElementById("table").innerHTML = "";


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
                showError(nicInput, data.message || "No prescription found");
                return;
            }

            openPopup(data);
        })

        .catch(() => {
            showError(nicInput, "Server error");
        })
        .finally(() => {
            isSearching = false;
        });
};


function openPopup(data) {

    lastPrescriptionId = data.prescriptionId;

    console.log("OPEN POPUP:", data);

    if (!data || !data.medicines) return;

    if (!data || !data.medicines || data.medicines.length === 0) {
        showError(document.getElementById("nicInput"), data.message || "No pending prescriptions");
        return;
    }

    const hasPending = data.medicines.some(m =>
        (m.status ?? "Given").trim().toLowerCase() !== "given"
    );

    if (!hasPending) {
        showError(document.getElementById("nicInput"), "No pending medicines in this prescription");
        return;
    }

    lastLoadedPrescriptionId = data.prescriptionId;

    const popup = document.getElementById("popup");
    if (!popup) return;

    popup.style.display = "flex";

    const name = document.getElementById("name");
    const nic = document.getElementById("nic");
    const pid = document.getElementById("prescriptionId");
    const table = document.getElementById("table");

    if (name) name.innerText = data.patientName;
    if (nic) nic.innerText = data.patientNIC;
    if (pid) pid.innerText = data.prescriptionId;

    table.innerHTML = "";

    let hasNotGiven = data.medicines.some(m =>
        (m.status ?? "Given").trim() !== "Given"
    );

    const externalBtn = document.getElementById("externalBtn");
    if (externalBtn) {
        externalBtn.style.display = hasNotGiven ? "inline-block" : "none";
    }

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

            const row = this.closest("tr");

            if (this.value === "Given") {
                this.removeAttribute("data-reason");
                const r = row.querySelector(".reason-text");
                if (r) r.remove();
            } else {
                currentSelect = this;
                const reasonInput = document.getElementById("reasonInput");
                if (reasonInput) reasonInput.value = "";
                const modal = document.getElementById("reasonModal");
                if (modal) modal.style.display = "flex";
            }

            updateExternalButton();
        });
    });
}


function closePopup() {
    const popup = document.getElementById("popup");
    if (popup) popup.style.display = "none";

    lastLoadedPrescriptionId = null;

    const table = document.getElementById("table");
    if (table) table.innerHTML = "";

    const nicInput = document.getElementById("nicInput");
    if (nicInput) nicInput.value = "";
}



function submitReason() {

    let input = document.getElementById("reasonInput");
    if (!input) return;

    let val = input.value.trim();

    if (!val) {
        input.style.border = "1px solid red";
        return;
    }

    input.style.border = "1px solid #ccc";

    if (!currentSelect) return;

    currentSelect.setAttribute("data-reason", val);

    let row = currentSelect.closest("tr");

    if (!row) return;

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
    const modal = document.getElementById("reasonModal");
    if (modal) modal.style.display = "none";

    const input = document.getElementById("reasonInput");
    if (input) input.value = "";
}


function saveData() {

    let data = [];
    let hasError = false;
    let hasNotGiven = false;

    document.querySelectorAll("#table tr").forEach(row => {

        let select = row.querySelector(".status-dropdown");
        if (!select) return;

        let status = select.value;
        let reason = select.getAttribute("data-reason") || "";

        if (status !== "Given") hasNotGiven = true;

        if (status !== "Given" && !reason.trim()) {
            hasError = true;
            row.style.background = "#fff3f3";
        }

        data.push({
            prescMedID: parseInt(row.getAttribute("data-id")),
            status,
            reason
        });
    });

    if (hasError) {
        showPopupError("Please enter reasons");
        return;
    }

    window._tempData = data;

    const modal = document.getElementById("confirmModal");
    if (modal) modal.style.display = "flex";
}


function updateExternalButton() {

    let hasNotGiven = false;

    document.querySelectorAll("#table .status-dropdown").forEach(s => {
        if (s.value !== "Given") hasNotGiven = true;
    });

    const btn = document.getElementById("externalBtn");
    if (btn) {
        btn.style.display = hasNotGiven ? "inline-block" : "none";
    }
}



function closeConfirm() {
    const modal = document.getElementById("confirmModal");
    if (modal) modal.style.display = "none";
}

function confirmSave() {

    const data = window._tempData;

    if (!data || data.length === 0) {
        showPopupError("No data to save");
        return;
    }

    closeConfirm();

    let hasNotGiven = false;

    document.querySelectorAll("#table .status-dropdown").forEach(s => {
        if (s.value !== "Given") hasNotGiven = true;
    });

    if (hasNotGiven) {
        generateExternalAndThenSave(data);
    } else {
        saveToDatabase(data);
    }
}

function generateExternalAndThenSave(data) {

    let meds = [];

    document.querySelectorAll("#table tr").forEach(row => {

        let select = row.querySelector(".status-dropdown");
        if (!select) return;

        let status = select.value;
        let reason = select.getAttribute("data-reason") || "";

        if (status !== "Given" && reason.trim()) {
            meds.push({
                medicineName: row.cells[0].innerText,
                dosage: row.cells[1].innerText,
                duration: row.cells[2].innerText,
                timesPerDay: parseInt(row.cells[3].innerText),
                reason
            });
        }
    });

    fetch('/Pharmacist/Prescription/GenerateExternal', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            patientName: document.getElementById("name").innerText,
            NIC: document.getElementById("nic").innerText,
            medicines: meds
        })
    })
        .then(res => res.blob())
        .then(blob => {

            let url = URL.createObjectURL(blob);
            let a = document.createElement("a");
            a.href = url;
            a.download = "ExternalPrescription.pdf";
            a.click();

            saveToDatabase(data);
        })
        .catch(err => {
            showPopupError("PDF failed: " + err.message);
        });
}



function saveToDatabase(data) {

    fetch('/Pharmacist/Prescription/Confirm', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
        .then(res => res.json())
        .then(res => {

            if (res.success) {

                if (currentPrescriptionId) {
                    window.confirmedPrescriptions.add(Number(currentPrescriptionId));

                    localStorage.setItem(
                        "confirmedPrescriptions",
                        JSON.stringify([...window.confirmedPrescriptions])
                    );
                }

                closeConfirm();
                closePopup();
            }
        })
        .catch(err => console.error("SAVE ERROR:", err));
}
