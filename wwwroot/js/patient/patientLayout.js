function updateTime() {

    const now = new Date();

    const formatted = now.toLocaleString('en-US', {
        month: 'short',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        hour12: true
    });

    const el = document.getElementById("currentDateTime");

    if (el) {
        el.textContent = formatted;
    }

}

// Run once immediately
updateTime();

// Update every minute
setInterval(updateTime, 60000);