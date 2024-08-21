function confirmDelete(url) {
    if (confirm("Are you sure you want to delete this user?")) {
        window.location.href = url;
    }
}
