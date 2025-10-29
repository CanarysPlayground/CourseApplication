// Configuration
const API_BASE_URL = 'http://localhost:5210/api';

// DOM Elements
const addCourseForm = document.getElementById('add-course-form');
const submitBtn = document.getElementById('submit-btn');
const submitText = document.getElementById('submit-text');
const loadingOverlay = document.getElementById('loading-overlay');

// Initialize the application
document.addEventListener('DOMContentLoaded', function() {
    initializeForm();
    loadCourses();
    updateNavigation();
});

// Form Initialization
function initializeForm() {
    // Set minimum date to tomorrow
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const minDate = tomorrow.toISOString().slice(0, 16);
    document.getElementById('startDate').min = minDate;
    
    // Character counter for description
    const descriptionField = document.getElementById('description');
    const characterCount = document.getElementById('description-count');
    
    descriptionField.addEventListener('input', function() {
        const count = this.value.length;
        characterCount.textContent = count;
        
        if (count > 450) {
            characterCount.style.color = '#ef4444';
        } else if (count > 400) {
            characterCount.style.color = '#f59e0b';
        } else {
            characterCount.style.color = '#64748b';
        }
    });

    // Start date change handler
    document.getElementById('startDate').addEventListener('change', function() {
        const startDate = new Date(this.value);
        const endDateField = document.getElementById('endDate');
        
        if (startDate) {
            // Set minimum end date to one day after start date
            const minEndDate = new Date(startDate);
            minEndDate.setDate(minEndDate.getDate() + 1);
            endDateField.min = minEndDate.toISOString().slice(0, 16);
            
            // Clear end date if it's before the new minimum
            if (endDateField.value && new Date(endDateField.value) <= startDate) {
                endDateField.value = '';
            }
        }
    });

    // Form submission
    addCourseForm.addEventListener('submit', handleFormSubmit);

    // Instructor name validation (letters, spaces, periods only)
    document.getElementById('instructorName').addEventListener('input', function(e) {
        const value = e.target.value;
        const validPattern = /^[a-zA-Z\s.]*$/;
        
        if (!validPattern.test(value)) {
            e.target.value = value.replace(/[^a-zA-Z\s.]/g, '');
        }
    });

    // Real-time validation
    const formFields = ['courseName', 'instructorName', 'startDate', 'endDate', 'schedule'];
    formFields.forEach(fieldName => {
        const field = document.getElementById(fieldName);
        field.addEventListener('blur', () => validateField(fieldName));
        field.addEventListener('input', () => clearFieldError(fieldName));
    });
}

// Form Submission Handler
async function handleFormSubmit(e) {
    e.preventDefault();
    
    if (!validateForm()) {
        return;
    }

    const formData = getFormData();
    
    try {
        showLoading(true);
        setSubmitButton(true, 'Creating Course...');
        
        const response = await fetch(`${API_BASE_URL}/courses`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(formData)
        });

        const result = await response.json();

        if (response.ok) {
            showSuccessModal(`Course "${formData.courseName}" has been created successfully!`);
            resetForm();
            loadCourses(); // Refresh the courses list
        } else {
            const errorMessage = result.message || result.title || 'Failed to create course';
            
            // Handle validation errors
            if (result.errors && typeof result.errors === 'object') {
                displayValidationErrors(result.errors);
            } else {
                showErrorModal(errorMessage);
            }
        }
    } catch (error) {
        console.error('Error creating course:', error);
        showErrorModal('Network error. Please check your connection and try again.');
    } finally {
        showLoading(false);
        setSubmitButton(false, 'Add Course');
    }
}

// Get Form Data
function getFormData() {
    return {
        courseName: document.getElementById('courseName').value.trim(),
        description: document.getElementById('description').value.trim() || null,
        instructorName: document.getElementById('instructorName').value.trim(),
        startDate: new Date(document.getElementById('startDate').value).toISOString(),
        endDate: new Date(document.getElementById('endDate').value).toISOString(),
        schedule: document.getElementById('schedule').value.trim()
    };
}

