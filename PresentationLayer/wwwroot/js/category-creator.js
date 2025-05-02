document.addEventListener("DOMContentLoaded", function () {
  // Character count for text inputs
  const textInputs = document.querySelectorAll(
    ".form-control[data-max-length]"
  );
  textInputs.forEach((input) => {
    const maxLength = parseInt(input.getAttribute("data-max-length"));
    const charCountEl = input.parentElement.querySelector(".char-count");

    if (charCountEl) {
      updateCharCount(input, charCountEl, maxLength);

      input.addEventListener("input", () => {
        updateCharCount(input, charCountEl, maxLength);
      });
    }
  });

  function updateCharCount(input, countEl, maxLength) {
    const currentLength = input.value.length;
    countEl.textContent = `${currentLength}/${maxLength}`;

    if (currentLength > maxLength) {
      countEl.style.color = "var(--color-error)";
    } else {
      countEl.style.color = "var(--color-text-light)";
    }
  }

  // File upload functionality
  const fileUploadAreas = document.querySelectorAll(".file-upload-area");
  fileUploadAreas.forEach(setupFileUpload);

  function setupFileUpload(area) {
    const fileInput = area.querySelector(".file-input");

    area.addEventListener("click", () => {
      fileInput.click();
    });

    fileInput.addEventListener("change", () => {
      if (fileInput.files.length > 0) {
        const fileName = fileInput.files[0].name;
        const uploadText = area.querySelector(".upload-text");
        uploadText.textContent = `Selected: ${fileName}`;
        area.style.borderColor = "var(--color-primary)";
      }
    });

    area.addEventListener("dragover", (e) => {
      e.preventDefault();
      area.style.borderColor = "var(--color-primary)";
      area.style.backgroundColor = "rgba(16, 185, 129, 0.1)";
    });

    area.addEventListener("dragleave", () => {
      area.style.borderColor = "var(--color-border)";
      area.style.backgroundColor = "var(--color-bg-upload)";
    });

    area.addEventListener("drop", (e) => {
      e.preventDefault();

      if (e.dataTransfer.files.length > 0) {
        fileInput.files = e.dataTransfer.files;
        const fileName = fileInput.files[0].name;
        const uploadText = area.querySelector(".upload-text");
        uploadText.textContent = `Selected: ${fileName}`;
      }

      area.style.borderColor = "var(--color-primary)";
      area.style.backgroundColor = "var(--color-bg-upload)";
    });
  }

  // Color picker functionality
  const colorPicker = document.getElementById("categoryColor");
  const colorPreview = document.querySelector(".color-preview");
  const colorHexInput = document.getElementById("colorHex");

  colorPicker.addEventListener("input", () => {
    const color = colorPicker.value;
    colorPreview.style.backgroundColor = color;
    colorHexInput.value = color;
  });

  colorHexInput.addEventListener("input", () => {
    let color = colorHexInput.value;

    // Ensure the input is a valid hex color
    if (/^#([0-9A-F]{3}){1,2}$/i.test(color)) {
      colorPicker.value = color;
      colorPreview.style.backgroundColor = color;
    }
  });

  colorHexInput.addEventListener("blur", () => {
    let color = colorHexInput.value;

    // Add # if missing
    if (color.charAt(0) !== "#") {
      color = "#" + color;
    }

    // Validate format
    if (!/^#([0-9A-F]{3}){1,2}$/i.test(color)) {
      // Reset to default if invalid
      color = "#10B981";
    }

    colorHexInput.value = color;
    colorPicker.value = color;
    colorPreview.style.backgroundColor = color;
  });

  // Subcategory functionality
  const subcategoriesContainer = document.querySelector(
    ".subcategories-container"
  );
  const addSubcategoryButton = document.querySelector(
    ".add-subcategory-button"
  );
  const subcategoryNameInput = document.getElementById("subcategoryName");

  addSubcategoryButton.addEventListener("click", () => {
    const subcategoryName = subcategoryNameInput.value.trim();

    if (subcategoryName) {
      addSubcategory(subcategoryName);
      subcategoryNameInput.value = "";

      // Update character count
      const charCountEl =
        subcategoryNameInput.parentElement.querySelector(".char-count");
      updateCharCount(subcategoryNameInput, charCountEl, 50);
    }
  });

  function addSubcategory(name) {
    const subcategoryItem = document.createElement("div");
    subcategoryItem.className = "subcategory-item";

    subcategoryItem.innerHTML = `
      <span class="subcategory-name">${name}</span>
      <button class="delete-subcategory-button" aria-label="Delete subcategory">
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M3 6H5H21" stroke="#EF4444" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M8 6V4C8 3.46957 8.21071 2.96086 8.58579 2.58579C8.96086 2.21071 9.46957 2 10 2H14C14.5304 2 15.0391 2.21071 15.4142 2.58579C15.7893 2.96086 16 3.46957 16 4V6M19 6V20C19 20.5304 18.7893 21.0391 18.4142 21.4142C18.0391 21.7893 17.5304 22 17 22H7C6.46957 22 5.96086 21.7893 5.58579 21.4142C5.21071 21.0391 5 20.5304 5 20V6H19Z" stroke="#EF4444" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>
    `;

    const deleteButton = subcategoryItem.querySelector(
      ".delete-subcategory-button"
    );
    deleteButton.addEventListener("click", () => {
      showDeleteConfirmation(subcategoryItem, name);
    });

    subcategoriesContainer.appendChild(subcategoryItem);
  }

  // Delete confirmation
  const deleteModal = document.querySelector(".delete-modal");
  const deleteConfirmButton = deleteModal.querySelector(
    ".delete-confirm-button"
  );
  const deleteCancelButton = deleteModal.querySelector(".delete-cancel-button");
  const subcategoryToDeleteSpan = deleteModal.querySelector(
    ".subcategory-to-delete"
  );

  let subcategoryToDelete = null;

  function showDeleteConfirmation(subcategoryItem, subcategoryName) {
    subcategoryToDelete = subcategoryItem;
    subcategoryToDeleteSpan.textContent = subcategoryName;
    deleteModal.classList.add("active");
  }

  function closeDeleteModal() {
    deleteModal.classList.remove("active");
    subcategoryToDelete = null;
  }

  deleteConfirmButton.addEventListener("click", () => {
    if (subcategoryToDelete) {
      subcategoryToDelete.style.opacity = "0";
      subcategoryToDelete.style.maxHeight = "0";
      subcategoryToDelete.style.marginBottom = "0";

      setTimeout(() => {
        subcategoryToDelete.remove();
        subcategoryToDelete = null;
      }, 300);
    }
    closeDeleteModal();
  });

  deleteCancelButton.addEventListener("click", closeDeleteModal);

  deleteModal.addEventListener("click", (e) => {
    if (e.target === deleteModal) {
      closeDeleteModal();
    }
  });

  document.addEventListener("keydown", (e) => {
    if (e.key === "Escape" && deleteModal.classList.contains("active")) {
      closeDeleteModal();
    }
  });

  // Navigation
  const backButton = document.querySelector(".header .back-button");
  backButton.addEventListener("click", () => {
    window.location.href = "/";
  });

  // Save functionality
  const saveButton = document.querySelector(".action-buttons .save-button");
  saveButton.addEventListener("click", saveCategory);

  const headerSaveButton = document.querySelector(".header .save-button");
  headerSaveButton.addEventListener("click", saveCategory);

  function saveCategory() {
    // Get values
    const categoryName = document.getElementById("categoryName").value.trim();
    const categoryDescription = document
      .getElementById("categoryDescription")
      .value.trim();
    const categoryColor = document.getElementById("categoryColor").value;
    const categoryParent = document.getElementById("categoryParent").value;

    // Validate required fields
    if (!categoryName) {
      alert("Please enter a category name");
      return;
    }

    // Get subcategories
    const subcategories = [];
    document.querySelectorAll(".subcategory-item").forEach((item) => {
      subcategories.push(item.querySelector(".subcategory-name").textContent);
    });

    // Create category object
    const categoryData = {
      name: categoryName,
      description: categoryDescription,
      color: categoryColor,
      parent: categoryParent,
      subcategories: subcategories,
      icon: document.getElementById("categoryIcon").files[0]
        ? document.getElementById("categoryIcon").files[0].name
        : null,
    };

    // In a real app, you would send this data to the server
    console.log("Category data to be saved:", categoryData);

    // Show success message (in a real app, you would wait for server response)
    alert("Category saved successfully!");

    // Redirect to dashboard or categories list
    // window.location.href = "/";
  }

  // Cancel button functionality
  const cancelButton = document.querySelector(".action-buttons .cancel-button");
  cancelButton.addEventListener("click", () => {
    if (confirm("Are you sure you want to cancel? All changes will be lost.")) {
      window.location.href = "/";
    }
  });

  // Ripple effect for buttons
  const buttons = document.querySelectorAll("button");
  buttons.forEach((button) => {
    button.addEventListener("click", function (e) {
      const ripple = document.createElement("span");
      const rect = this.getBoundingClientRect();

      const size = Math.max(rect.width, rect.height);
      const x = e.clientX - rect.left - size / 2;
      const y = e.clientY - rect.top - size / 2;

      ripple.style.width = ripple.style.height = `${size}px`;
      ripple.style.left = `${x}px`;
      ripple.style.top = `${y}px`;
      ripple.className = "ripple";

      const currentRipples = this.getElementsByClassName("ripple");
      [].forEach.call(currentRipples, function (ripple) {
        ripple.remove();
      });

      this.appendChild(ripple);

      setTimeout(() => {
        ripple.remove();
      }, 600);
    });
  });

  // Add some example subcategories
  addSubcategory("Graphic Design");
  addSubcategory("UX/UI Design");
});
