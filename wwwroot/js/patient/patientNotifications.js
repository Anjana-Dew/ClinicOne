function updateBadge() {
    fetch('/Patient/Notification/GetUnreadCount')
        .then(res => res.json())
        .then(count => {
            const notifWrapper = document.querySelector('.notification');
            if (!notifWrapper) return;

            let badge = notifWrapper.querySelector('.badge');

            if (count > 0) {
                if (!badge) {
                    badge = document.createElement('span');
                    badge.className = 'badge';
                    notifWrapper.querySelector('a').appendChild(badge);
                }
            } else {
                if (badge) badge.remove();
            }
        })
        .catch(() => { });
}

updateBadge();
setInterval(updateBadge, 30000);

function onCardClick(id, card) {
    if (card.classList.contains('unread')) {
        fetch('/Patient/Notification/MarkRead', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id: id })
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    card.classList.remove('unread');
                    card.classList.add('read');
                    const dot = card.querySelector('.unread-dot');
                    if (dot) dot.remove();
                    updateBadge();
                }
            })
            .catch(() => { });
    }
}

function markAllAsRead() {
    fetch('/Patient/Notification/MarkAllRead', {
        method: 'POST'
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                document.querySelectorAll('.notif-card.unread').forEach(card => {
                    card.classList.remove('unread');
                    card.classList.add('read');
                    const dot = card.querySelector('.unread-dot');
                    if (dot) dot.remove();
                });

                const btn = document.querySelector('.mark-all-btn');
                if (btn) btn.remove();

                updateBadge();
            }
        })
        .catch(() => { });
}

function filterNotifications(type, clicked) {
    document.querySelectorAll('#filterBar button').forEach(btn => {
        btn.classList.remove('active');
    });

    clicked.classList.add('active');

    document.querySelectorAll('.notif-card').forEach(card => {
        card.style.display = (type === 'all' || card.dataset.type === type)
            ? 'block'
            : 'none';
    });
}