// Form Validation
function validateForm() {
    let isValid = true;
    
    // Clear previous errors
    clearAllErrors();
    
    // Validate course name
    if (!validateField('courseName')) isValid = false;
    
    // Validate instructor name
    if (!validateField('instructorName')) isValid = false;
    
    // Validate dates
    if (!validateField('startDate')) isValid = false;
    if (!validateField('endDate')) isValid = false;
    
    // Validate schedule
    if (!validateField('schedule')) isValid = false;
    
    // Cross-field validation
    const startDate = new Date(document.getElementById('startDate').value);
    const endDate = new Date(document.getElementById('endDate').value);
    
    if (startDate && endDate && endDate <= startDate) {
        showFieldError('endDate', 'End date must be after start date');
        isValid = false;
    }
    
    if (startDate && startDate <= new Date()) {
        showFieldError('startDate', 'Start date must be in the future');
        isValid = false;
    }
    
    return isValid;
}

function validateField(fieldName) {
    const field = document.getElementById(fieldName);
    const value = field.value.trim();
    
    switch (fieldName) {
        case 'courseName':
            if (!value) {
                showFieldError(fieldName, 'Course name is required');
                return false;
            }
            if (value.length > 100) {
                showFieldError(fieldName, 'Course name cannot exceed 100 characters');
                return false;
            }
            break;
            
        case 'instructorName':
            if (!value) {
                showFieldError(fieldName, 'Instructor name is required');
                return false;
            }
            if (value.length > 100) {
                showFieldError(fieldName, 'Instructor name cannot exceed 100 characters');
                return false;
            }
            if (!/^[a-zA-Z\s.]+$/.test(value)) {
                showFieldError(fieldName, 'Instructor name can only contain letters, spaces, and periods');
                return false;
            }
            break;
            
        case 'startDate':
            if (!value) {
                showFieldError(fieldName, 'Start date is required');
                return false;
            }
            break;
            
        case 'endDate':
            if (!value) {
                showFieldError(fieldName, 'End date is required');
                return false;
            }
            break;
            
        case 'schedule':
            if (!value) {
                showFieldError(fieldName, 'Schedule is required');
                return false;
            }
            break;
    }
    
    return true;
}

// Error Display Functions
function showFieldError(fieldName, message) {
    const field = document.getElementById(fieldName);
    const errorElement = document.getElementById(`${fieldName}-error`);
    
    field.classList.add('error');
    errorElement.textContent = message;
}

function clearFieldError(fieldName) {
    const field = document.getElementById(fieldName);
    const errorElement = document.getElementById(`${fieldName}-error`);
    
    field.classList.remove('error');
    errorElement.textContent = '';
}

function clearAllErrors() {
    const errorElements = document.querySelectorAll('.error-message');
    const inputElements = document.querySelectorAll('.form-input, .form-textarea');
    
    errorElements.forEach(el => el.textContent = '');
    inputElements.forEach(el => el.classList.remove('error'));
}

function displayValidationErrors(errors) {
    Object.keys(errors).forEach(fieldName => {
        const message = Array.isArray(errors[fieldName]) 
            ? errors[fieldName][0] 
            : errors[fieldName];
        
        // Convert field names to match our form IDs
        const mappedFieldName = mapFieldName(fieldName);
        if (mappedFieldName) {
            showFieldError(mappedFieldName, message);
        }
    });
}

function mapFieldName(apiFieldName) {
    const fieldMap = {
        'CourseName': 'courseName',
        'InstructorName': 'instructorName',
        'StartDate': 'startDate',
        'EndDate': 'endDate',
        'Schedule': 'schedule',
        'Description': 'description'
    };
    
    return fieldMap[apiFieldName] || apiFieldName.toLowerCase();
}

// UI State Functions
function setSubmitButton(loading, text) {
    submitBtn.disabled = loading;
    submitText.textContent = text;
    
    if (loading) {
        submitBtn.innerHTML = `<i class="fas fa-spinner fa-spin"></i> ${text}`;
    } else {
        submitBtn.innerHTML = `<i class="fas fa-plus"></i> ${text}`;
    }
}

function showLoading(show) {
    loadingOverlay.classList.toggle('hidden', !show);
}

// Modal Functions
function showSuccessModal(message) {
    document.getElementById('success-message').textContent = message;
    document.getElementById('success-modal').classList.remove('hidden');
}

function showErrorModal(message) {
    document.getElementById('error-message').textContent = message;
    document.getElementById('error-modal').classList.remove('hidden');
}

function closeModal(modalId) {
    document.getElementById(modalId).classList.add('hidden');
}

// Form Reset
function resetForm() {
    addCourseForm.reset();
    clearAllErrors();
    document.getElementById('description-count').textContent = '0';
    document.getElementById('description-count').style.color = '#64748b';
    
    // Reset date minimums
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const minDate = tomorrow.toISOString().slice(0, 16);
    document.getElementById('startDate').min = minDate;
    document.getElementById('endDate').min = '';
}

