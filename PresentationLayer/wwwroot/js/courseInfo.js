
// Wait for the DOM to be fully loaded before running JavaScript
document.addEventListener('DOMContentLoaded', function() {
  // Handle accordion functionality
  const accordionItems = document.querySelectorAll('.accordion-item');
  
  accordionItems.forEach(item => {
    const header = item.querySelector('.accordion-header');
    const content = item.querySelector('.accordion-content');
    const icon = item.querySelector('.accordion-icon');
    
    header.addEventListener('click', () => {
      // Check if this content is already active
      const isActive = content.classList.contains('active');
      
      // Close all accordion items first
      document.querySelectorAll('.accordion-content').forEach(el => {
        el.classList.remove('active');
      });
      
      document.querySelectorAll('.accordion-icon').forEach(el => {
        el.textContent = '+';
      });
      
      // Toggle the current item
      if (!isActive) {
        content.classList.add('active');
        icon.textContent = '−';
      }
    });
  });

  // Show mobile CTA on scroll for mobile devices
  const mobileCTA = document.querySelector('.mobile-cta');
  const heroSection = document.querySelector('.hero');
  
  if (window.innerWidth <= 768) {
    window.addEventListener('scroll', () => {
      const heroBottom = heroSection.getBoundingClientRect().bottom;
      
      if (heroBottom < 0) {
        mobileCTA.style.display = 'flex';
      } else {
        mobileCTA.style.display = 'none';
      }
    });
  }

  // Smooth scroll for enrollment button
  const enrollButtons = document.querySelectorAll('.btn-cta');
  
  enrollButtons.forEach(button => {
    button.addEventListener('click', (e) => {
      e.preventDefault();
      
      // This would typically redirect to enrollment page or show modal
      // For demo, let's just show an alert
      alert('Thank you for your interest! In a real application, this would take you to the enrollment page.');
    });
  });

  // Handle show all reviews button
  const showReviewsBtn = document.querySelector('.reviews-section .btn-outline');
  
  if (showReviewsBtn) {
    showReviewsBtn.addEventListener('click', () => {
      // In a real app, this would load more reviews or expand the section
      alert('In a real application, this would show more reviews from students.');
    });
  }

  // Replace Font Awesome with Unicode icons for demo purposes (since we don't load the actual Font Awesome)
  document.querySelectorAll('.fas.fa-star').forEach(el => {
    el.textContent = '★';
    el.classList.remove('fas', 'fa-star');
    el.classList.add('icon-star');
  });

  document.querySelectorAll('.fas.fa-users').forEach(el => {
    el.textContent = '👥';
    el.classList.remove('fas', 'fa-users');
    el.classList.add('icon-users');
  });

  document.querySelectorAll('.fas.fa-clock').forEach(el => {
    el.textContent = '⏱️';
    el.classList.remove('fas', 'fa-clock');
    el.classList.add('icon-clock');
  });

  document.querySelectorAll('.fas.fa-video').forEach(el => {
    el.textContent = '📹';
    el.classList.remove('fas', 'fa-video');
    el.classList.add('icon-video');
  });

  document.querySelectorAll('.fas.fa-file').forEach(el => {
    el.textContent = '📄';
    el.classList.remove('fas', 'fa-file');
    el.classList.add('icon-file');
  });

  document.querySelectorAll('.fas.fa-code').forEach(el => {
    el.textContent = '💻';
    el.classList.remove('fas', 'fa-code');
    el.classList.add('icon-code');
  });

  document.querySelectorAll('.fas.fa-certificate').forEach(el => {
    el.textContent = '🏆';
    el.classList.remove('fas', 'fa-certificate');
    el.classList.add('icon-certificate');
  });

  document.querySelectorAll('.fas.fa-mobile-alt').forEach(el => {
    el.textContent = '📱';
    el.classList.remove('fas', 'fa-mobile-alt');
    el.classList.add('icon-mobile');
  });
});
