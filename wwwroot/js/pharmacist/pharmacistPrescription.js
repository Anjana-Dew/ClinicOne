/* =========================
   SEARCH PATIENT
========================= */
function searchPatient() {
    let nic = document.getElementById("nicInput").value.trim();
    const validationDiv = document.getElementById("searchValidation");

    // Clear previous validation message
    validationDiv.style.display = "none";
    validationDiv.innerText = "";

    // Define NIC regex patterns
    const oldNICRegex = /^[0-9]{9}[vVxX]$/;             // e.g., 903051234V
    const newNICRegex = /^[0-9]{12}$/;                  // e.g., 199030500123

    if (!nic) {
        validationDiv.innerText = "Please enter Patient NIC";
        validationDiv.style.display = "block";
        return;
    }

    if (!oldNICRegex.test(nic) && !newNICRegex.test(nic)) {
        validationDiv.innerText = "Invalid NIC format ";
        validationDiv.style.display = "block";
        return;
    }

    // If valid, clear validation and proceed to fetch data
    validationDiv.style.display = "none";
    validationDiv.innerText = "";

    fetch('/Pharmacist/Dashboard/Search?nic=' + encodeURIComponent(nic))
        .then(r => r.json())
        .then(data => {
            // Populate patient info
            document.getElementById("popupPatientName").innerText = data.patientName || "Unknown";
            document.getElementById("popupPatientNIC").innerText = data.patientNIC || "N/A";
            document.getElementById("popupPrescriptionID").innerText = data.prescriptionID || "N/A";

            let tbody = document.getElementById("medicineRows");
            tbody.innerHTML = "";

            if (!data.success) {
                // No prescription found
                document.getElementById("medicineSection").style.display = "none";
                showMessage(data.message || "No prescription found", "error");
            } else if (!data.medicines || data.medicines.length === 0) {
                // Prescription exists but no medicines
                tbody.innerHTML = `<tr>
                    <td colspan="4" style="text-align:center; color:#555;">No medicines found</td>
                </tr>`;
                document.getElementById("medicineSection").style.display = "block";
                toggleExternalBtn(false);
                showMessage("Prescription found but no medicines listed", "error");
            } else {
                // Prescription with medicines
                let showGenerateBtn = false;
                data.medicines.forEach(m => {
                    let status = m.Status || "Not Given";
                    if (status === "Not Given") showGenerateBtn = true;

                    document.getElementById("confirmBtn").style.display = showGenerateBtn ? "inline-block" : "none";

                    let row = `
<tr data-id="${m.PrescMedID}">
<td>${m.MedicineName}</td>
<td>${m.Dosage ?? "-"}</td>
<td>
<select class="status-dropdown" onchange="statusChanged(this)">
<option value="Given" ${status === "Given" ? "selected" : ""}>Given</option>
<option value="Not Given" ${status === "Not Given" ? "selected" : ""}>Not Given</option>
<option value="Partially Given" ${status === "Partially Given" ? "selected" : ""}>Partially Given</option>
</select>
</td>
<td>
<input type="text" class="reason-input" placeholder="Reason" disabled>
</td>
</tr>`;
                    tbody.innerHTML += row;
                });

                document.getElementById("medicineSection").style.display = "block";
                toggleExternalBtn(showGenerateBtn);
            }

            // Show popup
            document.getElementById("prescriptionPopup").style.display = "flex";
        })
        .catch(() => {
            showMessage("Error loading prescription data", "error");
            document.getElementById("prescriptionPopup").style.display = "flex";
        });
}
/* =========================
   STATUS CHANGE
========================= */
function statusChanged(select) {

    let row = select.closest("tr");
    let reasonInput = row.querySelector(".reason-input");

    if (select.value === "Not Given") {
        reasonInput.disabled = false;
    } else {
        reasonInput.disabled = true;
        reasonInput.value = "";
    }

    let anyNotGiven = Array.from(document.querySelectorAll(".status-dropdown"))
        .some(s => s.value === "Not Given");

    toggleExternalBtn(anyNotGiven);
}


/* =========================
   CONFIRM DISPENSE BUTTON
========================= */
function confirmDispense() {
    document.getElementById("confirmPopup").style.display = "flex";
}


/* =========================
   SUBMIT DISPENSE
========================= */
function submitDispense() {

    let medicines = [];
    let validationError = false;

    document.querySelectorAll("#medicineRows tr").forEach(row => {

        let id = row.dataset.id;
        let status = row.querySelector(".status-dropdown").value;
        let reasonInput = row.querySelector(".reason-input");
        let reason = reasonInput.value.trim();

        if (status === "Not Given" && reason === "") {
            reasonInput.style.border = "2px solid red";
            validationError = true;
        } else {
            reasonInput.style.border = "";
        }

        medicines.push({
            PrescMedID: id,
            Status: status,
            Reason: reason
        });

    });

    if (validationError) {
        showMessage("Reason required for Not Given medicines", "error");
        return;
    }

    fetch("/Pharmacist/Dashboard/ConfirmPrescription", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(medicines)
    })
        .then(r => r.json())
        .then(res => {
            if (res.success) {
                showMessage("Prescription confirmed successfully", "success");
                closeConfirm();
                closePopup();
            } else {
                showMessage("Error saving prescription", "error");
            }
        });
}


/* =========================
   CLOSE CONFIRM POPUP
========================= */
function closeConfirm() {
    document.getElementById("confirmPopup").style.display = "none";
}


/* =========================
   GENERATE EXTERNAL
========================= */
function generateExternal() {

    let anyNotGiven = Array.from(document.querySelectorAll(".status-dropdown"))
        .some(s => s.value === "Not Given");

    if (!anyNotGiven) {
        showMessage("No medicines require external prescription.", "error");
        return;
    }

    let nic = document.getElementById("popupPatientNIC").innerText;
    window.open(`/Pharmacist/Prescription/GenerateExternalPrescription?nic=${encodeURIComponent(nic)}`);
}


/* =========================
   TOGGLE EXTERNAL BUTTON
========================= */
function toggleExternalBtn(show) {
    let btn = document.getElementById("generateExternalBtn");
    if (btn) {
        btn.style.display = show ? "inline-block" : "none";
    }
}


/* =========================
   CLOSE POPUP
========================= */
function closePopup() {
    document.getElementById("prescriptionPopup").style.display = "none";
    document.getElementById("popupMessage").style.display = "none";
    document.getElementById("medicineRows").innerHTML = "";
}


/* =========================
   UI MESSAGE
========================= */
function showMessage(msg, type) {
    let box = document.getElementById("popupMessage");
    box.innerText = msg;
    box.className = "popup-message " +
        (type === "success" ? "popup-success" : "popup-error");
    box.style.display = "block";
}