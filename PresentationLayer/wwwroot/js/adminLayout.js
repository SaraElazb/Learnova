document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.querySelector(".sidebar");
    const mobileMenuBtn = document.querySelector(".mobile-menu-btn");
    const mobileCloseBtn = document.querySelector(".mobile-close-btn");
    const profileBtn = document.querySelector(".profile-btn");
    const notificationBtn = document.querySelector(".notification-btn");
    const logoutLink = document.querySelector(".logout-link");
    const profileDropdown = document.querySelector(".profile-dropdown");

    // Toggle sidebar on mobile
    if (mobileMenuBtn) {
        mobileMenuBtn.addEventListener("click", function () {
            sidebar.classList.add("active");
        });
    }
    
    if (mobileCloseBtn) {
        mobileCloseBtn.addEventListener("click", function () {
            sidebar.classList.remove("active");
        });
    }
    
    // Close sidebar when clicking outside (on mobile)
    document.addEventListener("click", function (e) {
        if (sidebar.classList.contains("active") &&
            !sidebar.contains(e.target) &&
            !mobileMenuBtn.contains(e.target)) {
            sidebar.classList.remove("active");
        }
    });

    // Toggle profile dropdown
    if (profileBtn && profileDropdown) {
        profileBtn.addEventListener("click", function (e) {
            e.stopPropagation();
            profileDropdown.classList.toggle("active");
            // If notification popup exists, close it
            const notificationPopup = document.querySelector(".notification-popup");
            if (notificationPopup) {
                notificationPopup.classList.remove("active");
            }
        });
    }
    
    if (notificationBtn) {
        const notificationPopup = document.querySelector(".notification-popup");
        if (notificationPopup) {
            notificationBtn.addEventListener("click", function (e) {
                e.stopPropagation();
                notificationPopup.classList.toggle("active");
                if (profileDropdown) {
                    profileDropdown.classList.remove("active");
                }
            });
        }
    }
    
    // Close dropdowns when clicking outside
    document.addEventListener("click", function (e) {
        if (profileDropdown && !profileBtn.contains(e.target)) {
            profileDropdown.classList.remove("active");
        }
        
        const notificationPopup = document.querySelector(".notification-popup");
        if (notificationPopup && notificationBtn && !notificationBtn.contains(e.target)) {
            notificationPopup.classList.remove("active");
        }
    });

    // Handle "Mark all as read" in notifications
    const markAllReadBtn = document.querySelector(".mark-all-read");
    if (markAllReadBtn) {
        markAllReadBtn.addEventListener("click", function () {
            const unreadNotifications = document.querySelectorAll(".notification-item.unread");
            unreadNotifications.forEach(notification => notification.classList.remove("unread"));
            const notificationBadge = document.querySelector(".notification-badge");
            if (notificationBadge) {
                notificationBadge.style.display = "none";
            }
        });
    }

    // Only set up logout handler if it's not a form submit
    if (logoutLink) {
        logoutLink.addEventListener("click", function (e) {
            // The form will handle the actual logout
            // This is just for any custom confirmation
            if (logoutLink.tagName !== 'BUTTON' || !logoutLink.closest('form')) {
                e.preventDefault();
                if (confirm("Are you sure you want to logout?")) {
                    // Find the form and submit it
                    const logoutForm = document.querySelector('form[action*="LogOut"]');
                    if (logoutForm) {
                        logoutForm.submit();
                    } else {
                        window.location.href = "/Account/LogOut";
                    }
                }
            }
        });
    }

    // Initialize ripple effect for buttons
    const buttons = document.querySelectorAll("button:not(.profile-btn):not(.notification-btn)");
    buttons.forEach(button => {
        button.addEventListener("click", createRippleEffect);
    });
    
    function createRippleEffect(e) {
        const button = this;
        const ripple = document.createElement("span");
        const rect = button.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const x = e.clientX - rect.left - size / 2;
        const y = e.clientY - rect.top - size / 2;
        ripple.style.width = ripple.style.height = size + "px";
        ripple.style.left = x + "px";
        ripple.style.top = y + "px";
        ripple.className = "ripple";
        const currentRipples = button.getElementsByClassName("ripple");
        Array.from(currentRipples).forEach(r => r.remove());
        button.appendChild(ripple);
        setTimeout(() => ripple.remove(), 600);
    }

    // Handle responsive behavior
    if (window.innerWidth <= 1024) {
        sidebar.classList.remove("active");
    }
    window.addEventListener("resize", function () {
        if (window.innerWidth > 1024) {
            sidebar.style.transform = "translateX(0)";
        } else if (!sidebar.classList.contains("active")) {
            sidebar.style.transform = "translateX(-100%)";
        }
    });
});
