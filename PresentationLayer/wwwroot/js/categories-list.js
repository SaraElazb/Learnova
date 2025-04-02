document.addEventListener("DOMContentLoaded", function () {
    // 1) Debug: Check if categoriesData is available
    console.log("categoriesData from Razor:", categoriesData);

    let categories = [...categoriesData]; // working copy that reflects deletions

    // 2) DOM element references
    const categoriesContainer = document.querySelector(".categories-grid");
    const searchInput = document.getElementById("searchCategories");
    const filterSelect = document.getElementById("filterCategories");
    const addCategoryButton = document.querySelector(".add-category-button");
    const backButton = document.querySelector(".back-button");

    // 3) Delete modal elements
    const deleteModal = document.querySelector(".delete-modal");
    const deleteConfirmButton = document.querySelector(".delete-confirm-button");
    const deleteCancelButton = document.querySelector(".delete-cancel-button");
    const categoryToDeleteSpan = document.querySelector(".category-to-delete");

    let categoryToDelete = null;

    // 4) Render categories on initial load
    renderCategories(categories);

    // 5) Event listeners for search and filter
    searchInput.addEventListener("input", filterCategories);
    filterSelect.addEventListener("change", filterCategories);

    addCategoryButton.addEventListener("click", () => {
        window.location.href = "/Category/Creator";
    });
    backButton.addEventListener("click", () => {
        window.location.href = "/";
    });

    document.querySelectorAll("button").forEach(button => {
        button.addEventListener("click", createRippleEffect);
    });


    function renderCategories(catArray) {
        console.log("renderCategories called with:", catArray.length, "items");
        categoriesContainer.innerHTML = "";

        if (catArray.length === 0) {
            renderEmptyState();
            return;
        }

        catArray.forEach(function (category) {
            const card = createCategoryCard(category);
            categoriesContainer.appendChild(card);
        });
    }

    function createCategoryCard(category) {
        console.log("Creating card for:", category.Category_ID, category.Category_Name);

        const card = document.createElement("div");
        card.className = "category-card";
        card.id = "category-" + category.Category_ID;

        const imageSrc = category.ImagePath ? category.ImagePath : "/images/default-category.png";
        const color = category.IsActive ? "#008000" : "#ff0000";

        card.innerHTML = `
      <div class="category-color" style="background-color: ${color}"></div>
      <div class="category-header">
        <div class="category-icon">
          <img src="${imageSrc}" alt="${category.Category_Name} icon">
        </div>
        <div class="category-name-container">
          <h3 class="category-name">${category.Category_Name}</h3>
        </div>
      </div>
      <p class="category-description">${category.Description || ""}</p>
      <div class="category-actions">
        <button class="edit-category-button" data-id="${category.Category_ID}" aria-label="Edit Category">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none"
               xmlns="http://www.w3.org/2000/svg">
            <path d="M12 5H9C7.89543 5 7 5.89543 7 7V17C7 18.1046 7.89543 19 9 19H15C16.1046 19 17 18.1046 17 17V10M12 5V3M12 5V7M17 3L19 5L15 9H13V7L17 3Z"
                  stroke="currentColor" stroke-width="1.5" stroke-linecap="round"
                  stroke-linejoin="round"/>
          </svg>
        </button>
        <button class="delete-category-button" data-id="${category.Category_ID}" aria-label="Delete Category">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none"
               xmlns="http://www.w3.org/2000/svg">
            <path d="M3 6H5H21" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"
                  stroke-linejoin="round"/>
            <path d="M8 6V4C8 3.46957 8.21071 2.96086 8.58579 2.58579C8.96086 2.21071 9.46957 2 10 2H14C14.5304 2
                     15.0391 2.21071 15.4142 2.58579C15.7893 2.96086 16 3.46957 16 4V6M19 6V20C19 20.5304
                     18.7893 21.0391 18.4142 21.4142C18.0391 21.7893 17.5304 22 17 22H7C6.46957 22 5.96086
                     21.7893 5.58579 21.4142C5.21071 21.0391 5 20.5304 5 20V6H19Z"
                  stroke="currentColor" stroke-width="1.5" stroke-linecap="round"
                  stroke-linejoin="round"/>
          </svg>
        </button>
      </div>
    `;

        const editBtn = card.querySelector(".edit-category-button");
        editBtn.addEventListener("click", function (e) {
            e.stopPropagation();
            window.location.href = `/Category/Creator?id=${category.Category_ID}`;
        });

        const deleteBtn = card.querySelector(".delete-category-button");
        deleteBtn.addEventListener("click", function (e) {
            e.stopPropagation();
            showDeleteConfirmation(category);
        });

        return card;
    }

    // Filters categories based on the search input
    function filterCategories() {
        const term = searchInput.value.toLowerCase();
        let filtered = [...categories]; // reflect any deletions

        if (term) {
            filtered = filtered.filter(category =>
                category.Category_Name.toLowerCase().includes(term) ||
                (category.Description && category.Description.toLowerCase().includes(term))
            );
        }

        renderCategories(filtered);
    }

    // Renders an empty state if no categories are found
    function renderEmptyState() {
        categoriesContainer.innerHTML = `
      <div class="empty-state">
        <div class="empty-state-icon">
          <!-- Optional SVG for empty state -->
        </div>
        <h3 class="empty-state-title">No Categories Found</h3>
        <p class="empty-state-text">
          No categories match your current filters. Try adjusting your search or create a new category.
        </p>
        <button class="no-categories-button" onclick="window.location.href='/Category/Creator'">
          <svg width="16" height="16" viewBox="0 0 16 16" fill="none"
               xmlns="http://www.w3.org/2000/svg">
            <path d="M8 3.5V12.5M3.5 8H12.5" stroke="currentColor" stroke-width="1.5"
                  stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          Add New Category
        </button>
      </div>
    `;
    }

    // Delete modal functions
    function showDeleteConfirmation(category) {
        categoryToDelete = category;
        categoryToDeleteSpan.textContent = category.Category_Name;
        deleteModal.classList.add("active");
    }

    function closeDeleteModal() {
        deleteModal.classList.remove("active");
        setTimeout(() => { categoryToDelete = null; }, 300);
    }

    deleteConfirmButton.addEventListener("click", function () {
        if (categoryToDelete) {
            const card = document.getElementById("category-" + categoryToDelete.Category_ID);
            if (card) {
                card.style.opacity = "0";
                card.style.transform = "scale(0.9)";
            }
            setTimeout(() => {
                // Remove the category from the array
                categories = categories.filter(c => c.Category_ID !== categoryToDelete.Category_ID);
                filterCategories();
                closeDeleteModal();
            }, 300);
        }
    });

    deleteCancelButton.addEventListener("click", closeDeleteModal);

    deleteModal.addEventListener("click", function (e) {
        if (e.target === deleteModal) {
            closeDeleteModal();
        }
    });

    document.addEventListener("keydown", function (e) {
        if (e.key === "Escape" && deleteModal.classList.contains("active")) {
            closeDeleteModal();
        }
    });

    function createRippleEffect(e) {
        const button = this;
        const ripple = document.createElement("span");
        const rect = button.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const x = e.clientX - rect.left - size / 2;
        const y = e.clientY - rect.top - size / 2;
        ripple.style.width = size + "px";
        ripple.style.height = size + "px";
        ripple.style.left = x + "px";
        ripple.style.top = y + "px";
        ripple.className = "ripple";
        Array.from(button.getElementsByClassName("ripple")).forEach(r => r.remove());
        button.appendChild(ripple);
        setTimeout(() => ripple.remove(), 600);
    }
});
