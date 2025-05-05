document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.querySelector(".sidebar");
    const mobileMenuBtn = document.querySelector(".mobile-menu-btn");
    const mobileCloseBtn = document.querySelector(".mobile-close-btn");
    const profileBtn = document.querySelector(".profile-btn");
    const notificationBtn = document.querySelector(".notification-btn");
    const logoutLink = document.querySelector(".logout-link");

    // Create profile dropdown dynamically
    const profileDropdown = document.createElement("div");
    profileDropdown.className = "profile-dropdown";
    profileDropdown.innerHTML = `
      <ul>
        <li><a href="#">My Profile</a></li>
        <li><a href="#">Account Settings</a></li>
        <li>
          <form method="post" action="/Account/LogOut">
            <button type="submit" class="dropdown-logout-btn">Logout</button>
          </form>
        </li>
      </ul>
    `;
    // Append profile dropdown to user menu
    const userMenu = document.querySelector(".user-profile");
    userMenu.appendChild(profileDropdown);

    // Create notification popup dynamically
    const notificationPopup = document.createElement("div");
    notificationPopup.className = "notification-popup";
    notificationPopup.innerHTML = `
      <div class="notification-header">
        <h3>Notifications</h3>
        <button class="mark-all-read">Mark all as read</button>
      </div>
      <div class="notification-list">
        <div class="notification-item unread">
          <div class="notification-icon info">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none">
              <path d="M12 8V12M12 16H12.01M22 12C22 17.5228 17.5228 22 12 22C6.47715 22 2 17.5228 2 12C2 6.47715 6.47715 2 12 2C17.5228 2 22 6.47715 22 12Z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </div>
          <div class="notification-content">
            <p class="notification-text">New course <strong>"Advanced React Patterns"</strong> has been published</p>
            <span class="notification-time">Just now</span>
          </div>
        </div>
        <div class="notification-item">
          <div class="notification-icon warning">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none">
              <path d="M12 9V13M12 17H12.01M5.07183 19H18.9282C20.4678 19 21.4301 17.3333 20.6603 16L13.7321 4C12.9623 2.66667 11.0378 2.66667 10.268 4L3.33978 16C2.56998 17.3333 3.53223 19 5.07183 19Z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </div>
          <div class="notification-content">
            <p class="notification-text">Your subscription will expire in <strong>5 days</strong></p>
            <span class="notification-time">Yesterday</span>
          </div>
        </div>
      </div>
      <div class="notification-footer">
        <a href="#" class="view-all-notifications">View all notifications</a>
      </div>
    `;
    // Append notification popup to notifications container
    const notificationsContainer = document.querySelector(".notifications");
    notificationsContainer.appendChild(notificationPopup);

    // Toggle sidebar on mobile
    mobileMenuBtn.addEventListener("click", function () {
        sidebar.classList.add("active");
    });
    mobileCloseBtn.addEventListener("click", function () {
        sidebar.classList.remove("active");
    });
    // Close sidebar when clicking outside (on mobile)
    document.addEventListener("click", function (e) {
        if (sidebar.classList.contains("active") &&
            !sidebar.contains(e.target) &&
            !mobileMenuBtn.contains(e.target)) {
            sidebar.classList.remove("active");
        }
    });

    // Toggle profile dropdown
    profileBtn.addEventListener("click", function (e) {
        e.stopPropagation();
        profileDropdown.classList.toggle("active");
        notificationPopup.classList.remove("active");
    });
    notificationBtn.addEventListener("click", function (e) {
        e.stopPropagation();
        notificationPopup.classList.toggle("active");
        profileDropdown.classList.remove("active");
    });
    // Close dropdowns when clicking outside
    document.addEventListener("click", function (e) {
        if (!profileBtn.contains(e.target)) {
            profileDropdown.classList.remove("active");
        }
        if (!notificationBtn.contains(e.target)) {
            notificationPopup.classList.remove("active");
        }
    });

    // Handle "Mark all as read" in notifications
    const markAllReadBtn = document.querySelector(".mark-all-read");
    markAllReadBtn.addEventListener("click", function () {
        const unreadNotifications = document.querySelectorAll(".notification-item.unread");
        unreadNotifications.forEach(notification => notification.classList.remove("unread"));
        const notificationBadge = document.querySelector(".notification-badge");
        if (notificationBadge) {
            notificationBadge.style.display = "none";
        }
    });

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
