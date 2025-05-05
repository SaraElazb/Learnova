document.addEventListener("DOMContentLoaded", function () {
  // Character count functionality
  const textInputs = document.querySelectorAll(".form-control[data-max-length]");
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

  // File upload handling
  const fileInputs = document.querySelectorAll(".file-input");
  fileInputs.forEach((input) => {
    const uploadArea = input.closest(".file-upload-area");
    
    uploadArea.addEventListener("click", () => {
      input.click();
    });
    
    uploadArea.addEventListener("dragover", (e) => {
      e.preventDefault();
      uploadArea.classList.add("dragging");
    });
    
    uploadArea.addEventListener("dragleave", () => {
      uploadArea.classList.remove("dragging");
    });
    
    uploadArea.addEventListener("drop", (e) => {
      e.preventDefault();
      uploadArea.classList.remove("dragging");
      
      if (e.dataTransfer.files.length) {
        input.files = e.dataTransfer.files;
        updateFilePreview(input);
      }
    });
    
    input.addEventListener("change", () => {
      updateFilePreview(input);
    });
  });
  
  function updateFilePreview(input) {
    const uploadArea = input.closest(".file-upload-area");
    const file = input.files[0];
    
    if (file && file.type.startsWith("image/")) {
      const reader = new FileReader();
      reader.onload = function(e) {
        const previewDiv = document.createElement("div");
        previewDiv.className = "file-preview";
        previewDiv.innerHTML = `
          <img src="${e.target.result}" alt="Preview" class="preview-image">
          <div class="preview-info">
            <span class="preview-name">${file.name}</span>
            <span class="preview-size">${formatFileSize(file.size)}</span>
          </div>
          <button type="button" class="remove-file">×</button>
        `;
        
        // Check if preview already exists
        const existingPreview = uploadArea.querySelector(".file-preview");
        if (existingPreview) {
          existingPreview.remove();
        }
        
        // Replace upload text with preview
        const uploadText = uploadArea.querySelector(".upload-text");
        const uploadHint = uploadArea.querySelector(".upload-hint");
        const uploadIcon = uploadArea.querySelector("svg");
        
        if (uploadText) uploadText.style.display = "none";
        if (uploadHint) uploadHint.style.display = "none";
        if (uploadIcon) uploadIcon.style.display = "none";
        
        uploadArea.appendChild(previewDiv);
        
        // Add remove button functionality
        const removeBtn = previewDiv.querySelector(".remove-file");
        removeBtn.addEventListener("click", (e) => {
          e.stopPropagation();
          input.value = "";
          previewDiv.remove();
          if (uploadText) uploadText.style.display = "block";
          if (uploadHint) uploadHint.style.display = "block";
          if (uploadIcon) uploadIcon.style.display = "block";
        });
      };
      reader.readAsDataURL(file);
    }
  }
  
  function formatFileSize(bytes) {
    if (bytes < 1024) return bytes + " bytes";
    else if (bytes < 1048576) return (bytes / 1024).toFixed(1) + " KB";
    else return (bytes / 1048576).toFixed(1) + " MB";
  }
}); 