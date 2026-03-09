function closePasswordModal() {

    document.getElementById("passwordModal").style.display = "none";
    document.querySelector(".main-content").classList.remove("blur");

    fetch('/Patient/Dashboard/DisableFirstLogin');

}
document.addEventListener("DOMContentLoaded", function () {

    console.log("Patient Dashboard Loaded");

    const elements = document.querySelectorAll(".slide-up");

    elements.forEach((el, index) => {

        el.style.opacity = 0;
        el.style.transform = "translateY(20px)";

        setTimeout(() => {
            el.style.transition = "0.6s";
            el.style.opacity = 1;
            el.style.transform = "translateY(0)";
        }, 200 * index)

    });

});