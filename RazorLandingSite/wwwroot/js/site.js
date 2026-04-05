function handleSubscribe(event) {
    event.preventDefault();
    const email = document.getElementById('subscribeEmail')?.value;
    
    if (email && /^\S+@\S+\.\S+$/.test(email)) {
        showSubscribePopup();
        document.getElementById('subscribeEmail').value = '';
    }
}

function showSubscribePopup() {
    const popup = document.getElementById('subscribePopup');
    if (popup) {
        popup.classList.remove('hidden');
    }
}

function closeSubscribePopup() {
    const popup = document.getElementById('subscribePopup');
    if (popup) {
        popup.classList.add('hidden');
    }
}

// Close popup when clicking outside
document.getElementById('subscribePopup')?.addEventListener('click', (e) => {
    if (e.target === document.getElementById('subscribePopup')) {
        closeSubscribePopup();
    }
});