using Microsoft.AspNetCore.Mvc;

namespace CourseRegistration.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            var html = @"
<!DOCTYPE html>
<html>
<head>
    <title>Course Registration System</title>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
        }

        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
        }

        header {
            text-align: center;
            margin-bottom: 50px;
            color: white;
        }

        h1 {
            font-size: 3rem;
            margin-bottom: 10px;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.3);
        }

        .subtitle {
            font-size: 1.2rem;
            opacity: 0.9;
        }

        .card {
            background: white;
            border-radius: 15px;
            padding: 30px;
            margin: 20px 0;
            box-shadow: 0 10px 30px rgba(0,0,0,0.1);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
        }

        .card:hover {
            transform: translateY(-5px);
            box-shadow: 0 15px 40px rgba(0,0,0,0.15);
        }

        .button-group {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
            margin: 30px 0;
        }

        .btn {
            display: inline-block;
            padding: 15px 30px;
            background: linear-gradient(45deg, #667eea, #764ba2);
            color: white;
            text-decoration: none;
            border-radius: 50px;
            border: none;
            cursor: pointer;
            font-size: 1.1rem;
            font-weight: bold;
            text-align: center;
            transition: all 0.3s ease;
            box-shadow: 0 4px 15px rgba(0,0,0,0.2);
        }

        .btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(0,0,0,0.3);
            background: linear-gradient(45deg, #5a67d8, #6b46c1);
        }

        .btn:active {
            transform: translateY(0);
        }

        .search-section {
            margin-top: 40px;
        }

        .search-container {
            display: flex;
            gap: 10px;
            margin-bottom: 20px;
            flex-wrap: wrap;
        }

        .search-input {
            flex: 1;
            min-width: 250px;
            padding: 12px 20px;
            border: 2px solid #ddd;
            border-radius: 25px;
            font-size: 1rem;
            outline: none;
            transition: border-color 0.3s ease;
        }

        .search-input:focus {
            border-color: #667eea;
        }

        .btn-search {
            background: linear-gradient(45deg, #48bb78, #38a169);
        }

        .btn-search:hover {
            background: linear-gradient(45deg, #38a169, #2f855a);
        }

        .results-container {
            margin-top: 20px;
            display: none;
        }

        .certificate-display {
            background: linear-gradient(135deg, #f7fafc 0%, #edf2f7 100%);
            border: 3px solid #667eea;
            border-radius: 15px;
            padding: 40px;
            text-align: center;
            margin: 20px 0;
            position: relative;
            overflow: hidden;
        }

        .certificate-display::before {
            content: '';
            position: absolute;
            top: -50%;
            left: -50%;
            width: 200%;
            height: 200%;
            background: repeating-linear-gradient(
                45deg,
                transparent,
                transparent 10px,
                rgba(102, 126, 234, 0.1) 10px,
                rgba(102, 126, 234, 0.1) 20px
            );
            animation: shimmer 3s linear infinite;
            pointer-events: none;
        }

        @keyframes shimmer {
            0% { transform: translateX(-100%) translateY(-100%) rotate(45deg); }
            100% { transform: translateX(100%) translateY(100%) rotate(45deg); }
        }

        .certificate-header {
            font-size: 2.5rem;
            color: #667eea;
            margin-bottom: 20px;
            font-weight: bold;
            text-shadow: 1px 1px 2px rgba(0,0,0,0.1);
        }

        .certificate-content {
            position: relative;
            z-index: 1;
        }

        .certificate-name {
            font-size: 2rem;
            color: #2d3748;
            margin: 20px 0;
            font-weight: bold;
        }

        .certificate-course {
            font-size: 1.5rem;
            color: #4a5568;
            margin: 15px 0;
        }

        .certificate-details {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            margin: 30px 0;
            text-align: left;
        }

        .detail-item {
            background: white;
            padding: 15px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .detail-label {
            font-weight: bold;
            color: #667eea;
            margin-bottom: 5px;
        }

        .detail-value {
            color: #2d3748;
            font-size: 1.1rem;
        }

        .no-results {
            text-align: center;
            color: #718096;
            font-style: italic;
            padding: 20px;
        }

        .loading {
            text-align: center;
            color: #667eea;
            font-weight: bold;
        }

        @media (max-width: 768px) {
            h1 {
                font-size: 2rem;
            }
            
            .container {
                padding: 10px;
            }
            
            .card {
                padding: 20px;
            }
            
            .search-container {
                flex-direction: column;
            }
            
            .search-input {
                min-width: 100%;
            }
        }
    </style>
</head>
<body>
    <div class=""container"">
        <header>
            <h1>Course Registration System</h1>
            <p class=""subtitle"">Manage students, courses, and certificates</p>
        </header>

        <div class=""card"">
            <h2>System Navigation</h2>
            <div class=""button-group"">
                <a href=""/students"" class=""btn"">Students</a>
                <a href=""/courses"" class=""btn"">Courses</a>
                <a href=""/registrations"" class=""btn"">Registrations</a>
                <button onclick=""showCertificateSearch()"" class=""btn"">Certificate</button>
            </div>
        </div>

        <div class=""card search-section"" id=""certificateSection"" style=""display: none;"">
            <h2>Certificate Verification Portal</h2>
            <p>Enter a student name to search for their certificates:</p>
            
            <div class=""search-container"">
                <input type=""text"" id=""studentName"" class=""search-input"" placeholder=""Enter student name (e.g., John Doe, Jane Smith)"" />
                <button onclick=""searchCertificates()"" class=""btn btn-search"">Search Certificates</button>
            </div>

            <div id=""resultsContainer"" class=""results-container"">
                <div id=""loadingMessage"" class=""loading"" style=""display: none;"">Searching certificates...</div>
                <div id=""certificateResults""></div>
            </div>
        </div>
    </div>

    <script>
        function showCertificateSearch() {
            const section = document.getElementById('certificateSection');
            section.style.display = section.style.display === 'none' ? 'block' : 'none';
            
            if (section.style.display === 'block') {
                document.getElementById('studentName').focus();
            }
        }

        async function searchCertificates() {
            const studentName = document.getElementById('studentName').value.trim();
            const resultsContainer = document.getElementById('resultsContainer');
            const loadingMessage = document.getElementById('loadingMessage');
            const certificateResults = document.getElementById('certificateResults');

            if (!studentName) {
                alert('Please enter a student name');
                return;
            }

            // Show loading
            resultsContainer.style.display = 'block';
            loadingMessage.style.display = 'block';
            certificateResults.innerHTML = '';

            try {
                const response = await fetch(`/api/certificates/search?studentName=${encodeURIComponent(studentName)}`);
                const certificates = await response.json();

                loadingMessage.style.display = 'none';

                if (certificates && certificates.length > 0) {
                    certificateResults.innerHTML = certificates.map(cert => `
                        <div class=""certificate-display"">
                            <div class=""certificate-content"">
                                <div class=""certificate-header"">🎓 Certificate of Completion 🎓</div>
                                <div class=""certificate-name"">${cert.studentName}</div>
                                <div class=""certificate-course"">Successfully completed: ${cert.courseName}</div>
                                
                                <div class=""certificate-details"">
                                    <div class=""detail-item"">
                                        <div class=""detail-label"">Certificate Number</div>
                                        <div class=""detail-value"">${cert.certificateNumber}</div>
                                    </div>
                                    <div class=""detail-item"">
                                        <div class=""detail-label"">Grade</div>
                                        <div class=""detail-value"">${cert.grade}</div>
                                    </div>
                                    <div class=""detail-item"">
                                        <div class=""detail-label"">Issue Date</div>
                                        <div class=""detail-value"">${new Date(cert.issueDate).toLocaleDateString()}</div>
                                    </div>
                                    <div class=""detail-item"">
                                        <div class=""detail-label"">Credits</div>
                                        <div class=""detail-value"">${cert.credits}</div>
                                    </div>
                                </div>
                                
                                <p style=""margin-top: 30px; font-style: italic; color: #666;"">
                                    This certificate verifies that the above-named student has successfully completed 
                                    the requirements for the specified course with the grade shown above.
                                </p>
                            </div>
                        </div>
                    `).join('');
                } else {
                    certificateResults.innerHTML = '<div class=""no-results"">No certificates found for this student name.</div>';
                }
            } catch (error) {
                loadingMessage.style.display = 'none';
                certificateResults.innerHTML = '<div class=""no-results"">Error searching certificates. Please try again.</div>';
                console.error('Error:', error);
            }
        }

        // Allow Enter key to trigger search
        document.addEventListener('DOMContentLoaded', function() {
            document.getElementById('studentName').addEventListener('keypress', function(e) {
                if (e.key === 'Enter') {
                    searchCertificates();
                }
            });
        });
    </script>
</body>
</html>";

            return Content(html, "text/html");
        }
    }
}