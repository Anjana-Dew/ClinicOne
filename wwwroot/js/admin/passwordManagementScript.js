document.addEventListener("DOMContentLoaded", function () {

    const openBtn = document.getElementById("openConfirm");
    const modal = document.getElementById("confirmModal");

    const cancelBtn = document.getElementById("cancelReset");
    const confirmBtn = document.getElementById("confirmReset");

    const form = document.getElementById("resetForm");

    const usernameInput = form.querySelector('input[name="username"]');
    const passwordInput = form.querySelector('input[name="newPassword"]');

    openBtn.addEventListener("click", function () {

        // let HTML validation run first
        if (!form.checkValidity()) {
            form.reportValidity();
            return;
        }

        // custom validation
        if (passwordInput.value.length < 8) {
            alert("Password must be at least 8 characters.");
            return;
        }

        // show modal only if valid
        modal.style.display = "flex";
    });

    cancelBtn.addEventListener("click", function () {
        modal.style.display = "none";
    });

    confirmBtn.addEventListener("click", function () {
        form.submit();
    });

    window.addEventListener("click", function (e) {
        if (e.target === modal) {
            modal.style.display = "none";
        }
    });

});