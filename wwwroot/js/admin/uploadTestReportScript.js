document.getElementById("panelSelect").addEventListener("change", function () {

    const panelId = this.value;

    fetch('/Admin/TestReports/GetParameters?panelId=' + panelId)

        .then(res => res.json())
        .then(data => {
            const container = document.getElementById("parametersContainer");

            container.innerHTML = "";

            data.forEach(p => {
                const row = document.createElement("div");

                row.className = "parameter-row";

                row.innerHTML = `
                    <label>${p.parameterName} (${p.unit})</label>
                    <input type="number" step="0.01" data-id="${p.parameterID}">
                `;
                container.appendChild(row);
            });
        });
});

function saveReport() {
    const panelID = document.getElementById("panelSelect").value;
    const inputs = document.querySelectorAll("#parametersContainer input");
    const pdfFile = document.getElementById("pdfFile").files[0];

    let hasValue = false;

    inputs.forEach(i => {
        if (i.value !== "") {
            hasValue = true;
        }
    });

    if (panelID === "") {
        showMessage("Please select a test panel.", "error");
        return;
    }

    if (!hasValue) {
        showMessage("Please enter at lease one test value.", "error");
        return;
    }
    if (!pdfFile) {
        showMessage("Please upload the PDF test report.", "error");
        return;
    }
    const modal = document.getElementById("confirmModal");
    modal.style.display = "flex";
       
}

// back btn (confirmation modal)
document.getElementById("cancelSave").addEventListener("click", function () {
    document.getElementById("confirmModal").style.display = "none";
});

//continue btn (confirmation modal)
document.getElementById("confirmSave").addEventListener("click", function () {
    document.getElementById("confirmModal").style.display = "none";
    submitReport();
})

function submitReport() {

    const patientNIC = document.getElementById("patientNIC").value;
    const panelID = document.getElementById("panelSelect").value;

    const inputs = document.querySelectorAll("#parametersContainer input");

    let testValues = {};

    inputs.forEach(i => {
        if (i.value !== "") {
            testValues[i.dataset.id] = parseFloat(i.value);
        }
    });

    if (Object.keys(testValues).length === 0) {
        showMessage("Please enter at least one test value.", "error");
        return;
    }

    const formData = new FormData();

    formData.append("PatientNIC", patientNIC);
    formData.append("PanelID", panelID);

    formData.append("pdfFile", document.getElementById("pdfFile").files[0]);

    for (let key in testValues) {
        formData.append(`TestValues[${key}]`, testValues[key]);
    }

    fetch('/Admin/TestReports/SaveReport', {
        method: 'POST',
        body: formData
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                showMessage("Report saved successfully.", "success");

                setTimeout(() => {
                    window.location.href = "/Admin/TestReports"

                }, 1000);
            }
        });
}
function showMessage(message, type) {
    const box = document.getElementById("validationMessage");

    box.className = "validation-message";
    box.classList.add(type === "success" ? "validation-success" : "validation-error");

    box.innerText = message;
    box.style.display = "block";

    setTimeout(() => {
        box.style.display = "none";
    }, 2000);
}