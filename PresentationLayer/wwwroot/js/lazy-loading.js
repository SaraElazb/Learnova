// Lazy Loading for Images
document.addEventListener('DOMContentLoaded', function() {
    // Initialize intersection observer
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            // If the image is in the viewport
            if (entry.isIntersecting) {
                const image = entry.target;
                // Set the src attribute to the value of data-src
                const dataSrc = image.getAttribute('data-src');
                
                if (dataSrc) {
                    // For regular images
                    image.src = dataSrc;
                    image.classList.add('loaded');
                    
                    // Clean up after loading
                    image.removeAttribute('data-src');
                    observer.unobserve(image);
                }
                
                // Handle background images
                const dataBg = image.getAttribute('data-bg');
                if (dataBg) {
                    image.style.backgroundImage = `url(${dataBg})`;
                    image.classList.add('loaded');
                    
                    // Clean up after loading
                    image.removeAttribute('data-bg');
                    observer.unobserve(image);
                }
            }
        });
    }, {
        // Options
        rootMargin: '50px 0px', // Start loading slightly before the image enters the viewport
        threshold: 0.01 // Trigger when at least 1% of the image is visible
    });
    
    // Target all images with the lazy-load class
    const lazyImages = document.querySelectorAll('.lazy-load');
    lazyImages.forEach(image => {
        imageObserver.observe(image);
    });
    
    // Fallback for browsers that don't support IntersectionObserver
    if (!('IntersectionObserver' in window)) {
        lazyImages.forEach(image => {
            if (image.getAttribute('data-src')) {
                image.src = image.getAttribute('data-src');
                image.removeAttribute('data-src');
            }
            if (image.getAttribute('data-bg')) {
                image.style.backgroundImage = `url(${image.getAttribute('data-bg')})`;
                image.removeAttribute('data-bg');
            }
            image.classList.add('loaded');
        });
    }
});

// Function to convert regular images to lazy-loaded images
function prepareLazyImages() {
    const images = document.querySelectorAll('img:not(.lazy-load)');
    
    images.forEach(img => {
        // Skip images that are already set up for lazy loading or don't have a src
        if (img.classList.contains('lazy-load') || !img.src || img.src === '') return;
        
        // Store the original src in data-src
        const originalSrc = img.src;
        img.setAttribute('data-src', originalSrc);
        
        // Add a placeholder or low-quality image if needed
        // For simplicity, we're using a transparent placeholder
        img.src = 'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1 1"%3E%3C/svg%3E';
        
        // Add the lazy-load class
        img.classList.add('lazy-load');
    });
}

// Convert background images to lazy loading
function prepareLazyBackgrounds() {
    const elements = document.querySelectorAll('[data-bg]');
    elements.forEach(el => {
        el.classList.add('lazy-load');
    });
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', function() {
    // Apply to existing images
    prepareLazyImages();
    prepareLazyBackgrounds();
}); 