// Navigation Functions
function showAddCourse() {
    document.getElementById('add-course-section').classList.remove('hidden');
    document.getElementById('view-courses-section').classList.add('hidden');
    document.getElementById('registrations-section').classList.add('hidden');
    updateNavigation('add');
}

function showAllCourses() {
    document.getElementById('add-course-section').classList.add('hidden');
    document.getElementById('view-courses-section').classList.remove('hidden');
    document.getElementById('registrations-section').classList.add('hidden');
    updateNavigation('view');
    loadCourses();
}

function updateNavigation(activeSection = 'add') {
    const navLinks = document.querySelectorAll('.nav-link');
    navLinks.forEach(link => link.classList.remove('active'));
    
    if (activeSection === 'add') {
        navLinks[0].classList.add('active');
    } else if (activeSection === 'view') {
        navLinks[1].classList.add('active');
    } else if (activeSection === 'registrations') {
        navLinks[3].classList.add('active');
    }
}

// Course Loading Functions
async function loadCourses() {
    const coursesContainer = document.getElementById('courses-container');
    const coursesLoading = document.getElementById('courses-loading');
    const coursesEmpty = document.getElementById('courses-empty');
    
    try {
        coursesLoading.classList.remove('hidden');
        coursesContainer.innerHTML = '';
        coursesEmpty.classList.add('hidden');
        
        const response = await fetch(`${API_BASE_URL}/courses?pageSize=100`);
        const result = await response.json();
        
        if (response.ok && result.data && result.data.items) {
            const courses = result.data.items;
            
            if (courses.length === 0) {
                coursesEmpty.classList.remove('hidden');
            } else {
                displayCourses(courses);
            }
        } else {
            throw new Error('Failed to load courses');
        }
    } catch (error) {
        console.error('Error loading courses:', error);
        coursesContainer.innerHTML = `
            <div class="error-state">
                <i class="fas fa-exclamation-triangle"></i>
                <h3>Error Loading Courses</h3>
                <p>Unable to load courses. Please try again.</p>
                <button class="btn btn-primary" onclick="loadCourses()">
                    <i class="fas fa-retry"></i> Retry
                </button>
            </div>
        `;
    } finally {
        coursesLoading.classList.add('hidden');
    }
}

function displayCourses(courses) {
    const coursesContainer = document.getElementById('courses-container');
    
    coursesContainer.innerHTML = courses.map(course => `
        <div class="course-card" onclick="showCourseDetails('${course.courseId}')">
            <h3>${escapeHtml(course.courseName)}</h3>
            <div class="instructor">
                <i class="fas fa-chalkboard-teacher"></i>
                ${escapeHtml(course.instructorName)}
            </div>
            
            ${course.description ? `
                <div class="description">
                    ${escapeHtml(course.description)}
                </div>
            ` : ''}
            
            <div class="details">
                <div class="course-detail">
                    <i class="fas fa-calendar-alt"></i>
                    ${formatDate(course.startDate)} - ${formatDate(course.endDate)}
                </div>
                <div class="course-detail">
                    <i class="fas fa-clock"></i>
                    ${escapeHtml(course.schedule)}
                </div>
                <div class="course-detail">
                    <i class="fas fa-calendar-plus"></i>
                    Created: ${formatDate(course.createdAt)}
                </div>
            </div>
            
            <div class="course-status ${getCourseStatus(course.startDate, course.endDate)}">
                ${getCourseStatusText(course.startDate, course.endDate)}
            </div>
        </div>
    `).join('');
}

