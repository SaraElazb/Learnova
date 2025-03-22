// Toggle the sidebar (filters)
function toggleSidebar() {
    const sidebar = document.getElementById("sidebar");
    sidebar.classList.toggle("active");
}

// Toggle the navigation links sidebar on mobile
function toggleNavLinks(event) {
    event.stopPropagation();
    const navLinks = document.querySelector('.nav-links');
    navLinks.classList.toggle('active');
}

// Close the nav-links sidebar when clicking outside of it
document.addEventListener('click', function(e) {
    const navLinks = document.querySelector('.nav-links');
    const navToggle = document.querySelector('.nav-toggle');
    if (navLinks && navLinks.classList.contains('active') &&
        !navLinks.contains(e.target) && !navToggle.contains(e.target)) {
        navLinks.classList.remove('active');
    }
});

// Close the nav-links sidebar on window resize if width > 768px
window.addEventListener('resize', function() {
    if (window.innerWidth > 768) {
        const navLinks = document.querySelector('.nav-links');
        if (navLinks && navLinks.classList.contains('active')) {
            navLinks.classList.remove('active');
        }
    }
});
