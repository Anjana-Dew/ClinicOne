let currentWeekOffset = 0;
let selectedSessionId = null;
let selectedDate = null;
let selectedContainer = null;

function getMonday(date) {
    const day = date.getDay();
    const diff = date.getDate() - day + (day === 0 ? -6 : 1);
    return new Date(date.setDate(diff));
}

function renderWeek() {

    const calendar = document.getElementById("weeklyCalendar");
    calendar.innerHTML = "";

    const baseMonday = getMonday(new Date());
    baseMonday.setDate(baseMonday.getDate() + currentWeekOffset * 7);

    const weekStart = new Date(baseMonday);
    const weekEnd = new Date(baseMonday);
    weekEnd.setDate(weekStart.getDate() + 6);

    document.getElementById("weekRangeText").innerText =
        weekStart.toDateString() + " - " + weekEnd.toDateString();

    const isPastWeek = currentWeekOffset < 0;

    for (let i = 0; i < 7; i++) {

        const dayDate = new Date(baseMonday);
        dayDate.setDate(baseMonday.getDate() + i);

        const dayCard = document.createElement("div");
        dayCard.className = "day-card";

        if (isPastWeek) {
            dayCard.classList.add("read-only");
        }

        dayCard.innerHTML = `
            <div class="day-header">
                <div class="day-name">
                    ${dayDate.toLocaleDateString('en-US', { weekday: 'long' })}
                </div>
                <div class="day-date">
                    ${dayDate.toLocaleDateString()}
                </div>
            </div>
        `;

        sessionsFromDb.forEach(session => {

            const sessionBox = document.createElement("div");
            sessionBox.className = "session-box";

            const doctorContainer = document.createElement("div");
            doctorContainer.className = "doctor-container";

            sessionBox.innerHTML += `<div class="session-title">${session.SessionName}</div>`;
            sessionBox.appendChild(doctorContainer);

            if (!isPastWeek) {
                const btn = document.createElement("button");
                btn.className = "add-doctor-btn";
                btn.innerText = "Add Doctors";

                btn.onclick = function () {
                    selectedSessionId = session.SessionID;
                    selectedDate = dayDate.toISOString();
                    selectedContainer = doctorContainer;
                    openDoctorModal();
                };

                sessionBox.appendChild(btn);
            }

            dayCard.appendChild(sessionBox);
        });

        calendar.appendChild(dayCard);
    }
}

function openDoctorModal() {
    document.getElementById("doctorModal").style.display = "flex";
}

function closeDoctorModal() {
    document.getElementById("doctorModal").style.display = "none";
}

function saveDoctorToSession() {

    const checked = document.querySelectorAll("#doctorModal input:checked");
    const doctorIds = [];

    checked.forEach(cb => doctorIds.push(parseInt(cb.value)));

    fetch('/Admin/DoctorDutySchedule/SaveDoctors', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            sessionId: selectedSessionId,
            clinicDate: selectedDate,
            doctorIds: doctorIds
        })
    })
        .then(res => res.json())
        .then(data => {

            if (data.success) {

                checked.forEach(cb => {

                    const name = cb.getAttribute("data-name");

                    const card = document.createElement("div");
                    card.className = "doctor-card";
                    card.innerHTML =
                        `${name} <span class="remove-doctor">×</span>`;

                    card.querySelector(".remove-doctor").onclick = function () {
                        card.remove();
                    };

                    selectedContainer.appendChild(card);
                    cb.checked = false;
                });

                closeDoctorModal();
            }
        });
}

document.getElementById("prevWeekBtn").onclick = function () {
    currentWeekOffset--;
    renderWeek();
};

document.getElementById("nextWeekBtn").onclick = function () {
    currentWeekOffset++;
    renderWeek();
};

renderWeek();