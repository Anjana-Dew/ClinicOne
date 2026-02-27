
// close btn
function closeAlert() {
    document.getElementById("successAlert").style.display = "none";
}

//cancel btn
document.querySelector(".cancel-btn").addEventListener("click", function () {

    const form = document.querySelector("form");

    // Clear all inputs manually
    form.querySelectorAll("input").forEach(input => {
        input.value = "";
    });

    form.querySelectorAll("select").forEach(select => {
        select.selectedIndex = 0;
    });

    // Clear validation
    if (window.jQuery) {
        $(form).validate().resetForm();
    }

});
function searchDoctor(){
    const regNo = document.getElementById("searchRegNo").value.trim();
    const resultDiv = document.getElementById("searchResult");

    if (!regNo) {
        resultDiv.innerHTML = `<p class="error-msg"> Enter registration number</p>`;
        return;

    }

    fetch(`/Admin/Doctors/SearchDoctor?regNo=${regNo}`)
        .then(response => response.json())
        .then(data => {

            if (!data.success) {
                resultDiv.innerHTML = `<p class="error-msg"> ${data.message}</p>`;
                return;
            }

            const updateButton = `<button class="update-btn" onclick="openUpdateModel('${data.regNo}', '${data.name}','${data.specialization}')">
                                    Update
                                  </button>`
            const actionButton = data.isActive ?
                `<button class="deactivate-btn" onclick="deactivateDoctor('${data.regNo}')">Deactivate</button>`
                :
                `<button class="activate-btn" onclick="activateDoctor('${data.regNo}')"> Activate </button>`;

            resultDiv.innerHTML = `
            <div class="entity-card">
                <div class="entity-info">
                    <span class="doctor-name">${data.name}</span>
                    <span class="doctor-specialization">${data.specialization}</span>
                    <span class="doctor-status">${data.isActive ? "Active" : "Inactive"}</span>
                </div>
                <div class="entity-actions">
                        ${updateButton}
                        ${actionButton}
                    <button class="close-result-btn" onclick="clearSearchResult()">×</button>
                </div>
            </div>`;
        });
}
// clear search results
function clearSearchResult() {
    document.getElementById("searchResult").innerHTML = "";
    document.getElementById("searchRegNo").value = "";
}
function deactivateDoctor(regNo) {
    fetch(`/Admin/Doctors/DeactivateDoctor?regNo=${regNo}`, { method: "POST" })
        .then(response => response.json())
        .then(data => {
            if (data.success) searchDoctor();
        });
}

function activateDoctor(regNo) {
    fetch(`/Admin/Doctors/ActivateDoctor?regNo=${regNo}`, { method: "POST" })
        .then(response => response.json())
        .then(data => {
            if (data.success) searchDoctor();
        });
}


// update function

function openUpdateModel(regNo, name, specialization) {
    document.getElementById("updateRegNo").value = regNo;
    document.getElementById("updateName").value = name;
    document.getElementById("updateSpecialization").value = specialization;
    document.getElementById("updateModal").style.display = "flex";
}

function closeModal() {
    document.getElementById("updateModal").style.display = "none";
}

function closeModal() {
    document.getElementById("updateModal").style.display = "none";
}

function updateDoctor() {
    const regNo = document.getElementById("updateRegNo").value;
    const name = document.getElementById("updateName").value;
    const specialization = document.getElementById("updateSpecialization").value;

    fetch(`/Admin/Doctors/UpdateDoctor`, {
        method: "POST", headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            regNo: regNo,
            name: name,
            specialization: specialization
        })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                closeModal();
                searchDoctor();
            }
        });
}