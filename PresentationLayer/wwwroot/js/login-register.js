// DOM Elements
const loginForm = document.getElementById('loginForm');
const registerForm = document.getElementById('registerForm');
const togglePasswordButtons = document.querySelectorAll('.toggle-password');

// Initialize forms and attach event listeners
document.addEventListener('DOMContentLoaded', function () {
    initializePasswordToggles();

    if (loginForm) {
        initializeLoginForm();
    }

    if (registerForm) {
        initializeRegisterForm();
        initializeRoleSelection();
    }
});

// Password visibility toggle functionality
function initializePasswordToggles() {
    togglePasswordButtons.forEach(button => {
        button.addEventListener('click', function () {
            const input = this.parentNode.querySelector('input');
            const eyeIcon = this.querySelector('.eye');
            const eyeOffIcon = this.querySelector('.eye-off');

            if (input.type === 'password') {
                input.type = 'text';
                eyeIcon.classList.add('hidden');
                eyeOffIcon.classList.remove('hidden');
            } else {
                input.type = 'password';
                eyeIcon.classList.remove('hidden');
                eyeOffIcon.classList.add('hidden');
            }
        });
    });
}

// Login form functionality
function initializeLoginForm() {
    loginForm.addEventListener('submit', function (e) {
        e.preventDefault();

        // Reset previous errors
        resetErrors(this);

        // Get form values
        const email = this.email.value.trim();
        const password = this.password.value;
        const remember = this.remember?.checked || false;

        // Validate form
        let isValid = true;

        if (!email) {
            showError(this.email, 'Email is required');
            isValid = false;
        } else if (!isValidEmail(email)) {
            showError(this.email, 'Please enter a valid email address');
            isValid = false;
        }

        if (!password) {
            showError(this.password, 'Password is required');
            isValid = false;
        }

        if (isValid) {
            // Show loading state
            const submitBtn = this.querySelector('button[type="submit"]');
            const btnText = submitBtn.querySelector('.btn-text');
            const spinner = submitBtn.querySelector('.spinner');

            submitBtn.disabled = true;
            btnText.textContent = 'Logging in...';
            spinner.classList.remove('hidden');

            // Simulate API call
            setTimeout(() => {
                // In a real application, you would send the data to your server
                console.log('Login form submitted:', { email, password, remember });

                // Reset form after successful submission (in a real app, you'd redirect)
                alert('Login successful! Redirecting to dashboard...');

                // Reset button state
                submitBtn.disabled = false;
                btnText.textContent = 'Log In';
                spinner.classList.add('hidden');

                // For demo purposes - you'd normally redirect the user
                // window.location.href = '/dashboard';
            }, 1500);
        }
    });
}

// Initialize role-specific fields toggle
function initializeRoleSelection() {
    const roleStudent = document.getElementById('roleStudent');
    const roleInstructor = document.getElementById('roleInstructor');
    const studentFields = document.querySelector('.student-fields');
    const instructorFields = document.querySelector('.instructor-fields');

    // Set initial state based on default selection
    toggleRoleFields(roleStudent.checked);

    // Add event listeners to radio buttons
    roleStudent.addEventListener('change', function () {
        toggleRoleFields(true);
    });

    roleInstructor.addEventListener('change', function () {
        toggleRoleFields(false);
    });

    // Toggle the appropriate fields
    function toggleRoleFields(isStudentRole) {
        if (isStudentRole) {
            studentFields.classList.remove('hidden');
            instructorFields.classList.add('hidden');
        } else {
            studentFields.classList.add('hidden');
            instructorFields.classList.remove('hidden');
        }
    }
}

