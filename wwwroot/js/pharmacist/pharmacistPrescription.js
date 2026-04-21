function searchPatient() {

    let nic = document.getElementById("nicInput").value;
    let validation = document.getElementById("searchValidation");

    fetch(`/Pharmacist/Prescription/GetPrescriptionByNIC?nic=${nic}`)
        .then(res => res.json())
        .then(data => {

            console.log(data); // DEBUG

            // ❌ HANDLE ERROR RESPONSE FIRST
            if (!data.success) {
                validation.innerText = data.message;
                document.getElementById("prescriptionPopup").style.display = "none";
                return;
            }

            validation.innerText = "";

            // ✅ SHOW POPUP ONLY WHEN VALID
            document.getElementById("prescriptionPopup").style.display = "flex";

            document.getElementById("popupPatientName").innerText = data.patientName;
            document.getElementById("popupPatientNIC").innerText = data.patientNIC;
            document.getElementById("popupPrescriptionID").innerText = data.prescriptionID;

            let rows = "";

            if (!data.medicines || data.medicines.length === 0) {
                rows = `<tr><td colspan="4">No medicines found</td></tr>`;
            } else {
                data.medicines.forEach(m => {
                    rows += `
                        <tr>
                            <td>${m.medicineName}</td>
                            <td>${m.dosage}</td>
                            <td>${m.status}</td>
                            <td>${m.reason}</td>
                        </tr>
                    `;
                });
            }

            document.getElementById("medicineRows").innerHTML = rows;
            document.getElementById("medicineSection").style.display = "block";

        })
        .catch(err => {
            console.log("ERROR:", err);
            validation.innerText = "Server error occurred";
        });
}