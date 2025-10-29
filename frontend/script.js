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
    updateNavigation('add');
}

function showAllCourses() {
    document.getElementById('add-course-section').classList.add('hidden');
    document.getElementById('view-courses-section').classList.remove('hidden');
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