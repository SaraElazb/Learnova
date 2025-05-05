// This script handles password toggles and role selection for login/register forms
document.addEventListener('DOMContentLoaded', () => {
    // Password visibility toggle
    setupPasswordToggles();
    
    // Role selection for registration form
    setupRoleSelection();
});

// Handle password visibility toggle buttons
function setupPasswordToggles() {
    document.querySelectorAll('.toggle-password').forEach(button => {
        button.addEventListener('click', function() {
            const input = this.previousElementSibling;
            if (!input) return;
            
            const eyeIcon = this.querySelector('.eye');
            const eyeOffIcon = this.querySelector('.eye-off');

            if (input.type === 'password') {
                input.type = 'text';
                if (eyeIcon) eyeIcon.classList.add('hidden');
                if (eyeOffIcon) eyeOffIcon.classList.remove('hidden');
            } else {
                input.type = 'password';
                if (eyeIcon) eyeIcon.classList.remove('hidden');
                if (eyeOffIcon) eyeOffIcon.classList.add('hidden');
            }
        });
    });
}

// Handle role selection in registration form
function setupRoleSelection() {
    // Get role radio buttons
    const studentRadio = document.getElementById('roleStudent');
    const instructorRadio = document.getElementById('roleInstructor');
    
    // If we're not on the register page, these elements won't exist
    if (!studentRadio || !instructorRadio) return;
    
    // Get field containers
    const studentFieldsContainer = document.querySelector('.student-fields');
    const instructorFieldsContainer = document.querySelector('.instructor-fields');
    
    // If field containers don't exist, stop
    if (!studentFieldsContainer || !instructorFieldsContainer) return;
    
    // Function to toggle fields based on selected role
    function updateFieldVisibility() {
        const isStudentSelected = studentRadio.checked;
        
        // Toggle field visibility
        studentFieldsContainer.classList.toggle('hidden', !isStudentSelected);
        instructorFieldsContainer.classList.toggle('hidden', isStudentSelected);
        
        // Update field requirements
        const specializationField = document.getElementById('Specialization');
        const yearsField = document.getElementById('YearsOfExperience');
        
        if (specializationField) {
            specializationField.required = !isStudentSelected;
        }
        
        if (yearsField) {
            yearsField.required = !isStudentSelected;
        }
    }
    
    // Set initial state
    updateFieldVisibility();
    
    // Add change listeners to radio buttons
    studentRadio.addEventListener('change', updateFieldVisibility);
    instructorRadio.addEventListener('change', updateFieldVisibility);
} 