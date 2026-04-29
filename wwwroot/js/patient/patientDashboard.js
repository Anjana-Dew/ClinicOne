function openModal() {
    document.getElementById("profileModal").style.display = "flex";

    const messageBox = document.getElementById("formMessage");
    messageBox.innerHTML = "";
    messageBox.className = "form-message";
}
function closeModal() {
    document.getElementById("profileModal").style.display = "none";

    const form = document.getElementById("profileForm");

    form.reset();

    const messageBox = document.getElementById("formMessage");
    messageBox.innerHTML = "";
    messageBox.className = "form-message";
}

document.addEventListener("DOMContentLoaded", function () {

    const form = document.getElementById("profileForm");

    form.addEventListener("submit", async function (e) {
        e.preventDefault();

        const messageBox = document.getElementById("formMessage");

        messageBox.innerHTML = "Saving...";
        messageBox.className = "form-message";

        try {
            const response = await fetch(form.action, {
                method: "POST",
                body: new FormData(form),
                credentials: "same-origin"
            });

            const text = await response.text();
            console.log("SERVER RESPONSE:", text);

            let result;

            try {
                result = JSON.parse(text);
            } catch {
                messageBox.innerHTML = "ERROR: Not JSON (wrong endpoint hit)";
                return;
            }

            if (result.success) {

                messageBox.innerHTML = result.message;
                messageBox.classList.add("success");

                document.getElementById("profileName").innerText = result.data.fullName;
                document.getElementById("profilePhone").innerText = result.data.phoneNumber;
                document.getElementById("profileAddress").innerText = result.data.address;

                document.getElementById("fullName").value = result.data.fullName;
                document.getElementById("phoneNumber").value = result.data.phoneNumber;
                document.getElementById("address").value = result.data.address;

                setTimeout(() => {
                    closeModal();
                }, 1000);                    

            } else {
                messageBox.innerHTML = result.message;
                messageBox.classList.add("error");
            }

        } catch (err) {
            console.error(err);
            messageBox.innerHTML = "Server error";
        }
    });
});

document.addEventListener("DOMContentLoaded", function () {

    const countdown = document.getElementById("countdown");

    if (!countdown) return;

    const date = countdown.dataset.date;
    const time = countdown.dataset.time;

    if (!date || !time) {
        countdown.innerHTML = "No session data";
        return;
    }

    const target = new Date(date + " " + time).getTime();

    setInterval(function () {
        const now = new Date().getTime();
        const diff = target - now;

        if (diff <= 0) {
            countdown.innerHTML = "Session started";
            return;
        }

        const h = Math.floor(diff / (1000 * 60 * 60));
        const m = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
        const s = Math.floor((diff % (1000 * 60)) / 1000);

        countdown.innerHTML = ` ${h}h ${m}m ${s}s`;
    }, 1000);
});
