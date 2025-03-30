document.addEventListener("DOMContentLoaded", function () {
    // DOM elements
    const sidebar = document.querySelector(".sidebar");
    const mobileMenuBtn = document.querySelector(".mobile-menu-btn");
    const mobileCloseBtn = document.querySelector(".mobile-close-btn");
    const profileBtn = document.querySelector(".profile-btn");
    const studentsLink = document.querySelector(".students-link");
    const reportsLink = document.querySelector(".reports-link");
    const settingsLink = document.querySelector(".settings-link");
    const logoutLink = document.querySelector(".logout-link");
    const notificationBtn = document.querySelector(".notification-btn");

    // Create profile dropdown dynamically
    const profileDropdown = document.createElement("div");
    profileDropdown.className = "profile-dropdown";
    profileDropdown.innerHTML = `
    <ul>
      <li><a href="#">My Profile</a></li>
      <li><a href="#">Account Settings</a></li>
      <li><a href="#">Logout</a></li>
    </ul>
  `;

    // Append dropdown to user menu
    const userMenu = document.querySelector(".user-menu");
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
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M12 8V12M12 16H12.01M22 12C22 17.5228 17.5228 22 12 22C6.47715 22 2 17.5228 2 12C2 6.47715 6.47715 2 12 2C17.5228 2 22 6.47715 22 12Z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </div>
        <div class="notification-content">
          <p class="notification-text">New course <strong>"Advanced React Patterns"</strong> has been published</p>
          <span class="notification-time">Just now</span>
        </div>
      </div>
      <div class="notification-item unread">
        <div class="notification-icon success">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M9 12L11 14L15 10M21 12C21 16.9706 16.9706 21 12 21C7.02944 21 3 16.9706 3 12C3 7.02944 7.02944 3 12 3C16.9706 3 21 7.02944 21 12Z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </div>
        <div class="notification-content">
          <p class="notification-text"><strong>10 new students</strong> enrolled in your courses today</p>
          <span class="notification-time">2 hours ago</span>
        </div>
      </div>
      <div class="notification-item">
        <div class="notification-icon warning">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
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

    // Append notification popup to user menu
    const notificationsContainer = document.querySelector(".notifications");
    notificationsContainer.appendChild(notificationPopup);

    // Toggle sidebar on mobile
    mobileMenuBtn.addEventListener("click", function () {
        sidebar.classList.add("active");
    });

    mobileCloseBtn.addEventListener("click", function () {
        sidebar.classList.remove("active");
    });

    // Close sidebar when clicking outside on mobile
    document.addEventListener("click", function (e) {
        if (
            sidebar.classList.contains("active") &&
            !sidebar.contains(e.target) &&
            !mobileMenuBtn.contains(e.target)
        ) {
            sidebar.classList.remove("active");
        }
    });

    // Toggle profile dropdown
    profileBtn.addEventListener("click", function (e) {
        e.stopPropagation();
        profileDropdown.classList.toggle("active");

        // Close notification popup if open
        notificationPopup.classList.remove("active");
    });

    // Toggle notification popup
    notificationBtn.addEventListener("click", function (e) {
        e.stopPropagation();
        notificationPopup.classList.toggle("active");

        // Close profile dropdown if open
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

    // Handle mark all as read
    const markAllReadBtn = document.querySelector(".mark-all-read");
    markAllReadBtn.addEventListener("click", function () {
        const unreadNotifications = document.querySelectorAll(
            ".notification-item.unread"
        );
        unreadNotifications.forEach((notification) => {
            notification.classList.remove("unread");
        });

        // Update notification badge
        const notificationBadge = document.querySelector(".notification-badge");
        notificationBadge.style.display = "none";
    });

    // Placeholder alerts for not-yet-implemented pages
    studentsLink.addEventListener("click", function (e) {
        e.preventDefault();
        alert("Students management will be implemented in a future update.");
    });

    reportsLink.addEventListener("click", function (e) {
        e.preventDefault();
        alert("Reports will be implemented in a future update.");
    });

    settingsLink.addEventListener("click", function (e) {
        e.preventDefault();
        alert("Settings will be implemented in a future update.");
    });

    logoutLink.addEventListener("click", function (e) {
        e.preventDefault();
        if (confirm("Are you sure you want to logout?")) {
            window.location.href = "/";
        }
    });

    // Initialize any charts or other components here
    initializeCharts();

    // Add ripple effect to buttons
    const buttons = document.querySelectorAll(
        "button:not(.profile-btn):not(.notification-btn)"
    );
    buttons.forEach((button) => {
        button.addEventListener("click", createRippleEffect);
    });

    // Function to create ripple effect
    function createRippleEffect(e) {
        const button = this;
        const ripple = document.createElement("span");

        const rect = button.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const x = e.clientX - rect.left - size / 2;
        const y = e.clientY - rect.top - size / 2;

        ripple.style.width = ripple.style.height = `${size}px`;
        ripple.style.left = `${x}px`;
        ripple.style.top = `${y}px`;
        ripple.className = "ripple";

        // Clear existing ripples
        const currentRipples = button.getElementsByClassName("ripple");
        Array.from(currentRipples).forEach((ripple) => ripple.remove());

        // Add new ripple
        button.appendChild(ripple);

        // Remove ripple after animation completes
        setTimeout(() => ripple.remove(), 600);
    }

    // Function to initialize charts (placeholder for future implementation)
    function initializeCharts() {
        // This would be where we initialize charts if needed
        console.log("Charts initialized");
    }

    // Add click handler for view all buttons
    const viewAllButtons = document.querySelectorAll(".view-all-btn");
    viewAllButtons.forEach((button) => {
        button.addEventListener("click", function (e) {
            e.preventDefault();

            // Get the title of the card
            const cardTitle =
                this.closest(".card-header").querySelector("h3").textContent;

            if (cardTitle === "Recent Activity") {
                alert("View all activities will be implemented in a future update.");
            } else if (cardTitle === "Popular Courses") {
                window.location.href = "/course-creator.html";
            }
        });
    });

    // Ensure sidebar is closed on page load for mobile
    if (window.innerWidth <= 1024) {
        sidebar.classList.remove("active");
    }

    // Update sidebar state on window resize
    window.addEventListener("resize", function () {
        if (window.innerWidth > 1024) {
            sidebar.style.transform = "translateX(0)";
        } else {
            if (!sidebar.classList.contains("active")) {
                sidebar.style.transform = "translateX(-100%)";
            }
        }
    });
});
