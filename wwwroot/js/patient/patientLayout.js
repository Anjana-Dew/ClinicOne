function updateTime() {
    const now = new Date();

    const options = {
        year: 'numeric',
        month: 'short',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
    };

    document.getElementById("currentDateTime").textContent =
        now.toLocaleString('en-US', options);
}

setInterval(updateTime, 1000);
updateTime();