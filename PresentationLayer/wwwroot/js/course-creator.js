document.addEventListener("DOMContentLoaded", function () {
  // Tab switching logic
  const steps = document.querySelectorAll(".step");
  const sections = document.querySelectorAll(".section-container");
  const nextBtn = document.getElementById("next-btn");
  const backBtn = document.getElementById("back-btn");

  let currentSectionIndex = 0;

  // Function to switch sections
  function showSection(index) {
    sections.forEach((section) => {
      section.classList.remove("active");
    });

    sections[index].classList.add("active");

    steps.forEach((step, i) => {
      step.classList.remove("active", "completed");

      if (i < index) {
        step.classList.add("completed");
      } else if (i === index) {
        step.classList.add("active");
      }
    });

    currentSectionIndex = index;

    updateNavButtons();
  }

  steps.forEach((step, index) => {
    step.addEventListener("click", () => {
      showSection(index);
    });
  });

  nextBtn.addEventListener("click", () => {
    if (currentSectionIndex < sections.length - 1) {
      showSection(currentSectionIndex + 1);
    }
  });

  backBtn.addEventListener("click", () => {
    if (currentSectionIndex > 0) {
      showSection(currentSectionIndex - 1);
    }
  });

  function updateNavButtons() {
    backBtn.disabled = currentSectionIndex === 0;
    backBtn.style.opacity = currentSectionIndex === 0 ? "0.5" : "1";

    if (currentSectionIndex === sections.length - 1) {
      nextBtn.textContent = "Finish";
    } else {
      nextBtn.textContent = "Next";
    }
  }

  showSection(0);

  const textInputs = document.querySelectorAll(".form-control");
  textInputs.forEach((input) => {
    const maxLength = input.hasAttribute("data-max-length")
      ? parseInt(input.getAttribute("data-max-length"))
      : 50;
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

  const curriculumHeaders = document.querySelectorAll(".curriculum-header");

  function setupCurriculumHeaderListeners() {
    document.querySelectorAll(".curriculum-header").forEach((header) => {
      const newHeader = header.cloneNode(true);
      header.parentNode.replaceChild(newHeader, header);

      const deleteButton = newHeader.querySelector(".delete-lecture-button");
      if (deleteButton) {
        deleteButton.addEventListener("click", function (e) {
          e.stopPropagation();
          const lectureItem = this.closest(".curriculum-item");
          const lectureTitle =
            lectureItem.querySelector(".lecture-title").textContent;
          showDeleteConfirmation(lectureItem, lectureTitle);
        });
      }

      newHeader.addEventListener("click", function (e) {
        if (e.target.closest(".delete-lecture-button")) {
          return;
        }

        const item = this.closest(".curriculum-item");
        toggleCurriculumItem(item);
      });
    });
  }

  const { modal, confirmButton } = createDeleteModal();

  function showDeleteConfirmation(lectureItem, lectureTitle) {
    const lectureToDeleteSpan = modal.querySelector(".lecture-to-delete");
    lectureToDeleteSpan.textContent = lectureTitle;

    const newConfirmButton = confirmButton.cloneNode(true);
    confirmButton.parentNode.replaceChild(newConfirmButton, confirmButton);

    newConfirmButton.addEventListener("click", () => {
      deleteLecture(lectureItem);
      closeDeleteModal();
    });

    modal.classList.add("active");
  }

  function closeDeleteModal() {
    modal.classList.remove("active");
  }

  function deleteLecture(lectureItem) {
    lectureItem.style.opacity = "0";
    lectureItem.style.maxHeight = "0";
    lectureItem.style.marginBottom = "0";

    setTimeout(() => {
      lectureItem.remove();
      renumberLectures();
    }, 300);
  }

  function renumberLectures() {
    document.querySelectorAll(".curriculum-item").forEach((item, index) => {
      const lectureNumber = item.querySelector(".lecture-number");
      if (lectureNumber) {
        lectureNumber.textContent = `1.${index}`;
      }
    });
  }

  function toggleCurriculumItem(item) {
    const isExpanded = item.classList.contains("expanded");
    const curriculumContent = item.querySelector(".curriculum-content");

    document.querySelectorAll(".curriculum-item").forEach((el) => {
      el.classList.remove("expanded");
      const content = el.querySelector(".curriculum-content");
      if (content) {
        content.style.display = "none";
      }
      updateExpandIcon(el, false);
    });

    if (!isExpanded) {
      item.classList.add("expanded");
      if (curriculumContent) {
        curriculumContent.style.display = "block";
      } else {
        createEditableLectureContent(item);
      }
      updateExpandIcon(item, true);
    }
  }

  function updateExpandIcon(item, isExpanded) {
    const icon = item.querySelector(".expand-collapse svg path");
    if (icon) {
      if (isExpanded) {
        icon.setAttribute("d", "M5 15L12 8L19 15");
      } else {
        icon.setAttribute("d", "M19 9L12 16L5 9");
      }
    }
  }

  function createEditableLectureContent(item) {
    const lectureTitle = item.querySelector(".lecture-title").textContent;
    const contentDiv = document.createElement("div");
    contentDiv.className = "curriculum-content";
    contentDiv.style.display = "block";

    contentDiv.innerHTML = `
      <div class="lecture-form">
        <div class="form-group">
          <label for="lectureName${Date.now()}">Lecture Name</label>
          <input type="text" id="lectureName${Date.now()}" value="${lectureTitle}" class="form-control" data-max-length="50">
          <div class="char-count">0/50</div>
        </div>
        
        <div class="form-group">
          <label for="lectureFile${Date.now()}">Upload Video</label>
          <div class="file-upload-area">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M12 15V3M12 3L7 8M12 3L17 8" stroke="#10B981" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
              <path d="M3 15V16C3 18.2091 4.79086 20 7 20H17C19.2091 20 21 18.2091 21 16V15" stroke="#10B981" stroke-width="1.5" stroke-linecap="round"/>
            </svg>
            <p class="upload-text">Click here or drag and drop to upload files</p>
            <p class="upload-hint">Files: MP4, MOV or AVI (Max. 250MB)</p>
            <input type="file" id="lectureFile${Date.now()}" class="file-input" accept="video/mp4,video/mov,video/avi">
          </div>
        </div>
        
        <div class="action-buttons">
          <button class="cancel-button">Cancel</button>
          <button class="save-lecture-button">Save</button>
        </div>
      </div>
    `;

    item.appendChild(contentDiv);

    setupFileUpload(contentDiv.querySelector(".file-upload-area"));

    const newInput = contentDiv.querySelector(".form-control");
    const maxLength = 50;
    const charCountEl = contentDiv.querySelector(".char-count");
    updateCharCount(newInput, charCountEl, maxLength);

    newInput.addEventListener("input", () => {
      updateCharCount(newInput, charCountEl, maxLength);
    });

    const saveButton = contentDiv.querySelector(".save-lecture-button");
    saveButton.addEventListener("click", function () {
      const newName = newInput.value;
      if (newName.trim() !== "") {
        item.querySelector(".lecture-title").textContent = newName;
      }

      item.classList.add("expanded");
    });

    const cancelButton = contentDiv.querySelector(".cancel-button");
    cancelButton.addEventListener("click", function () {
      item.classList.remove("expanded");
      contentDiv.style.display = "none";
      updateExpandIcon(item, false);
    });
  }

  function createDeleteModal() {
    const modalHTML = `
      <div class="delete-modal">
        <div class="delete-modal-content">
          <div class="delete-modal-header">
            <h3 class="delete-modal-title">Delete Lecture</h3>
          </div>
          <div class="delete-modal-body">
            <p>Are you sure you want to delete "<span class="lecture-to-delete"></span>"? This action cannot be undone.</p>
          </div>
          <div class="delete-modal-actions">
            <button class="delete-cancel-button">Cancel</button>
            <button class="delete-confirm-button">Delete</button>
          </div>
        </div>
      </div>
    `;

    document.body.insertAdjacentHTML("beforeend", modalHTML);

    const modal = document.querySelector(".delete-modal");
    const cancelButton = modal.querySelector(".delete-cancel-button");
    const confirmButton = modal.querySelector(".delete-confirm-button");

    cancelButton.addEventListener("click", () => {
      closeDeleteModal();
    });

    modal.addEventListener("click", (e) => {
      if (e.target === modal) {
        closeDeleteModal();
      }
    });

    document.addEventListener("keydown", (e) => {
      if (e.key === "Escape" && modal.classList.contains("active")) {
        closeDeleteModal();
      }
    });

    return {
      modal,
      confirmButton,
    };
  }

  setupCurriculumHeaderListeners();

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

  const addLectureBtn = document.querySelector(".add-lecture-button");
  if (addLectureBtn) {
    addLectureBtn.addEventListener("click", () => {
      const curriculumContainer = document.querySelector(
        ".curriculum-container"
      );
      const newItemIndex = document.querySelectorAll(".curriculum-item").length;

      const newItem = document.createElement("div");
      newItem.className = "curriculum-item expanded";
      newItem.innerHTML = `
        <div class="curriculum-header">
          <div class="drag-handle">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M8 9H16M8 15H16" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>
            </svg>
          </div>
          <span class="lecture-number">1.${newItemIndex}</span>
          <h3 class="lecture-title">New Lecture</h3>
          <div class="spacer"></div>
          <button class="delete-lecture-button">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M3 6H5H21" stroke="#EF4444" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
              <path d="M8 6V4C8 3.46957 8.21071 2.96086 8.58579 2.58579C8.96086 2.21071 9.46957 2 10 2H14C14.5304 2 15.0391 2.21071 15.4142 2.58579C15.7893 2.96086 16 3.46957 16 4V6M19 6V20C19 20.5304 18.7893 21.0391 18.4142 21.4142C18.0391 21.7893 17.5304 22 17 22H7C6.46957 22 5.96086 21.7893 5.58579 21.4142C5.21071 21.0391 5 20.5304 5 20V6H19Z" stroke="#EF4444" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </button>
          <div class="expand-collapse">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M5 15L12 8L19 15" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </div>
        </div>
      `;

      curriculumContainer.appendChild(newItem);

      createEditableLectureContent(newItem);

      setupCurriculumHeaderListeners();

      newItem.scrollIntoView({ behavior: "smooth", block: "center" });
    });
  }

  const buttons = document.querySelectorAll("button");
  buttons.forEach((button) => {
    button.addEventListener("click", function (e) {
      if (
        !this.classList.contains("back-button") &&
        e.target.tagName === "BUTTON"
      ) {
        e.preventDefault();
      }

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

  const style = document.createElement("style");
  style.textContent = `
    button {
      position: relative;
      overflow: hidden;
    }
    
    .ripple {
      position: absolute;
      border-radius: 50%;
      background-color: rgba(255, 255, 255, 0.7);
      transform: scale(0);
      animation: ripple 0.6s linear;
      pointer-events: none;
    }
    
    @keyframes ripple {
      to {
        transform: scale(2);
        opacity: 0;
      }
    }
    
    .curriculum-item {
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    }
    
    .curriculum-content {
      max-height: 0;
      overflow: hidden;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      opacity: 0;
    }
    
    .curriculum-item.expanded .curriculum-content {
      max-height: 1000px;
      opacity: 1;
      display: block;
    }
    
    .main-content {
      animation: fadeIn 0.5s ease-out;
    }
  `;

  document.head.appendChild(style);

  const expandedItems = document.querySelectorAll(".curriculum-item.expanded");
  expandedItems.forEach((item) => {
    updateExpandIcon(item, true);
    if (item.querySelector(".curriculum-content")) {
      item.querySelector(".curriculum-content").style.display = "block";
    }
  });
});