// Register form functionality
function initializeRegisterForm() {
    registerForm.addEventListener('submit', function (e) {
        e.preventDefault();

        // Reset previous errors
        resetErrors(this);

        // Get form values
        const fullName = this.fullName.value.trim();
        const email = this.email.value.trim();
        const password = this.password.value;
        const confirmPassword = this.confirmPassword.value;
        const role = this.role.value; // Get selected role value

        // Get role-specific fields based on selection
        let roleSpecificData = {};
        if (role === 'student') {
            roleSpecificData = {
                nationalId: this.nationalId.value.trim(),
                grade: this.grade.value.trim()
            };
        } else if (role === 'instructor') {
            roleSpecificData = {
                specialization: this.specialization.value.trim(),
                experience: this.experience.value.trim()
            };
        }

        // Validate form
        let isValid = true;

        if (!fullName) {
            showError(this.fullName, 'Full name is required');
            isValid = false;
        }

        if (!email) {
            showError(this.email, 'Email is required');
            isValid = false;
        } else if (!isValidEmail(email)) {
            showError(this.email, 'Please enter a valid email address');
            isValid = false;
        }

        if (!password) {
            showError(this.password, 'Password is required');
            isValid = false;
        } else if (password.length < 8) {
            showError(this.password, 'Password must be at least 8 characters');
            isValid = false;
        }

        if (!confirmPassword) {
            showError(this.confirmPassword, 'Please confirm your password');
            isValid = false;
        } else if (password !== confirmPassword) {
            showError(this.confirmPassword, 'Passwords do not match');
            isValid = false;
        }

        // Validate role-specific fields
        if (role === 'student') {
            if (!roleSpecificData.nationalId) {
                showError(this.nationalId, 'National ID is required');
                isValid = false;
            }
            if (!roleSpecificData.grade) {
                showError(this.grade, 'Grade is required');
                isValid = false;
            }
        } else if (role === 'instructor') {
            if (!roleSpecificData.specialization) {
                showError(this.specialization, 'Specialization is required');
                isValid = false;
            }
            if (!roleSpecificData.experience) {
                showError(this.experience, 'Years of experience is required');
                isValid = false;
            }
        }

        if (isValid) {
            // Show loading state
            const submitBtn = this.querySelector('button[type="submit"]');
            const btnText = submitBtn.querySelector('.btn-text');
            const spinner = submitBtn.querySelector('.spinner');

            submitBtn.disabled = true;
            btnText.textContent = 'Creating account...';
            spinner.classList.remove('hidden');

            // Simulate API call
            setTimeout(() => {
                // In a real application, you would send the data to your server
                console.log('Register form submitted:', {
                    fullName,
                    email,
                    password,
                    role,
                    ...roleSpecificData
                });

                // Reset form after successful submission (in a real app, you'd redirect)
                alert('Account created successfully! Please log in.');

                // Reset button state
                submitBtn.disabled = false;
                btnText.textContent = 'Sign Up';
                spinner.classList.add('hidden');

                // For demo purposes - you'd normally redirect the user
                // window.location.href = '/login';
            }, 1500);
        }
    });
}

// Helper function to validate email format
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

// Show error on invalid input
function showError(input, message) {
    input.classList.add('error');
    const errorMessage = input.parentNode.parentNode.querySelector('.error-message');
    if (errorMessage) {
        errorMessage.textContent = message;
        errorMessage.style.display = 'block';
    }
}

// Reset all errors
function resetErrors(form) {
    const inputs = form.querySelectorAll('input');
    inputs.forEach(input => {
        input.classList.remove('error');
        const errorMessage = input.parentNode.parentNode.querySelector('.error-message');
        if (errorMessage) {
            errorMessage.textContent = '';
            errorMessage.style.display = 'none';
        }
    });
}

// Add input validation on blur
document.addEventListener('DOMContentLoaded', function () {
    const inputs = document.querySelectorAll('input');

    inputs.forEach(input => {
        input.addEventListener('blur', function () {
            validateInput(this);
        });

        input.addEventListener('input', function () {
            if (this.classList.contains('error')) {
                validateInput(this);
            }
        });
    });
});

// Validate individual input
function validateInput(input) {
    // Reset previous errors
    input.classList.remove('error');
    const errorMessage = input.parentNode.parentNode.querySelector('.error-message');
    if (errorMessage) {
        errorMessage.textContent = '';
        errorMessage.style.display = 'none';
    }

    const value = input.value.trim();

    switch (input.id) {
        case 'email':
        case 'registerEmail':
            if (!value) {
                showError(input, 'Email is required');
            } else if (!isValidEmail(value)) {
                showError(input, 'Please enter a valid email address');
            }
            break;

        case 'password':
        case 'registerPassword':
            if (!value) {
                showError(input, 'Password is required');
            } else if (value.length < 8) {
                showError(input, 'Password must be at least 8 characters');
            }
            break;

        case 'confirmPassword':
            const password = document.getElementById('registerPassword').value;
            if (!value) {
                showError(input, 'Please confirm your password');
            } else if (value !== password) {
                showError(input, 'Passwords do not match');
            }
            break;

        case 'fullName':
            if (!value) {
                showError(input, 'Full name is required');
            }
            break;

        case 'nationalId':
            if (!value) {
                showError(input, 'National ID is required');
            }
            break;

        case 'grade':
            if (!value) {
                showError(input, 'Grade is required');
            }
            break;

        case 'specialization':
            if (!value) {
                showError(input, 'Specialization is required');
            }
            break;

        case 'experience':
            if (!value) {
                showError(input, 'Years of experience is required');
            }
            break;
    }
}
