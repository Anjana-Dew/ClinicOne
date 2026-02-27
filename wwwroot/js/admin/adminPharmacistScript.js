function closeAlert() {
    document.getElementById("successAlert").style.display = "none";
}
// Clear register
//function clearForm() {
//    document.querySelector("form").reset();
//}
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

// search pharmacists
function searchPharmacist() {
    const regNo = document.getElementById("searchRegNo").value.trim();
    const resultDiv = document.getElementById("searchResult");

    if (!regNo) {
        resultDiv.innerHTML = `<p class="error-msg"> Please enter registration number. </p>`;
        return;
    }

    fetch(`/Admin/Pharmacists/SearchPharmacist?regNo=${regNo}`)
        .then(response => response.json())
        .then(data => {
            if (!data.success) {
                resultDiv.innerHTML = `<p class="error-msg">${data.message}</P>`;
                return;
            }

            const actionButton = data.isActive ?
                `<button class="deactivate-btn" onclick="deactivatePharmacist('${data.regNo}')">
                    Deactivate
                </button>`
                : `<button class="activate-btn" onclick="activatePharmacist('${data.regNo}')">
                        Activate
                </button>`;

            resultDiv.innerHTML = `
                <div class="entity-card">
                    <div class="entity-info">
                        <span>
                            ${data.name}
                            <small>(${data.isActive ? "Active" : "Inactive"})</small>
                        </span>
                    </div>
                    <div class="entity-actions">
                        ${actionButton}
                            <button class="close-result-btn" onclick="clearSearchResult()">×</button>
                    </div>
                </div>
            `;
        })
}

// clear search results
function clearSearchResult() {
    document.getElementById("searchResult").innerHTML = "";
    document.getElementById("searchRegNo").value = "";
}

// deactivate patients
function deactivatePharmacist(regNo) {
    fetch(`/Admin/Pharmacists/DeactivatePharmacist?regNo=${regNo}`, { method: "POST" })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                searchPharmacist();
            }
        })
}

//activate patients
function activatePharmacist(regNo) {
    fetch(`/Admin/Pharmacists/ActivatePharmacist?regNo=${regNo}`,
        { method: "POST" })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                searchPharmacist();
            }
        });
}