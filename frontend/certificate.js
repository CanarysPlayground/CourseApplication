//Create a comprehensive certificate generation and display system for the Course Registration System that allows users to 
// search for and view digital certificates for completed courses.
import React, { useState, useEffect } from 'react';
import axios from 'axios';  
import { useParams } from 'react-router-dom';
import './certificate.css';

function Certificate() {
    const { certificateId } = useParams();
    const [certificate, setCertificate] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [searchQuery, setSearchQuery] = useState('');
    const [searchResults, setSearchResults] = useState([]);

    // Fetch certificate by ID using async/await
    const fetchCertificateById = async (id) => {
        setLoading(true);
        setError(null);
        try {
            const response = await axios.get(`/api/certificates/${id}`);
            setCertificate(response.data);
        } catch (err) {
            setError(err.response?.data?.message || 'Failed to fetch certificate');
            setCertificate(null);
        } finally {
            setLoading(false);
        }
    };

    // Search certificates by student name using async/await
    const searchCertificatesByName = async (studentName) => {
        if (!studentName.trim()) {
            setSearchResults([]);
            return;
        }

        setLoading(true);
        setError(null);
        try {
            const response = await axios.get(`/api/certificates/search`, {
                params: { studentName }
            });
            setSearchResults(response.data);
        } catch (err) {
            setError(err.response?.data?.message || 'Failed to search certificates');
            setSearchResults([]);
        } finally {
            setLoading(false);
        }
    };

    // Fetch certificate when certificateId is provided
    useEffect(() => {
        if (certificateId) {
            fetchCertificateById(certificateId);
        }
    }, [certificateId]);

    // Handle search form submission
    const handleSearch = async (e) => {
        e.preventDefault();
        await searchCertificatesByName(searchQuery);
    };

    // Download certificate as PDF using async/await
    const downloadCertificate = async (certId) => {
        try {
            const response = await axios.get(`/api/certificates/${certId}/download`, {
                responseType: 'blob'
            });
            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', `certificate-${certId}.pdf`);
            document.body.appendChild(link);
            link.click();
            link.remove();
        } catch (err) {
            setError('Failed to download certificate');
        }
    };

    return (
        <div className="certificate">
            <h1>Certificate Management</h1>
            
            {/* Search Form */}
            <div className="search-section">
                <form onSubmit={handleSearch}>
                    <input
                        type="text"
                        placeholder="Search by student name..."
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                    />
                    <button type="submit" disabled={loading}>Search</button>
                </form>
            </div>

            {/* Loading State */}
            {loading && <div className="loading">Loading...</div>}

            {/* Error State */}
            {error && <div className="error">{error}</div>}

            {/* Certificate Display */}
            {certificate && (
                <div className="certificate-details">
                    <h2>Certificate Details</h2>
                    <p><strong>Certificate Number:</strong> {certificate.certificateNumber}</p>
                    <p><strong>Student Name:</strong> {certificate.studentName}</p>
                    <p><strong>Course Name:</strong> {certificate.courseName}</p>
                    <p><strong>Instructor:</strong> {certificate.instructorName}</p>
                    <p><strong>Final Grade:</strong> {certificate.finalGrade}</p>
                    <p><strong>Issue Date:</strong> {new Date(certificate.issueDate).toLocaleDateString()}</p>
                    <p><strong>Course Period:</strong> {new Date(certificate.courseStartDate).toLocaleDateString()} - {new Date(certificate.courseEndDate).toLocaleDateString()}</p>
                    {certificate.remarks && <p><strong>Remarks:</strong> {certificate.remarks}</p>}
                    <button onClick={() => downloadCertificate(certificate.certificateId)}>
                        Download Certificate
                    </button>
                </div>
            )}

            {/* Search Results */}
            {searchResults.length > 0 && (
                <div className="search-results">
                    <h2>Search Results</h2>
                    <ul>
                        {searchResults.map((cert) => (
                            <li key={cert.certificateId}>
                                <div>
                                    <strong>{cert.studentName}</strong> - {cert.courseName}
                                    <br />
                                    Certificate #: {cert.certificateNumber}
                                    <br />
                                    Issued: {new Date(cert.issueDate).toLocaleDateString()}
                                </div>
                                <button onClick={() => fetchCertificateById(cert.certificateId)}>
                                    View Details
                                </button>
                            </li>
                        ))}
                    </ul>
                </div>
            )}

            {/* No Certificate Message */}
            {!certificateId && !certificate && searchResults.length === 0 && !loading && (
                <p>Search for certificates by student name or provide a certificate ID.</p>
            )}
        </div>
    );
}

export default Certificate;