const searchInput = document.getElementById("searchInput");
const dateFilter = document.getElementById("dateFilter");

function filterReports() {
    const filterText = searchInput.value.toLowerCase();
    const filterDate = dateFilter.value;
    const rows = document.querySelectorAll("#reportTable tbody tr");

    rows.forEach(row => {
        const testNameCell = row.cells[1];
        const dateCell = row.cells[5];

        const testName = testNameCell ? testNameCell.innerText.toLowerCase() : "";
        const reportDate = dateCell ? dateCell.innerText : "";

        const rowDate = new Date(reportDate);
        const formattedRowDate = rowDate.toISOString().split("T")[0];

        const matchesText = testName.includes(filterText);
        const matchesDate = filterDate ? (formattedRowDate === filterDate) : true;

        row.style.display = (matchesText && matchesDate) ? "" : "none";
    });
}

searchInput.addEventListener("keyup", filterReports);
dateFilter.addEventListener("change", filterReports);