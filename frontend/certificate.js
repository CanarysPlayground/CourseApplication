//Create a comprehensive certificate generation and display system for the Course Registration System that allows users to 
// search for and view digital certificates for completed courses.
import React, { useState } from 'react';
import axios from 'axios';  
import { useParams } from 'react-router-dom';
import './certificate.css';

function Certificate() {
    const { certificateId } = useParams();
    const [downloading, setDownloading] = useState(false);
    const [downloadError, setDownloadError] = useState(null);

    const handleDownload = async () => {
        if (!certificateId) return;

        setDownloading(true);
        setDownloadError(null);

        try {
            const encodedId = encodeURIComponent(certificateId);
            const response = await axios.get(
                `/api/certificates/${encodedId}/download`,
                { responseType: 'blob' }
            );

            const url = window.URL.createObjectURL(new Blob([response.data], { type: 'application/pdf' }));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', `certificate-${certificateId}.pdf`);
            document.body.appendChild(link);
            link.click();
            link.parentNode.removeChild(link);
            window.URL.revokeObjectURL(url);
        } catch (error) {
            if (error.response) {
                if (error.response.status === 404) {
                    setDownloadError('Certificate not found. Please verify the certificate ID.');
                } else {
                    setDownloadError('Failed to download certificate. Please try again later.');
                }
            } else {
                setDownloadError('Network error. Please check your connection and try again.');
            }
        } finally {
            setDownloading(false);
        }
    };

    return (
        <div className="certificate">
            <h1>Certificate</h1>
            {certificateId ? (
                <div>
                    <p>Certificate ID: {certificateId}</p>
                    <button
                        onClick={handleDownload}
                        disabled={downloading}
                        className="download-btn"
                    >
                        {downloading ? 'Downloading...' : 'Download PDF'}
                    </button>
                    {downloadError && (
                        <p className="download-error" role="alert">{downloadError}</p>
                    )}
                </div>
            ) : (
                <p>No certificate selected.</p>
            )}
        </div>
    );
}

export default Certificate;