// Course Details Modal
async function showCourseDetails(courseId) {
    try {
        const response = await fetch(`${API_BASE_URL}/courses/${courseId}`);
        const result = await response.json();
        
        if (response.ok && result.data) {
            const course = result.data;
            const modal = document.getElementById('course-details-modal');
            const content = document.getElementById('course-details-content');
            
            content.innerHTML = `
                <div class="course-details">
                    <h3>${escapeHtml(course.courseName)}</h3>
                    
                    <div class="detail-section">
                        <h4><i class="fas fa-chalkboard-teacher"></i> Instructor</h4>
                        <p>${escapeHtml(course.instructorName)}</p>
                    </div>
                    
                    ${course.description ? `
                        <div class="detail-section">
                            <h4><i class="fas fa-align-left"></i> Description</h4>
                            <p>${escapeHtml(course.description)}</p>
                        </div>
                    ` : ''}
                    
                    <div class="detail-section">
                        <h4><i class="fas fa-calendar-alt"></i> Duration</h4>
                        <p><strong>Start:</strong> ${formatDateTime(course.startDate)}</p>
                        <p><strong>End:</strong> ${formatDateTime(course.endDate)}</p>
                    </div>
                    
                    <div class="detail-section">
                        <h4><i class="fas fa-clock"></i> Schedule</h4>
                        <p>${escapeHtml(course.schedule)}</p>
                    </div>
                    
                    <div class="detail-section">
                        <h4><i class="fas fa-info-circle"></i> Course Information</h4>
                        <p><strong>Course ID:</strong> ${course.courseId}</p>
                        <p><strong>Status:</strong> <span class="course-status ${getCourseStatus(course.startDate, course.endDate)}">${getCourseStatusText(course.startDate, course.endDate)}</span></p>
                        <p><strong>Created:</strong> ${formatDateTime(course.createdAt)}</p>
                        <p><strong>Last Updated:</strong> ${formatDateTime(course.updatedAt)}</p>
                    </div>
                </div>
            `;
            
            modal.classList.remove('hidden');
        }
    } catch (error) {
        console.error('Error loading course details:', error);
        showErrorModal('Failed to load course details');
    }
}

// Search and Filter
function filterCourses() {
    const searchTerm = document.getElementById('search-input').value.toLowerCase();
    const courseCards = document.querySelectorAll('.course-card');
    
    courseCards.forEach(card => {
        const courseName = card.querySelector('h3').textContent.toLowerCase();
        const instructor = card.querySelector('.instructor').textContent.toLowerCase();
        const description = card.querySelector('.description')?.textContent.toLowerCase() || '';
        
        const matches = courseName.includes(searchTerm) || 
                       instructor.includes(searchTerm) || 
                       description.includes(searchTerm);
        
        card.style.display = matches ? 'block' : 'none';
    });
}

function refreshCourses() {
    loadCourses();
}

// Utility Functions
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });
}

