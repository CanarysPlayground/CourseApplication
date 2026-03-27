//Create a comprehensive certificate generation and display system for the Course Registration System that allows users to 
// search for and view digital certificates for completed courses.
import React, { useState, useEffect } from 'react';
import axios from 'axios';  
import { useParams } from 'react-router-dom';
import './certificate.css';
//function for the certificate
function Certificate() {
    const { certificateId } = useParams();

    return (
        <div className="certificate">
            <h1>Certificate</h1>
            {certificateId ? (
                <p>Certificate ID: {certificateId}</p>
            ) : (
                <p>No certificate selected.</p>
            )}
        </div>
    );
}

export default Certificate;
