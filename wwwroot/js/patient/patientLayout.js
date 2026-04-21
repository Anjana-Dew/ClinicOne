function updateDateTime() {
    const now = new Date();

    const options = {
        weekday: 'short',
        day: '2-digit',
        month: 'short'
    };

    document.getElementById("currentDateTime")
        .innerText = now.toLocaleDateString('en-US', options);
}

updateDateTime();
setInterval(updateDateTime, 60000);