function formatDateTime(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

function getCourseStatus(startDate, endDate) {
    const now = new Date();
    const start = new Date(startDate);
    const end = new Date(endDate);
    
    if (now < start) return 'upcoming';
    if (now > end) return 'completed';
    return 'active';
}

function getCourseStatusText(startDate, endDate) {
    const status = getCourseStatus(startDate, endDate);
    
    switch (status) {
        case 'upcoming': return 'Upcoming';
        case 'active': return 'Active';
        case 'completed': return 'Completed';
        default: return 'Unknown';
    }
}

// Keyboard Navigation
document.addEventListener('keydown', function(e) {
    // Close modals with Escape key
    if (e.key === 'Escape') {
        const openModals = document.querySelectorAll('.modal:not(.hidden)');
        openModals.forEach(modal => modal.classList.add('hidden'));
    }
});

// Click outside modal to close
document.addEventListener('click', function(e) {
    if (e.target.classList.contains('modal')) {
        e.target.classList.add('hidden');
    }
});

// Export functions for global access
window.showAddCourse = showAddCourse;
window.showAllCourses = showAllCourses;
window.resetForm = resetForm;
window.closeModal = closeModal;
window.refreshCourses = refreshCourses;
window.filterCourses = filterCourses;
window.showCourseDetails = showCourseDetails;

// ============================================================================
// REGISTRATIONS FUNCTIONALITY
// ============================================================================

let allRegistrations = [];
let allStudents = [];
let allCourses = [];

// Navigation to Registrations Section
function showRegistrations() {
    document.getElementById('add-course-section').classList.add('hidden');
    document.getElementById('view-courses-section').classList.add('hidden');
    document.getElementById('registrations-section').classList.remove('hidden');
    updateNavigation('registrations');
    loadRegistrations();
}

// Load all registrations
async function loadRegistrations() {
    const registrationsContainer = document.getElementById('registrations-container');
    const registrationsLoading = document.getElementById('registrations-loading');
    const registrationsEmpty = document.getElementById('registrations-empty');
    
    try {
        registrationsLoading.classList.remove('hidden');
        registrationsContainer.innerHTML = '';
        registrationsEmpty.classList.add('hidden');
        
        const response = await fetch(`${API_BASE_URL}/registrations?pageSize=100`);
        const result = await response.json();
        
        if (response.ok && result.data && result.data.items) {
            allRegistrations = result.data.items;
            
            if (allRegistrations.length === 0) {
                registrationsEmpty.classList.remove('hidden');
            } else {
                displayRegistrations(allRegistrations);
            }
        } else {
            throw new Error('Failed to load registrations');
        }
    } catch (error) {
        console.error('Error loading registrations:', error);
        registrationsContainer.innerHTML = `
            <div class="error-state">
                <i class="fas fa-exclamation-triangle"></i>
                <h3>Error Loading Registrations</h3>
                <p>Unable to load registrations. Please try again.</p>
                <button class="btn btn-primary" onclick="loadRegistrations()">
                    <i class="fas fa-retry"></i> Retry
                </button>
            </div>
        `;
    } finally {
        registrationsLoading.classList.add('hidden');
    }
}

// Display registrations
function displayRegistrations(registrations) {
    const registrationsContainer = document.getElementById('registrations-container');
    
    registrationsContainer.innerHTML = registrations.map(registration => {
        const studentName = registration.student 
            ? registration.student.fullName 
            : 'Unknown Student';
        const courseName = registration.course 
            ? registration.course.courseName 
            : 'Unknown Course';
        const statusClass = getRegistrationStatusClass(registration.status);
        const statusText = getRegistrationStatusText(registration.status);
        
        return `
            <div class="registration-card" onclick="showRegistrationDetails('${registration.registrationId}')">
                <div class="registration-header">
                    <div class="student-info">
                        <div class="student-name">
                            <i class="fas fa-user"></i>
                            ${escapeHtml(studentName)}
                        </div>
                        <div class="course-name">
                            <i class="fas fa-book"></i>
                            ${escapeHtml(courseName)}
                        </div>
                    </div>
                    <span class="registration-status ${statusClass}">
                        ${statusText}
                    </span>
                </div>
                
                <div class="registration-info">
                    <div class="registration-info-item">
                        <i class="fas fa-calendar-alt"></i>
                        Registered: ${formatDate(registration.registrationDate)}
                    </div>
                    ${registration.grade ? `
                        <div class="registration-info-item">
                            <i class="fas fa-graduation-cap"></i>
                            Grade: ${registration.grade}
                        </div>
                    ` : ''}
                    ${registration.notes ? `
                        <div class="registration-info-item">
                            <i class="fas fa-sticky-note"></i>
                            ${escapeHtml(registration.notes)}
                        </div>
                    ` : ''}
                </div>
            </div>
        `;
    }).join('');
}

// Show create registration modal
async function showCreateRegistration() {
    const modal = document.getElementById('create-registration-modal');
    
    // Load students and courses for dropdowns
    await loadStudentsForDropdown();
    await loadCoursesForDropdown();
    
    // Clear form
    document.getElementById('create-registration-form').reset();
    
    modal.classList.remove('hidden');
}

// Load students for dropdown
async function loadStudentsForDropdown() {
    try {
        const response = await fetch(`${API_BASE_URL}/students?pageSize=100`);
        const result = await response.json();
        
        if (response.ok && result.data && result.data.items) {
            allStudents = result.data.items;
            const select = document.getElementById('registration-student');
            select.innerHTML = '<option value="">Select a student...</option>' + 
                allStudents.map(student => 
                    `<option value="${student.studentId}">${escapeHtml(student.fullName)} - ${escapeHtml(student.email)}</option>`
                ).join('');
        }
    } catch (error) {
        console.error('Error loading students:', error);
    }
}

// Load courses for dropdown
async function loadCoursesForDropdown() {
    try {
        const response = await fetch(`${API_BASE_URL}/courses?pageSize=100`);
        const result = await response.json();
        
        if (response.ok && result.data && result.data.items) {
            allCourses = result.data.items;
            const select = document.getElementById('registration-course');
            select.innerHTML = '<option value="">Select a course...</option>' + 
                allCourses.map(course => 
                    `<option value="${course.courseId}">${escapeHtml(course.courseName)} - ${escapeHtml(course.instructorName)}</option>`
                ).join('');
        }
    } catch (error) {
        console.error('Error loading courses:', error);
    }
}

// Handle create registration form submission
document.addEventListener('DOMContentLoaded', function() {
    const createRegistrationForm = document.getElementById('create-registration-form');
    if (createRegistrationForm) {
        createRegistrationForm.addEventListener('submit', async function(e) {
            e.preventDefault();
            
            const formData = {
                studentId: document.getElementById('registration-student').value,
                courseId: document.getElementById('registration-course').value,
                notes: document.getElementById('registration-notes').value.trim() || null
            };
            
            if (!formData.studentId || !formData.courseId) {
                showErrorModal('Please select both a student and a course.');
                return;
            }
            
            try {
                showLoading(true);
                
                const response = await fetch(`${API_BASE_URL}/registrations`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(formData)
                });
                
                const result = await response.json();
                
                if (response.ok) {
                    closeModal('create-registration-modal');
                    showSuccessModal('Registration created successfully!');
                    loadRegistrations(); // Refresh the list
                } else {
                    const errorMessage = result.message || result.title || 'Failed to create registration';
                    showErrorModal(errorMessage);
                }
            } catch (error) {
                console.error('Error creating registration:', error);
                showErrorModal('Network error. Please check your connection and try again.');
            } finally {
                showLoading(false);
            }
        });
    }
});

