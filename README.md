# ClinicOne

ClinicOne is a clinic management system developed for government hospital clinics to digitize daily clinical workflows such as patient registration, clinic scheduling, medical record management, prescription handling, and patient monitoring.

The system was developed using ASP.NET MVC with C#, Entity Framework, and SQL Server as part of our second-year final project.

---

## System Features

### Admin Portal
- Register patients, doctors, pharmacists, and admins
- Manage clinic sessions
- Manage doctor duty schedules
- Upload patient medical reports
- Monitor doctor activities through access logs
- Manage account activation and deactivation
- Password management for system users

### Doctor Portal
- Access patient medical profiles
- View medical reports and prescription history
- Create prescriptions and medical test requests
- Update and monitor patient vitals
- Track patient progress over time
- Schedule next clinic sessions for patients

### Patient Portal
- View medical reports and prescriptions
- View upcoming clinic sessions
- Track personal health progress
- Receive notifications and medicine reminders
- View doctor notes and health summaries

### Pharmacist Portal
- Search patients using NIC
- View assigned prescriptions
- Update prescription medicine status
- Generate external prescriptions for unavailable medicines
- Manage medicine distribution records

---

## Security Features

- Password hashing
- Role-based access control
- Access log monitoring (audit trail)
- Doctor access restriction based on duty schedules
- Account activation/deactivation support
- Forced password change on first login
- Temporary account lock after multiple invalid login attempts

---

## Technologies Used

- ASP.NET MVC
- C#
- Entity Framework
- SQL Server
- HTML
- CSS
- JavaScript
- Visual Studio

---

## Contributors

### Anjana Dissanayaka
- System architecture and planning
- Database design and implementation
- Admin portal development
- Doctor portal development

### Hiruni Sawbhagya 
GitHub: https://github.com/hiruni2006
- Patient portal development
- Pharmacist portal development

---

## Future Improvements

- Inter-doctor referral system
- Hospital-wide integration across departments
- Multilingual support (Sinhala/Tamil)
- SMS and email notification integration
- Mobile application support
- Enhanced authentication and security features
 
---
 
## Academic Project
---
This project was developed as a second-year undergraduate software engineering project for academic and educational purposes.
---
## Setup Requirements

### Required Software
- Visual Studio 2022
- SQL Server
- SQL Server Management Studio (SSMS)
- ASP.NET and Web Development workload

### Required NuGet Packages
- Entity Framework Core
- Entity Framework Core SQL Server
- Entity Framework Core Tools

---

## Installation

1. Clone the repository and open the solution in Visual Studio.
2. Create the database using the provided SQL script.
3. Open appsettings.json and update the connection string to match your SQL Server installation.

Example:

json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ClinicOneDB;Trusted_Connection=True;TrustServerCertificate=True;"
}


4. Run the application through Visual Studio.

---

## Default Administrator Account

The system includes a default administrator account for administration purposes.

*Username:* mainadmin@clinic.com

*Password:* Admin@123

*Note:* Users are required to change their password upon first login.


