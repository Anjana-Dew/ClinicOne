function markRead(id, btn) {
    fetch('/Patient/Notifications/MarkRead', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ id: id })
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                btn.innerText = "Read";
                btn.parentElement.classList.add("read");
            }
        });
}