// Show registration details
async function showRegistrationDetails(registrationId) {
    try {
        const response = await fetch(`${API_BASE_URL}/registrations/${registrationId}`);
        const result = await response.json();
        
        if (response.ok && result.data) {
            const registration = result.data;
            const modal = document.getElementById('registration-details-modal');
            const content = document.getElementById('registration-details-content');
            
            const studentName = registration.student ? registration.student.fullName : 'Unknown Student';
            const studentEmail = registration.student ? registration.student.email : 'N/A';
            const courseName = registration.course ? registration.course.courseName : 'Unknown Course';
            const instructorName = registration.course ? registration.course.instructorName : 'N/A';
            const statusClass = getRegistrationStatusClass(registration.status);
            const statusText = getRegistrationStatusText(registration.status);
            
            content.innerHTML = `
                <div class="course-details">
                    <div class="detail-section">
                        <h4><i class="fas fa-user"></i> Student Information</h4>
                        <p><strong>Name:</strong> ${escapeHtml(studentName)}</p>
                        <p><strong>Email:</strong> ${escapeHtml(studentEmail)}</p>
                    </div>
                    
                    <div class="detail-section">
                        <h4><i class="fas fa-book"></i> Course Information</h4>
                        <p><strong>Course:</strong> ${escapeHtml(courseName)}</p>
                        <p><strong>Instructor:</strong> ${escapeHtml(instructorName)}</p>
                    </div>
                    
                    <div class="detail-section">
                        <h4><i class="fas fa-info-circle"></i> Registration Details</h4>
                        <p><strong>Registration ID:</strong> ${registration.registrationId}</p>
                        <p><strong>Status:</strong> <span class="registration-status ${statusClass}">${statusText}</span></p>
                        <p><strong>Registration Date:</strong> ${formatDateTime(registration.registrationDate)}</p>
                        ${registration.grade ? `<p><strong>Grade:</strong> ${registration.grade}</p>` : ''}
                        ${registration.notes ? `<p><strong>Notes:</strong> ${escapeHtml(registration.notes)}</p>` : ''}
                    </div>
                </div>
            `;
            
            modal.classList.remove('hidden');
        }
    } catch (error) {
        console.error('Error loading registration details:', error);
        showErrorModal('Failed to load registration details');
    }
}

// Apply filters
function applyFilters() {
    const statusFilter = document.getElementById('filter-status').value;
    
    let filteredRegistrations = allRegistrations;
    
    if (statusFilter !== '') {
        filteredRegistrations = filteredRegistrations.filter(reg => 
            reg.status.toString() === statusFilter
        );
    }
    
    displayRegistrations(filteredRegistrations);
}

// Refresh registrations
function refreshRegistrations() {
    loadRegistrations();
}

// Helper function to get registration status class
function getRegistrationStatusClass(status) {
    switch (status) {
        case 0: return 'pending';
        case 1: return 'confirmed';
        case 2: return 'cancelled';
        case 3: return 'completed';
        default: return 'pending';
    }
}

// Helper function to get registration status text
function getRegistrationStatusText(status) {
    switch (status) {
        case 0: return 'Pending';
        case 1: return 'Confirmed';
        case 2: return 'Cancelled';
        case 3: return 'Completed';
        default: return 'Unknown';
    }
}

// Export new functions
window.showRegistrations = showRegistrations;
window.showCreateRegistration = showCreateRegistration;
window.showRegistrationDetails = showRegistrationDetails;
window.refreshRegistrations = refreshRegistrations;
window.applyFilters = applyFilters;