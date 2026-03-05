let currentWeekOffset = 0;
let selectedSessionId = null;
let selectedDate = null;
let selectedContainer = null;

function getMonday(date) {
    const d = new Date(date);
    const day = d.getDay();
    const diff = d.getDate() - day + (day === 0 ? -6 : 1);
    d.setDate(diff);
    return d;
}
function parseTimeToMinutes(timeString) {
    const parts = timeString.split(':');
    return parseInt(parts[0] * 60 ) + parseInt(parts[1]);
}
function renderWeek() {

    const calendar = document.getElementById("weeklyCalendar");
    calendar.innerHTML = "";

    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const baseMonday = getMonday(today);
    baseMonday.setDate(baseMonday.getDate() + currentWeekOffset * 7);

    const weekStart = new Date(baseMonday);
    const weekEnd = new Date(baseMonday);
    weekEnd.setDate(weekStart.getDate() + 6);

    document.getElementById("weekRangeText").innerText =
        weekStart.toDateString() + " - " + weekEnd.toDateString();

    const isPastWeek = currentWeekOffset < 0;

    for (let i = 0; i < 7; i++) {
        // get date
        const dayDate = new Date(baseMonday);
        dayDate.setDate(baseMonday.getDate() + i);
        dayDate.setHours(0, 0, 0, 0);

        //get current time


        const year = dayDate.getFullYear();
        const month = String(dayDate.getMonth() + 1).padStart(2, '0');
        const day = String(dayDate.getDate()).padStart(2, '0');

        const formattedDate = `${year}-${month}-${day}`;

        const isPastDay = dayDate < today;
        const isReadOnly = isPastWeek || isPastDay;

        const dayCard = document.createElement("div");
        dayCard.className = "day-card";

        if (isReadOnly) {
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

            const now = new Date();
            const currentTime = now.getHours() * 60 + now.getMinutes();

            const isToday = dayDate.toDateString() === new Date().toDateString();

            const sessionStart = parseTimeToMinutes(session.StartTime);
            const sessionEnd = parseTimeToMinutes(session.EndTime);

            const isSessionFinished = isToday && currentTime > sessionEnd;

            const sessionReadOnly = isPastWeek || isPastDay || isSessionFinished;

            const sessionBox = document.createElement("div");
            sessionBox.className = "session-box";

            const doctorContainer = document.createElement("div");
            doctorContainer.className = "doctor-container";

            existingSchedules.forEach(item => {

                if (item.SessionID === session.SessionID &&
                    item.ClinicDate === formattedDate) {

                    const card = document.createElement("div");
                    card.className = "doctor-card";

                    card.setAttribute("data-doctorid", item.DoctorID);
                    card.setAttribute("data-sessionid", item.SessionID);
                    card.setAttribute("data-date", formattedDate);
                    card.setAttribute("data-name", item.DoctorName);

                    card.innerHTML =
                        `${item.DoctorName} <span class="remove-doctor">×</span>`;

                    if (!sessionReadOnly) {
                        card.querySelector(".remove-doctor").onclick = function () {
                            removeDoctorFromSession(card);
                        };
                    } else {
                        card.querySelector(".remove-doctor").remove();
                    }

                    doctorContainer.appendChild(card);
                }
            });

            sessionBox.innerHTML += `<div class="session-title">${session.SessionName}</div>`;
            sessionBox.appendChild(doctorContainer);

            if (!sessionReadOnly) {
                const btn = document.createElement("button");
                btn.className = "add-doctor-btn";
                btn.innerText = "Add Doctors";

                btn.onclick = function () {
                    selectedSessionId = session.SessionID;
                    selectedDate = formattedDate;
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
    
    resetDoctorModal();

    // get already added docs
    const existingCards = selectedContainer.querySelectorAll(".doctor-card");

    const existingDoctorNames = [];
    existingCards.forEach(card => {
        existingDoctorNames.push(card.getAttribute("data-name"));
    });

    //disable them
    const checkboxes = document.querySelectorAll("#doctorModal input[type='checkbox']");
    checkboxes.forEach(cb => {
        const name = cb.getAttribute("data-name");

        if (existingDoctorNames.includes(name)) {
            cb.disabled = true;
        }
    });
    document.getElementById("doctorModal").style.display = "flex";
}

function closeDoctorModal() {
    resetDoctorModal();
    document.getElementById("doctorModal").style.display = "none";
}
function resetDoctorModal() {
    const checkboxes = document.querySelectorAll("#doctorModal input[type='checkbox']");

    checkboxes.forEach(cb => {
        cb.checked = false;
        cb.disabled = false;
    });
}

function removeDoctorFromSession(cardElement) {

    const doctorId = parseInt(cardElement.getAttribute("data-doctorid"));
    const sessionId = parseInt(cardElement.getAttribute("data-sessionid"));
    const clinicDate = cardElement.getAttribute("data-date");

    fetch('/Admin/DoctorDutySchedule/RemoveDoctor', {
        method: "POST",
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            doctorId: doctorId,
            sessionId: sessionId,
            clinicDate: clinicDate
        })
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                cardElement.remove();

                showMessage("Doctor removed successfully.", "success");
            }
            else {
                showMessage("Failed to remove doctor from session.", "error");
            }
        });
}
function showMessage(message, type) {
    const box = document.getElementById("validationMessage");

    box.className = "validation-message";
    box.classList.add(type === "success" ? "validation-success" : "validation-error");

    box.innerText = message;
    box.style.display = "block";

    setTimeout(() => {
        box.style.display = "none";
    }, 4000);
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

                    const doctorId = parseInt(cb.value);
                    const name = cb.getAttribute("data-name");

                    const card = document.createElement("div");
                    card.className = "doctor-card";

                    card.setAttribute("data-doctorid", doctorId);
                    card.setAttribute("data-sessionid", selectedSessionId);
                    card.setAttribute("data-date", selectedDate);
                    card.setAttribute("data-name", name);
                    card.innerHTML =
                        `${name} <span class="remove-doctor">×</span>`;

                    card.querySelector(".remove-doctor").onclick = function () {
                        removeDoctorFromSession(card);
                    };

                    selectedContainer.appendChild(card);
                    cb.checked = false;
                });

                closeDoctorModal();

                showMessage("Doctor assigned successfully.", "success");
            } else {

                showMessage("Could not assign doctor.", "error");
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