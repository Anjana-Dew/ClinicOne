window.searchPatient = function () {

    let nic = document.getElementById("nicInput").value;

    fetch(`/Pharmacist/Prescription/Search?nic=${nic}`)
        .then(r => r.json())
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
                                <option>Given</option>
                                <option>Not Given</option>
                                <option>Partially Given</option>
                            </select>
                        </td>
                        <td><input class="reason" /></td>
                    </tr>
                `;
            });
        });
};

function saveData() {

    let data = [];

    document.querySelectorAll("#table tr").forEach(row => {

        data.push({
            prescMedID: parseInt(row.getAttribute("data-id")),
            status: row.querySelector(".status").value,
            reason: row.querySelector(".reason").value
        });
    });

    fetch('/Pharmacist/Prescription/Confirm', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
        .then(r => r.json())
        .then(r => alert(r.success ? "Saved" : "Failed"));
}

function generateExternal() {

    let meds = [];
    let patientName = document.getElementById("name").innerText;
    let nic = document.getElementById("nic").innerText;

    document.querySelectorAll("#table tr").forEach(row => {

        let status = row.querySelector(".status").value;

        if (status !== "Given") {
            meds.push({
                medicineName: row.cells[0].innerText,
                dosage: row.cells[1].innerText,
                duration: row.cells[2].innerText,
                timesPerDay: parseInt(row.cells[3].innerText),
                status: status,
                reason: row.querySelector(".reason").value
            });
        }
    });

    fetch('/Pharmacist/Prescription/GenerateExternal', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            patientName: patientName,
            nic: nic,
            medicines: meds
        })
    })
        .then(r => r.blob())
        .then(b => {
            let url = URL.createObjectURL(b);
            let a = document.createElement("a");
            a.href = url;
            a.download = "External.pdf";
            a.click();
        });
}