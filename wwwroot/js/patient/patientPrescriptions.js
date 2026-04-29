<script>

    async function openConfirm(id) {

    const msg = document.getElementById("msgBox");

    msg.style.display = "block";
    msg.innerHTML = "Checking prescription...";

    const res = await fetch(`/Patient/Prescriptions/CheckExternal?id=${id}`);
    const data = await res.json();

    if (data.hasExternal) {
        alert("External prescription available. Please download it.");
    }

    confirmPrescription(id);
}

    async function confirmPrescription(id) {

    const res = await fetch('/Patient/Prescriptions/Confirm', {
        method: 'POST',
    headers: {'Content-Type': 'application/json' },
    body: JSON.stringify({id})
    });

    const data = await res.json();

    if (data.success) {
        alert("Confirmed!");
    location.reload();
    }
}

</script>