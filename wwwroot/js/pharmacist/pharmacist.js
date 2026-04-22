function generateExternal() {

    let medicines = [];

    document.querySelectorAll("#table tr").forEach(row => {

        let status = row.querySelector(".status")?.value;
        let reason = row.querySelector(".reason")?.value;

        if (status === "Not Given" || status === "Partially Given") {

            medicines.push({
                medicineName: row.cells[0].innerText,
                dosage: row.cells[1].innerText,
                duration: row.cells[2].innerText,
                timesPerDay: parseInt(row.cells[3].innerText || "0"),
                status: status,
                reason: reason || ""
            });
        }
    });

    if (medicines.length === 0) {
        alert("No medicines selected for external prescription!");
        return;
    }

    fetch('/Pharmacist/Prescription/GenerateExternal', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(medicines)
    })
        .then(res => {
            if (!res.ok) throw new Error("Server rejected request");
            return res.blob();
        })
        .then(blob => {
            let url = window.URL.createObjectURL(blob);
            let a = document.createElement('a');
            a.href = url;
            a.download = "ExternalPrescription.pdf";
            a.click();
        })
        .catch(err => {
            console.error(err);
            alert("Error generating PDF");
        });
}

window.searchPatient = function () {

    let nic = document.getElementById("nicInput").value;

    fetch(`/Pharmacist/Prescription/Search?nic=${nic}`)
        .then(res => res.json())
        .then(data => {

            if (!data.success) {
                document.getElementById("msg").innerText = data.message;
                return;
            }

            document.getElementById("popup").style.display = "block";
            document.getElementById("name").innerText = data.patientName;
            document.getElementById("nic").innerText = data.patientNIC;

            let table = document.getElementById("table");
            table.innerHTML = "";

            data.medicines.forEach(m => {
                table.innerHTML += `
<tr data-id="${m.prescMedID}">
    <td>${m.medicineName}</td>
    <td>${m.dosage}</td>
    <td>${m.duration}</td>
    <td>${m.timesPerDay}</td>
    <td>
        <select class="status">
            <option ${m.status === "Given" ? "selected" : ""}>Given</option>
            <option ${m.status === "Not Given" ? "selected" : ""}>Not Given</option>
            <option ${m.status === "Partially Given" ? "selected" : ""}>Partially Given</option>
        </select>
    </td>
    <td><input class="reason" value="${m.reason || ""}" /></td>
</tr>
`;
            });
        })
        .catch(err => console.error(err));
};

function saveData() {

    let data = [];

    document.querySelectorAll("#table tr").forEach(row => {

        let id = row.getAttribute("data-id");

        let status = row.querySelector(".status")?.value;
        let reason = row.querySelector(".reason")?.value;

        if (!id) return;

        data.push({
            prescMedID: parseInt(id),
            status: status,
            reason: reason || ""
        });
    });

    if (data.length === 0) {
        alert("Nothing to save");
        return;
    }

    fetch('/Pharmacist/Prescription/Confirm', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
        .then(res => res.json())
        .then(res => {
            if (res.success) {
                alert("Saved successfully");
            } else {
                alert("Save failed");
            }
        })
        .catch(err => {
            console.error(err);
            alert("Error saving");
        });
}