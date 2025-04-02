document.addEventListener('DOMContentLoaded', () => {
    // Toggle Mobile Menu
    const mobileMenuToggle = document.querySelector('.mobile-menu-toggle');
    if (mobileMenuToggle) {
        mobileMenuToggle.addEventListener('click', () => {
            const nav = document.querySelector('.main-nav');
            if (nav) {
                nav.style.display = nav.style.display === 'flex' ? 'none' : 'flex';
            }
        });
    }

    // Toggle Filter Sections
    const filterHeaders = document.querySelectorAll('.filter-header');
    filterHeaders.forEach(header => {
        header.addEventListener('click', () => {
            header.classList.toggle('collapsed');
            const section = header.dataset.section;
            const options = document.getElementById(`${section}Options`);
            if (options) {
                options.style.display = options.style.display === 'none' ? 'block' : 'none';
            }
        });
    });

    // Toggle Mobile Filters
    const mobileFiltersToggle = document.getElementById('mobileFiltersToggle');
    if (mobileFiltersToggle) {
        mobileFiltersToggle.addEventListener('click', () => {
            const filtersContent = document.getElementById('filtersContent');
            if (filtersContent) {
                filtersContent.style.display = filtersContent.style.display === 'none' ? 'block' : 'none';
            }
        });
    }
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
});
