USE ClinicOne_Database;

CREATE TABLE UserAccount (
	UserAccountID INT IDENTITY(1,1) PRIMARY KEY,
	Username NVARCHAR(50) UNIQUE NOT NULL,
	PasswordHash NVARCHAR(255) NOT NULL,
	[Role] VARCHAR(20) NOT NULL
        CHECK ([Role] IN ('Patient','Doctor','Admin','Pharmacist')),
	IsLocked BIT  NOT NULL DEFAULT 0,
	FailedAttempts INT NOT NULL DEFAULT 0,
	LastLogin DATETIME NULL
);

CREATE TABLE Patient (
	PatientNIC VARCHAR(20) PRIMARY KEY,
	FullName NVARCHAR(100) NOT NULL,
	UserAccountID INT UNIQUE,
	[Address] NVARCHAR(200) NOT NULL,
	BloodPressure NVARCHAR(20),
	PhoneNumber VARCHAR(15) NOT NULL,
	Height DECIMAL(5,2),
    [Weight] DECIMAL(5,2),
	BloodType CHAR(3),
	Gender CHAR(1) NOT NULL
        CHECK (Gender IN ('M','F')),
	DOB DATE NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,

    FOREIGN KEY (UserAccountID) REFERENCES UserAccount(UserAccountID)

);
CREATE TABLE Doctor (
	DoctorID INT IDENTITY(1,1) PRIMARY KEY,
	FullName NVARCHAR(100) NOT NULL,
	UserAccountID INT UNIQUE,
	Specialization NVARCHAR(70) NOT NULL,
	RegistrationNumber NVARCHAR(50) UNIQUE NOT NULL,
	IsActive BIT NOT NULL DEFAULT 1, 

    FOREIGN KEY (UserAccountID) REFERENCES UserAccount(UserAccountID)
);
CREATE TABLE [Admin] (
	AdminID INT IDENTITY(1,1) PRIMARY KEY,
	UserAccountID INT UNIQUE,
	[Name] NVARCHAR(100) NOT NULL,
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
	Email NVARCHAR(100) UNIQUE NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1

    FOREIGN KEY (UserAccountID) REFERENCES UserAccount(UserAccountID)
);
CREATE TABLE Pharmacist (
	PharmacistID INT IDENTITY(1,1) PRIMARY KEY,
	UserAccountID INT UNIQUE,
	[Name] NVARCHAR(100) NOT NULL,
	RegistrationNumber NVARCHAR(50) UNIQUE NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1

    FOREIGN KEY (UserAccountID) REFERENCES UserAccount(UserAccountID)
);
CREATE TABLE MedicalReport (
    ReportID INT IDENTITY(1,1) PRIMARY KEY,
    PatientNIC VARCHAR(20) NOT NULL,
    ReportDate DATE NOT NULL,
    UploadedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ReportPath NVARCHAR(255) NOT NULL,

    FOREIGN KEY (PatientNIC) REFERENCES Patient(PatientNIC)
);
CREATE TABLE TestType (
    TestTypeID INT IDENTITY(1,1) PRIMARY KEY,
    TestName NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(255),
    Unit NVARCHAR(20)
);
CREATE TABLE TestRange (
    RangeID INT IDENTITY(1,1) PRIMARY KEY,
    TestTypeID INT NOT NULL,
    NormalMin DECIMAL(10,2),
    NormalMax DECIMAL(10,2),
    HighMin DECIMAL(10,2),
    HighMax DECIMAL(10,2),
    RiskMin DECIMAL(10,2),
    RiskMax DECIMAL(10,2),

    FOREIGN KEY (TestTypeID) REFERENCES TestType(TestTypeID)
);
CREATE TABLE ReportTestResult (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    ReportID INT NOT NULL,
    TestTypeID INT NOT NULL,
    TestValue DECIMAL(10,2) NOT NULL,
    ResultStatus VARCHAR(20)
        CHECK (ResultStatus IN ('Normal','High','Risk')),

    FOREIGN KEY (ReportID) REFERENCES MedicalReport(ReportID),
    FOREIGN KEY (TestTypeID) REFERENCES TestType(TestTypeID)
);
CREATE TABLE Prescription (
    PrescriptionID INT IDENTITY(1,1) PRIMARY KEY,
    PatientNIC VARCHAR(20) NOT NULL,
    PrescriptionDate DATE NOT NULL,
    Notes NVARCHAR(500),

    FOREIGN KEY (PatientNIC) REFERENCES Patient(PatientNIC)
);
CREATE TABLE PrescriptionMedicine (
    PrescMedID INT IDENTITY(1,1) PRIMARY KEY,
    PrescriptionID INT NOT NULL,
    MedicineName NVARCHAR(150) NOT NULL,
    [Status] VARCHAR(20) NOT NULL
        CHECK (Status IN ('Given','Not Given','Partially Given')),
    Reason NVARCHAR(200),
    Dosage NVARCHAR(100),
    Duration NVARCHAR(50),
    PatientConfirmed BIT NOT NULL DEFAULT 0,

    FOREIGN KEY (PrescriptionID) REFERENCES Prescription(PrescriptionID)
);
CREATE TABLE ClinicSession (
    SessionID INT IDENTITY(1,1) PRIMARY KEY,
    SessionName NVARCHAR(100) NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    MaxSlots INT NOT NULL
);
CREATE TABLE ClinicSchedule (
    ScheduleID INT IDENTITY(1,1) PRIMARY KEY,
    PatientNIC VARCHAR(20) NOT NULL,
    SessionID INT NOT NULL,
    ClinicDate DATE NOT NULL,
    AssignedDate DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (PatientNIC) REFERENCES Patient(PatientNIC),
    FOREIGN KEY (SessionID) REFERENCES ClinicSession(SessionID)
);
CREATE TABLE [Notification] (
    NotificationID INT IDENTITY(1,1) PRIMARY KEY,
    ScheduleID INT NOT NULL,
    PatientNIC VARCHAR(20) NOT NULL,
    [Message] NVARCHAR(500) NOT NULL,
    SentDate DATETIME NOT NULL DEFAULT GETDATE(),
    IsRead BIT NOT NULL DEFAULT 0,

    FOREIGN KEY (ScheduleID) REFERENCES ClinicSchedule(ScheduleID),
    FOREIGN KEY (PatientNIC) REFERENCES Patient(PatientNIC)
);
CREATE TABLE PatientProgress (
    ProgressID INT IDENTITY(1,1) PRIMARY KEY,
    PatientNIC VARCHAR(20) NOT NULL,
    ReportID INT NOT NULL,
    ProgressStatus VARCHAR(20) NOT NULL
        CHECK (ProgressStatus IN ('Improving','Stable','Worsening')),
    IsConfirmed BIT NOT NULL DEFAULT 0,
    DoctorNotes NVARCHAR(500),
    RecordedDate DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (PatientNIC) REFERENCES Patient(PatientNIC),
    FOREIGN KEY (ReportID) REFERENCES MedicalReport(ReportID)
);
CREATE TABLE MedicineReminder (
    ReminderID INT IDENTITY(1,1) PRIMARY KEY,
    PatientNIC VARCHAR(20) NOT NULL,
    PrescMedID INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,

    FOREIGN KEY (PatientNIC) REFERENCES Patient(PatientNIC),
    FOREIGN KEY (PrescMedID) REFERENCES PrescriptionMedicine(PrescMedID)
);
CREATE TABLE ExternalPrescription (
    ExternalPresID INT IDENTITY(1,1) PRIMARY KEY,
    GeneratedDate DATETIME NOT NULL DEFAULT GETDATE(),
    PrescriptionID INT NOT NULL,
    PDFPath NVARCHAR(255) NOT NULL,

    FOREIGN KEY (PrescriptionID) REFERENCES Prescription(PrescriptionID)
);
CREATE TABLE PrescribedTest (
    PrescribedTestID INT IDENTITY(1,1) PRIMARY KEY,
    TestTypeID INT NOT NULL,
    TestCategory NVARCHAR(50),
    OrderDate DATE NOT NULL,
    PrescriptionID INT NOT NULL,
    Notes NVARCHAR(300),
    [Status] VARCHAR(20) NOT NULL,

    FOREIGN KEY (TestTypeID) REFERENCES TestType(TestTypeID),
    FOREIGN KEY (PrescriptionID) REFERENCES Prescription(PrescriptionID)
);
CREATE TABLE DoctorSchedule (
    DutyID INT IDENTITY(1,1) PRIMARY KEY,
    DoctorID INT NOT NULL,
    SessionID INT NOT NULL,
    ClinicDate DATE NOT NULL,

    FOREIGN KEY (DoctorID) REFERENCES Doctor(DoctorID),
    FOREIGN KEY (SessionID) REFERENCES ClinicSession(SessionID)
);
CREATE TABLE AccessLog (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    DoctorID INT NOT NULL,
    PatientNIC VARCHAR(20) NOT NULL,
    [Action] NVARCHAR(100) NOT NULL
        CHECK ([Action] IN ('View','Update','Prescribe')),
    AccessDateTime DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (DoctorID) REFERENCES Doctor(DoctorID),
    FOREIGN KEY (PatientNIC) REFERENCES Patient(PatientNIC)
);

