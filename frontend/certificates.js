// Certificate Search and Display System
const API_BASE_URL = 'http://localhost:5210/api';

// DOM Elements
const searchForm = document.getElementById('certificate-search-form');
const searchStudentName = document.getElementById('searchStudentName');
const searchCertificateNumber = document.getElementById('searchCertificateNumber');
const searchResultsContainer = document.getElementById('search-results-container');
const certificatesGrid = document.getElementById('certificates-grid');
const searchLoading = document.getElementById('search-loading');
const searchEmpty = document.getElementById('search-empty');
const resultsCount = document.getElementById('results-count');
const certificateViewerModal = document.getElementById('certificate-viewer-modal');
const certificateDisplay = document.getElementById('certificate-display');
const loadingOverlay = document.getElementById('loading-overlay');

// Current certificate being viewed
let currentCertificate = null;

// Initialize the application
document.addEventListener('DOMContentLoaded', function() {
    initializeSearchForm();
});

// Initialize search form
function initializeSearchForm() {
    searchForm.addEventListener('submit', handleSearchSubmit);
    
    // Clear search results when inputs change
    searchStudentName.addEventListener('input', () => {
        if (!searchStudentName.value && !searchCertificateNumber.value) {
            hideSearchResults();
        }
    });
    
    searchCertificateNumber.addEventListener('input', () => {
        if (!searchStudentName.value && !searchCertificateNumber.value) {
            hideSearchResults();
        }
    });
}

// Handle search form submission
async function handleSearchSubmit(e) {
    e.preventDefault();
    
    const studentName = searchStudentName.value.trim();
    const certificateNumber = searchCertificateNumber.value.trim();
    
    if (!studentName && !certificateNumber) {
        showError('Please enter at least one search criterion');
        return;
    }
    
    await searchCertificates(studentName, certificateNumber);
}

// Search for certificates
async function searchCertificates(studentName, certificateNumber) {
    try {
        showLoading(true);
        hideSearchResults();
        searchEmpty.classList.add('hidden');
        
        // Build query parameters
        const params = new URLSearchParams();
        if (studentName) params.append('studentName', studentName);
        if (certificateNumber) params.append('certificateNumber', certificateNumber);
        
        const response = await fetch(`${API_BASE_URL}/certificates?${params.toString()}`);
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const certificates = await response.json();
        
        showLoading(false);
        
        if (certificates.length === 0) {
            searchEmpty.classList.remove('hidden');
        } else {
            displaySearchResults(certificates);
        }
    } catch (error) {
        console.error('Error searching certificates:', error);
        showLoading(false);
        showError('Failed to search certificates. Please try again.');
    }
}

// Display search results
function displaySearchResults(certificates) {
    certificatesGrid.innerHTML = '';
    
    certificates.forEach(certificate => {
        const card = createCertificateCard(certificate);
        certificatesGrid.appendChild(card);
    });
    
    resultsCount.textContent = `Found ${certificates.length} certificate${certificates.length !== 1 ? 's' : ''}`;
    searchResultsContainer.classList.remove('hidden');
}

// Create a certificate card
function createCertificateCard(certificate) {
    const card = document.createElement('div');
    card.className = 'certificate-card';
    card.onclick = () => viewCertificate(certificate);
    
    const issueDate = new Date(certificate.issueDate).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    });
    
    const gradeClass = `grade-${certificate.finalGrade}`;
    const gradeDisplay = getGradeDisplay(certificate.finalGrade);
    
    card.innerHTML = `
        <div class="certificate-card-header">
            <span class="certificate-number">
                <i class="fas fa-hashtag"></i>
                ${escapeHtml(certificate.certificateNumber)}
            </span>
            <div class="certificate-grade ${gradeClass}">${gradeDisplay}</div>
        </div>
        <div class="certificate-card-body">
            <h3 class="certificate-student-name">${escapeHtml(certificate.studentName)}</h3>
            <p class="certificate-course-name">${escapeHtml(certificate.courseName)}</p>
        </div>
        <div class="certificate-meta">
            <div class="certificate-meta-item">
                <i class="fas fa-calendar-alt"></i>
                <span>Issued: ${issueDate}</span>
            </div>
            <div class="certificate-meta-item">
                <i class="fas fa-chalkboard-teacher"></i>
                <span>${escapeHtml(certificate.instructorName)}</span>
            </div>
            ${certificate.remarks ? `
                <div class="certificate-meta-item">
                    <i class="fas fa-comment"></i>
                    <span>${escapeHtml(certificate.remarks)}</span>
                </div>
            ` : ''}
        </div>
    `;
    
    return card;
}

// View certificate details
function viewCertificate(certificate) {
    currentCertificate = certificate;
    certificateDisplay.innerHTML = renderCertificate(certificate);
    certificateViewerModal.classList.remove('hidden');
}

