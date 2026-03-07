function closeAlert() {
    document.getElementById("successAlert").style.display = "none";
}

// search admins
function searchAdmin() {

    const email = document.getElementById("searchEmail").value.trim();
    const resultDiv = document.getElementById("searchResult");

    if (!email) {
        resultDiv.innerHTML = `<p class="error-msg">Please enter an email.</p>`;
        return;
    }

    fetch(`/Admin/Admins/Search?email=${email}`)
        .then(response => response.json())
        .then(data => {

            if (!data) {
                resultDiv.innerHTML = `<p class="error-msg">No admin found.</p>`;
                return;
            }

            const actionButton = data.isActive
                ? `<button class="deactivate-btn" onclick="deactivateAdmin('${data.email}')">
                        Deactivate
                   </button>`
                : `<button class="activate-btn" onclick="activateAdmin('${data.email}')">
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
        });
}

// clear search
function clearSearchResult() {
    document.getElementById("searchResult").innerHTML = "";
    document.getElementById("searchEmail").value = "";
}

// deactivate admin
function deactivateAdmin(email) {
    fetch(`/Admin/Admins/DeactivateAdmin?email=${email}`, { method: "POST" })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                searchAdmin();
            }
        });
}

// activate admin
function activateAdmin(email) {
    fetch(`/Admin/Admins/ActivateAdmin?email=${email}`, { method: "POST" })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                searchAdmin();
            }
        });
}