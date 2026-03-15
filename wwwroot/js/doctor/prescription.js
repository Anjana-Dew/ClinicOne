let medicineIndex = 1;
let testIndex = 1;

function addMedicineRow() {

    let table = document.querySelector("#MedicineTable tbody");

    let row = document.createElement("tr");

    row.innerHTML = `
        <td><input name="Medicines[${medicineIndex}].MedicineName"/></td>
        <td><input name="Medicines[${medicineIndex}].Dosage"/></td>
        <td><input type="number" name="Medicines[${medicineIndex}].TimesPerDay" min="1"/></td>
        <td><button type="button" onclick="removeMedicineRow(this)">X</button></td>
    `;

    table.appendChild(row);

    medicineIndex++;
}
function validateMedicines() {

    let rows = document.querySelectorAll("#MedicineTable tbody tr");

    for (let row of rows) {

        let name = row.querySelector("[name*='MedicineName']").value.trim();
        let dosage = row.querySelector("[name*='Dosage']").value.trim();
        let times = row.querySelector("[name*='TimesPerDay']").value.trim();

        if (name && (!dosage || !times)) {
            showTempMessage("Dosage and Times/Day required when medicine name is entered.");
            return false;
        }
        if (!name && (dosage || times)) {
            showTempMessage("Enter medicine name before dosage or times/day.");
            return false;
        }
    }
    let hasMedicine = false;

    rows.forEach(row => {
        let name = row.querySelector("[name*='MedicineName']").value.trim();
        if (name) hasMedicine = true;
    });

    if (!hasMedicine) {
        return true; 
    }
    return true;
}
function removeMedicineRow(btn) {

    let rows = document.querySelectorAll("#MedicineTable tbody tr");

    if (rows.length === 1) {
        showTempMessage("At least one medicine row is required.");
        return;
    }

    btn.closest("tr").remove();
}


function confirmSave() {
    if (!validateMedicines()) {
        return;
    }
    document.getElementById("confirmPopup").style.display = "flex";
}

function closeConfirm() {
    document.getElementById("confirmPopup").style.display = "none";
}

function submitPrescription() {

    let medRows = document.querySelectorAll("#MedicineTable tbody tr");

    medRows.forEach((row, index) => {

        row.querySelector("[name*='MedicineName']").name = `Medicines[${index}].MedicineName`;
        row.querySelector("[name*='Dosage']").name = `Medicines[${index}].Dosage`;
        row.querySelector("[name*='TimesPerDay']").name = `Medicines[${index}].TimesPerDay`;

    });

    let testRows = document.querySelectorAll("#testTable tbody tr");

    testRows.forEach((row, index) => {

        row.querySelector("select").name = `Tests[${index}].PanelID`;
        row.querySelector("input").name = `Tests[${index}].Notes`;

    });
    document.querySelector("form").submit();
}

function addTestRow() {

    let table = document.querySelector("#testTable tbody");

    let template = document.querySelector("#testTable select").innerHTML;

    let row = document.createElement("tr");

    row.innerHTML = `
                    <td>
                        <select name="Tests[${testIndex}].PanelID" onchange="updateTestDropdowns()">
                            ${template}
                        </select>
                    </td>

                    <td>
                        <input name="Tests[${testIndex}].Notes" />
                    </td>

                    <td>
                        <button type="button" onclick="removeTestRow(this)">X</button>
                    </td>
                  `;

    table.appendChild(row);

    testIndex++;

    updateTestDropdowns();

}
function removeTestRow(btn) {

    let rows = document.querySelectorAll("#testTable tbody tr");

    if (rows.length === 1) {
        showTempMessage("At least one test row must remain.");
        return;
    }

    btn.closest("tr").remove();
}
function updateTestDropdowns() {

    let selects = document.querySelectorAll("#testTable select");

    let selectedValues = [];

    selects.forEach(s => {
        if (s.value !== "") {
            selectedValues.push(s.value);
        }
    });

    // reset all options first
    selects.forEach(select => {
        Array.from(select.options).forEach(option => {
            option.disabled = false;
        });
    });

    // then disable duplicates
    selects.forEach(select => {

        let currentValue = select.value;

        Array.from(select.options).forEach(option => {

            if (option.value === "")
                return;

            if (selectedValues.includes(option.value) && option.value !== currentValue) {
                option.disabled = true;
            }

        });

    });

}
function showTempMessage(message) {

    let msg = document.getElementById("tempMessage");

    msg.innerHTML = message;
    msg.style.display = "block";

    setTimeout(() => {
        msg.style.display = "none";
    }, 3000);
}