// Render certificate HTML
function renderCertificate(certificate) {
    const issueDate = new Date(certificate.issueDate).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    });
    
    const courseStartDate = new Date(certificate.courseStartDate).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long'
    });
    
    const courseEndDate = new Date(certificate.courseEndDate).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long'
    });
    
    const gradeDisplay = getGradeDisplay(certificate.finalGrade);
    
    return `
        <div class="certificate-ornament"></div>
        
        <div class="certificate-header">
            <div class="certificate-logo">
                <i class="fas fa-graduation-cap"></i>
            </div>
            <h1 class="certificate-title">Certificate of Completion</h1>
            <p class="certificate-subtitle">This is to certify that</p>
        </div>
        
        <div class="certificate-body">
            <h2 class="certificate-student">${escapeHtml(certificate.studentName)}</h2>
            <p class="certificate-description">
                has successfully completed the course
                <span class="certificate-course">${escapeHtml(certificate.courseName)}</span>
                with a final grade of <strong>${gradeDisplay}</strong>
                ${certificate.remarks ? `<br><em>"${escapeHtml(certificate.remarks)}"</em>` : ''}
            </p>
        </div>
        
        <div class="certificate-details">
            <div class="certificate-detail-item">
                <div class="certificate-detail-label">Certificate Number</div>
                <div class="certificate-detail-value">${escapeHtml(certificate.certificateNumber)}</div>
            </div>
            <div class="certificate-detail-item">
                <div class="certificate-detail-label">Issue Date</div>
                <div class="certificate-detail-value">${issueDate}</div>
            </div>
            <div class="certificate-detail-item">
                <div class="certificate-detail-label">Course Duration</div>
                <div class="certificate-detail-value">${courseStartDate} - ${courseEndDate}</div>
            </div>
            <div class="certificate-detail-item">
                <div class="certificate-detail-label">Instructor</div>
                <div class="certificate-detail-value">${escapeHtml(certificate.instructorName)}</div>
            </div>
        </div>
        
        <div class="certificate-footer">
            <div class="certificate-signature">
                <div class="certificate-signature-line"></div>
                <div class="certificate-signature-name">${escapeHtml(certificate.instructorName)}</div>
                <div class="certificate-signature-title">Course Instructor</div>
            </div>
            
            <div class="certificate-qr-section">
                <div class="certificate-qr-code">
                    <i class="fas fa-qrcode certificate-qr-placeholder"></i>
                </div>
                <div class="certificate-verification">
                    <small>Scan to verify</small>
                </div>
            </div>
        </div>
        
        ${certificate.digitalSignature ? `
            <div class="certificate-digital-signature">
                Digital Signature: ${escapeHtml(certificate.digitalSignature)}
            </div>
        ` : ''}
    `;
}

// Get grade display text
function getGradeDisplay(grade) {
    const gradeMap = {
        0: 'A (Excellent)',
        1: 'B (Good)',
        2: 'C (Satisfactory)',
        3: 'D (Poor)',
        4: 'F (Fail)'
    };
    
    return gradeMap[grade] || grade.toString();
}

// Close certificate viewer
function closeCertificateViewer() {
    certificateViewerModal.classList.add('hidden');
    currentCertificate = null;
}

// Print certificate
function printCertificate() {
    if (!currentCertificate) return;
    window.print();
}

// Download certificate as PDF
function downloadCertificate() {
    if (!currentCertificate) return;
    
    // For now, use print dialog
    // In a production app, you'd generate a proper PDF using a library like jsPDF
    showSuccess('Please use the print dialog to save as PDF');
    printCertificate();
}

// Reset search form
function resetSearchForm() {
    searchStudentName.value = '';
    searchCertificateNumber.value = '';
    hideSearchResults();
    searchEmpty.classList.add('hidden');
}

// Hide search results
function hideSearchResults() {
    searchResultsContainer.classList.add('hidden');
    certificatesGrid.innerHTML = '';
}

// Show/hide loading overlay
function showLoading(show) {
    if (show) {
        searchLoading.classList.remove('hidden');
    } else {
        searchLoading.classList.add('hidden');
    }
}

// Show success message
function showSuccess(message) {
    const successModal = document.getElementById('success-modal');
    const successMessage = document.getElementById('success-message');
    successMessage.textContent = message;
    successModal.classList.remove('hidden');
}

// Show error message
function showError(message) {
    const errorModal = document.getElementById('error-modal');
    const errorMessage = document.getElementById('error-message');
    errorMessage.textContent = message;
    errorModal.classList.remove('hidden');
}

// Close modal
function closeModal(modalId) {
    const modal = document.getElementById(modalId);
    modal.classList.add('hidden');
}

// Escape HTML to prevent XSS
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}
