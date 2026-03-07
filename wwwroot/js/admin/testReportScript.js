function searchPatient() {

    const nic = document.getElementById("nicInput").value;

    fetch('/Admin/TestReports/SearchPatient?nic=' + nic)

        .then(res => res.json())

        .then(data => {

            const resultDiv = document.getElementById("searchResult");

            if (!data.success) {

                resultDiv.innerHTML = `<p style="color:red;">Patient not found</p>`;
                return;
            }

            resultDiv.innerHTML = `
            <div class="patient-result">
                <p><strong>${data.name}</strong> (${data.nic})</p>

                <a href="/Admin/TestReports/Upload?nic=${data.nic}" class="upload-btn">
                    Upload Test Report
                </a>
            </div>
        `;
        });

}