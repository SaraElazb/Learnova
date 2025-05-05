document.addEventListener('DOMContentLoaded', function() {
  // Handle curriculum accordion functionality
  const curriculumItems = document.querySelectorAll('.curriculum-item');
  
  curriculumItems.forEach(item => {
    const header = item.querySelector('.curriculum-header');
    const content = item.querySelector('.curriculum-content');
    const toggle = item.querySelector('.curriculum-toggle');
    
    header.addEventListener('click', () => {
      // Check if this content is already active
      const isActive = content.classList.contains('active');
      
      // Close all accordion items first
      document.querySelectorAll('.curriculum-content').forEach(el => {
        el.classList.remove('active');
      });
      
      document.querySelectorAll('.curriculum-toggle').forEach(el => {
        el.textContent = '+';
      });
      
      // Toggle the current item
      if (!isActive) {
        content.classList.add('active');
        toggle.textContent = '−';
      }
    });
  });

  // Show mobile CTA on scroll for mobile devices
  const mobileCTA = document.querySelector('.mobile-cta');
  const courseHero = document.querySelector('.course-hero');
  
  if (window.innerWidth <= 768 && mobileCTA && courseHero) {
    window.addEventListener('scroll', () => {
      const heroBottom = courseHero.getBoundingClientRect().bottom;
      
      if (heroBottom < 0) {
        mobileCTA.style.display = 'flex';
      } else {
        mobileCTA.style.display = 'none';
      }
    });
  }

  // Smooth scroll to curriculum section
  const viewCurriculumButton = document.querySelector('.view-curriculum-btn');
  const curriculumSection = document.querySelector('.curriculum-section');
  
  if (viewCurriculumButton && curriculumSection) {
    viewCurriculumButton.addEventListener('click', (e) => {
      e.preventDefault();
      
      curriculumSection.scrollIntoView({ 
        behavior: 'smooth',
        block: 'start'
      });
    });
  }

  // Handle enrollment button click
  const enrollButtons = document.querySelectorAll('.btn-enroll');
  
  enrollButtons.forEach(button => {
    button.addEventListener('click', (e) => {
      e.preventDefault();
      
      // Add to cart functionality would go here
      // For demo, let's redirect to a cart page or show a modal
      
      // Check if we're logged in (this would be determined by your app)
      const isLoggedIn = document.body.classList.contains('logged-in');
      
      if (isLoggedIn) {
        // Add course to cart
        addToCart(button.dataset.courseId);
      } else {
        // Redirect to login
        window.location.href = '/Account/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
      }
    });
  });
  
  function addToCart(courseId) {
    // This would typically be an AJAX call to your server
    console.log('Adding course ID ' + courseId + ' to cart');
    
    // Show success message
    const successMessage = document.createElement('div');
    successMessage.className = 'cart-success-message';
    successMessage.innerHTML = `
      <div class="cart-success-content">
        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path>
          <polyline points="22 4 12 14.01 9 11.01"></polyline>
        </svg>
        <span>Course added to cart!</span>
      </div>
      <a href="/ShoppingCart" class="view-cart-btn">View Cart</a>
    `;
    
    document.body.appendChild(successMessage);
    
    // Animate in
    setTimeout(() => {
      successMessage.classList.add('show');
    }, 10);
    
    // Remove after 5 seconds
    setTimeout(() => {
      successMessage.classList.remove('show');
      setTimeout(() => {
        document.body.removeChild(successMessage);
      }, 300);
    }, 5000);
  }
}); 