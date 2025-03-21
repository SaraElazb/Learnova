document.addEventListener("DOMContentLoaded", function () {
    // Fade in animation
    const observer = new IntersectionObserver(
        (entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    entry.target.classList.add("visible");
                    observer.unobserve(entry.target);
                }
            });
        },
        { threshold: 0.1 }
    );

    document.querySelectorAll(".fade-in").forEach((el) => {
        observer.observe(el);
    });

    // Smooth scrolling for anchors
    document.querySelectorAll('a[href^="#"]').forEach((anchor) => {
        anchor.addEventListener("click", function (e) {
            e.preventDefault();
            const targetId = this.getAttribute("href");
            if (targetId && targetId !== "#") {
                const targetElement = document.querySelector(targetId);
                if (targetElement) {
                    targetElement.scrollIntoView({
                        behavior: "smooth",
                    });
                }
            }
        });
    });

    // Category buttons
    const categoryBtns = document.querySelectorAll(".category-btn");
    categoryBtns.forEach((btn) => {
        btn.addEventListener("click", () => {
            categoryBtns.forEach((b) => b.classList.remove("active"));
            btn.classList.add("active");
            
            console.log("Selected category:", btn.textContent.trim());
        });
    });

  
    // Course hover effect enhancement
    const courseCards = document.querySelectorAll(".course-card");
    courseCards.forEach((card) => {
        card.addEventListener("mouseenter", () => {
            card.querySelector(".course-view-btn").style.textDecoration = "underline";
        });

        card.addEventListener("mouseleave", () => {
            card.querySelector(".course-view-btn").style.textDecoration = "none";
        });
    });
});
