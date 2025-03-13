let currentPage = 1;
const itemsPerPage = 8;
let courses = []; // Will be set from coursesData injected in the Razor view

// Render course cards based on the filtered courses
function renderCourses(filteredCourses) {
    const courseList = document.getElementById("course-list");
    courseList.innerHTML = "";
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    const coursesToRender = filteredCourses.slice(startIndex, endIndex);

    coursesToRender.forEach((course) => {
        const courseLink = document.createElement("a");
        courseLink.className = "course-item";
        courseLink.href = `/Course/Details/${course.Course_ID}`;
        courseLink.innerHTML = `
            <img src="${course.ImagePath}" alt="Course Image">
            <div class="course-info">
                <h3 class="course-title">${course.Title}</h3>
                <div class="instructor-price">
                    <p class="course-instructor">${course.Instructor || ''}</p>
                    <div class="course-price">${course.Price}</div>
                </div>
            </div>
        `;
        courseList.appendChild(courseLink);
    });
    renderPagination(filteredCourses);
}

// Render pagination buttons based on the filtered course list
function renderPagination(filteredCourses) {
    const pagination = document.getElementById("pagination");
    pagination.innerHTML = "";
    const totalPages = Math.ceil(filteredCourses.length / itemsPerPage);
    if (totalPages <= 1) return;

    if (currentPage > 1) {
        const prevBtn = document.createElement("button");
        prevBtn.innerText = "Previous";
        prevBtn.onclick = () => changePage(currentPage - 1);
        pagination.appendChild(prevBtn);
    }

    for (let i = 1; i <= totalPages; i++) {
        const pageBtn = document.createElement("button");
        pageBtn.innerText = i;
        if (i === currentPage) pageBtn.classList.add("active");
        pageBtn.onclick = () => changePage(i);
        pagination.appendChild(pageBtn);
    }

    if (currentPage < totalPages) {
        const nextBtn = document.createElement("button");
        nextBtn.innerText = "Next";
        nextBtn.onclick = () => changePage(currentPage + 1);
        pagination.appendChild(nextBtn);
    }
}

// Change the current page and re-filter courses
function changePage(page) {
    currentPage = page;
    filterCourses();
}

// Filter courses based on search input and selected categories
function filterCourses() {
    const searchInput = document.getElementById("searchInput").value.toLowerCase();

    // Convert selected checkbox values to lowercase
    const selectedCategories = Array.from(
        document.querySelectorAll('.sidebar input[type="checkbox"]:checked')
    ).map(cb => cb.value.toLowerCase());

    const filteredCourses = courses.filter(course => {
        // Ensure Title is checked in lowercase
        const matchesSearch = course.Title.toLowerCase().includes(searchInput);

        // Use either CourseDTO property "CategoryName" or "Category" and convert to lowercase
        const courseCategory = (course.Category || "").toLowerCase();

        const matchesCategory =
            selectedCategories.length === 0 ||
            selectedCategories.includes(courseCategory);

        return matchesSearch && matchesCategory;
    });

    currentPage = 1; // Reset to first page when filtering
    renderCourses(filteredCourses);
}

// Clear search input and category filters
function clearFilters() {
    document.getElementById("searchInput").value = "";
    document.querySelectorAll('.sidebar input[type="checkbox"]').forEach(cb => cb.checked = false);
    filterCourses();
}

// Toggle the sidebar (for mobile views)
function toggleSidebar() {
    const sidebar = document.getElementById("sidebar");
    sidebar.classList.toggle("active");
}

// On window load, initialize the courses array from the injected coursesData and render the courses
window.onload = () => {
    if (typeof coursesData !== 'undefined') {
        courses = coursesData;
    }
    renderCourses(courses);
};
