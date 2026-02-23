function closeAlert() {
    document.getElementById("successAlert").style.display = "none";
}

//// cancel-btn
//document.querySelector(".cancel-btn").addEventListener("click", function () {

//    // clear validation summary
//    const summary = document.querySelector(".validation-summary");
//    if (summary) {
//        summary.innerHTML = "";
//        summary.classList.remove("validation-summary-errors");
//    }

//    // clear validations next to inputs
//    document.querySelectorAll(".field-validation-error").forEach(function (el) {
//        el.textContent = "";
//        el.classList.remove("fiels-validation-error");
//    })

//    document.querySelectorAll(".input-validation-error").forEach(function (el) {
//        el.classList.remove("input-validation-error");

//    });
//});
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


// search patients

function searchPatient() {

    const nic = document.getElementById("searchNIC").value.trim();
    const resultDiv = document.getElementById("searchResult");

    if (!nic) {
        resultDiv.innerHTML = `<p class="error-msg">Please Enter a NIC.</p>`;
        return;
    }

    fetch(`/Admin/Patients/SearchPatient?nic=${nic}`)
        .then(response => response.json())
        .then(data => {

            if (!data.success) {
                resultDiv.innerHTML = `<p class = "error-msg">${data.message}</p>`;
                return;
            }

            const actionButton = data.isActive ?
                `<button class="deactivate-btn" onclick="deactivatePatient('${data.nic}')">
                    Deactivate
                </button>`
                : `<button class="activate-btn" onclick = "activatePatient('${data.nic}')">
                    Activate
                 </button>`;
            resultDiv.innerHTML = `
                <div class="patient-card">
                    <div class="patient-info">
                        <span>
                            ${data.fullName}
                            <small>(${data.isActive ? "Active" : "Inactive"})</small>
                        </span>
                    </div>

                    <div class="patient-actions">
                        ${actionButton}
                        <button class="close-result-btn" onclick="clearSearchResult()">×</button>
                    </div>
                </div>
            `;
        });
}

// Deactivate Patients

function deactivatePatient(nic) {

    fetch(`/Admin/Patients/DeactivatePatient?nic=${nic}`, { method: "POST" })
        .then(response => response.json())
        .then(data => {

            if (data.success) {
                //document.getElementById("searchResult").innerHTML = `
                //    <p class= "success-msg"> Patient deactivate successfully. </p>`;
                searchPatient();
            }
            else {
                document.getElementById("searchResult").innerHTML =
                    `<p class="error-msg"> Something went wrong.</p>`
            }
        });
}

// Activate patients

function activatePatient(nic) {
    fetch(`/Admin/Patients/ActivatePatient?nic=${nic}`, { method: "POST" })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                searchPatient();
            }
            else {
                document.getElementById("searchResult").innerHTML = `<p class="error-msg">Something went Wrong. </p>`;
            }
        });
}

// clear serach results

function clearSearchResult() {
    document.getElementById("searchResult").innerHTML = "";
    document.getElementById("searchNIC").value = "";
}

