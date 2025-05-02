// Mock data for the course
const courseData = {
  id: "course-123",
  title: "Advanced Web Development Masterclass",
  image:
    "https://images.unsplash.com/photo-1488590528505-98d2b5aba04b?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=80",
  rating: 4.8,
  ratingCount: 2583,
  duration: "42 hours",
  instructor: "Dr. Sarah Johnson",
  originalPrice: 129.99,
  currentPrice: 89.99,
};

// Mock promo codes
const promoCodes = {
  WELCOME25: 0.25, // 25% off
  NEWSTUDENT: 0.15, // 15% off
  FLASH50: 0.5, // 50% off
};

// DOM Elements
const checkoutButton = document.getElementById("checkout-button");
const applyPromoButton = document.getElementById("apply-promo");
const promoInput = document.getElementById("promoCode");
const promoMessage = document.getElementById("promo-message");
const discountRow = document.getElementById("discount-row");
const discountAmount = document.getElementById("discount-amount");
const totalPrice = document.getElementById("total-price");
const originalPriceElement = document.getElementById("original-price");
const coursePriceElement = document.getElementById("course-price");
const paymentSuccessModal = document.getElementById("payment-success");
const paymentFailedModal = document.getElementById("payment-failed");
const closeSuccessButton = document.getElementById("close-success");
const closeFailedButton = document.getElementById("close-failed");
const tryAgainButton = document.getElementById("try-again");

// Populate course data
document.getElementById("course-title").textContent = courseData.title;
document.getElementById("course-image").src = courseData.image;
document.getElementById("course-rating").textContent = courseData.rating;
document.getElementById("course-duration").textContent = courseData.duration;
document.getElementById("instructor-name").textContent = courseData.instructor;
originalPriceElement.textContent = `$${courseData.originalPrice.toFixed(2)}`;
coursePriceElement.textContent = `$${courseData.currentPrice.toFixed(2)}`;
totalPrice.textContent = `$${courseData.currentPrice.toFixed(2)}`;

// Current price state
let currentPrice = courseData.currentPrice;
let appliedDiscount = 0;

// Apply promo code
applyPromoButton.addEventListener("click", () => {
  const promoCode = promoInput.value.trim().toUpperCase();

  if (!promoCode) {
    showPromoMessage("Please enter a promo code", "error");
    return;
  }

  if (promoCodes.hasOwnProperty(promoCode)) {
    const discount = promoCodes[promoCode];
    appliedDiscount = courseData.currentPrice * discount;
    currentPrice = courseData.currentPrice - appliedDiscount;

    // Update UI
    discountRow.classList.remove("hidden");
    discountAmount.textContent = `-$${appliedDiscount.toFixed(2)}`;
    totalPrice.textContent = `$${currentPrice.toFixed(2)}`;

    showPromoMessage(
      `Promo code applied! You saved $${appliedDiscount.toFixed(2)}`,
      "success"
    );
  } else {
    showPromoMessage("Invalid promo code", "error");
  }
});

// Show promo message
function showPromoMessage(message, type) {
  promoMessage.textContent = message;
  promoMessage.classList.remove("hidden", "text-error", "text-accent");
  promoMessage.classList.add(type === "error" ? "text-error" : "text-accent");
}

// Handle checkout button click
checkoutButton.addEventListener("click", () => {
  const form = document.getElementById("checkout-form");
  const fullName = document.getElementById("fullName").value.trim();
  const email = document.getElementById("email").value.trim();

  // Basic validation
  if (!fullName || !email) {
    alert("Please fill in all required fields");
    return;
  }

  if (!isValidEmail(email)) {
    alert("Please enter a valid email address");
    return;
  }

  // Here you would normally call your backend to create a Stripe Checkout session
  // For demo purposes, we'll simulate this with a timeout and random success/failure
  checkoutButton.disabled = true;
  checkoutButton.innerHTML = "<span>Processing...</span>";

  setTimeout(() => {
    // Simulate successful payment 80% of the time
    const success = Math.random() > 0.2;

    if (success) {
      // Show success modal
      paymentSuccessModal.classList.remove("hidden");
    } else {
      // Show failure modal
      paymentFailedModal.classList.remove("hidden");
    }

    checkoutButton.disabled = false;
    checkoutButton.innerHTML = "<span>Complete Purchase</span>";
  }, 1500);
});

// Basic email validation
function isValidEmail(email) {
  const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return re.test(email);
}

// Modal event listeners
closeSuccessButton.addEventListener("click", () => {
  paymentSuccessModal.classList.add("hidden");
});

closeFailedButton.addEventListener("click", () => {
  paymentFailedModal.classList.add("hidden");
});

tryAgainButton.addEventListener("click", () => {
  paymentFailedModal.classList.add("hidden");
  // You could trigger the checkout process again here
});

// Check URL parameters for success or canceled flags (from Stripe redirect)
document.addEventListener("DOMContentLoaded", () => {
  const urlParams = new URLSearchParams(window.location.search);
  const paymentStatus = urlParams.get("payment");

  if (paymentStatus === "success") {
    paymentSuccessModal.classList.remove("hidden");
  } else if (paymentStatus === "canceled") {
    paymentFailedModal.classList.remove("hidden");
  }
});
