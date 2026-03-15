USE ClinicOne_Database;

CREATE TABLE UserAccount (
	UserAccountID INT IDENTITY(1,1) PRIMARY KEY,
	Username NVARCHAR(50) UNIQUE NOT NULL,
	PasswordHash NVARCHAR(255) NOT NULL,
	[Role] VARCHAR(20) NOT NULL
        CHECK ([Role] IN ('Patient','Doctor','Admin','Pharmacist')),
	IsLocked BIT  NOT NULL DEFAULT 0,
	FailedAttempts INT NOT NULL DEFAULT 0,
	LastLogin DATETIME NULL,
    LockUntil DATETIME NULL,
    FirstLogin BIT NOT NULL DEFAULT 1
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
CREATE TABLE TestPanel (
    PanelID INT IDENTITY(1,1) PRIMARY KEY,
    TestName NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(255)
);
CREATE TABLE TestParameter (
    ParameterID INT IDENTITY(1,1) PRIMARY KEY,
    PanelID INT NOT NULL,
    ParameterName NVARCHAR(100) NOT NULL,
    Unit NVARCHAR(20),

    FOREIGN KEY (PanelID) REFERENCES TestPanel(PanelID)
);
CREATE TABLE TestRange (
    RangeID INT IDENTITY(1,1) PRIMARY KEY,
    ParameterID INT NOT NULL,
    Gender CHAR(1) NULL,
    ReferenceMin DECIMAL(10,2),
    ReferenceMax DECIMAL(10,2),
    CriticalLow DECIMAL(10,2) NULL,
    CriticalHigh DECIMAL (10,2) NULL,

    FOREIGN KEY (ParameterID) REFERENCES TestParameter(ParameterID)
);
CREATE TABLE ReportTestResult (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    ReportID INT NOT NULL,
    ParameterID INT NOT NULL,
    TestValue DECIMAL(10,2) NOT NULL,
    ResultStatus VARCHAR(20)
        CHECK (ResultStatus IN ('Normal','High','Risk')),

    FOREIGN KEY (ReportID) REFERENCES MedicalReport(ReportID),
    FOREIGN KEY (ParameterID) REFERENCES TestParameter(ParameterID)
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
    TimesPerDay INT NOT NULL,

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
    ProgressDate DATE NOT NULL,
    ProgressStatus VARCHAR(20) NOT NULL
        CHECK (ProgressStatus IN ('Improving','Stable','Worsening')),
    IsConfirmed BIT NOT NULL DEFAULT 0,
    DoctorNotes NVARCHAR(500),
    RecordedDate DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (PatientNIC) REFERENCES Patient(PatientNIC)
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
    PanelID INT NOT NULL,
    TestCategory NVARCHAR(50),
    OrderDate DATE NOT NULL,
    PrescriptionID INT NOT NULL,
    Notes NVARCHAR(300),
    [Status] VARCHAR(20) NOT NULL,

    FOREIGN KEY (PanelID) REFERENCES TestPanel(PanelID),
    FOREIGN KEY (PrescriptionID) REFERENCES Prescription(PrescriptionID)
);
CREATE TABLE DoctorDutySchedule (
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

--Admin--
INSERT INTO UserAccount (Username, PasswordHash, Role, FirstLogin)
VALUES ('mainadmin@clinic.com',
        '6G94qKPK8LYNjnTllCqm2G3BUM08AzOK7yW30tfjrMc=',
        'Admin',
        1);

INSERT INTO Admin (UserAccountID, Name, Email)
VALUES (1,'Main Admin','mainadmin@clinic.com');
-- Test full types
INSERT INTO TestPanel (TestName, Description)
VALUES
('Full Blood Count', 'Complete blood count test'),
('Lipid Profile', 'Cholesterol and fat levels'),
('Blood Sugar Panel', 'Glucose related tests'),
('Kidney Function Test', 'Kidney health markers'),
('Liver Function Test', 'Liver enzymes and bilirubin'),
('Thyroid Profile', 'Thyroid hormone levels'),
('Electrolytes', 'Electrolyte balance test');


-- test parameter types 

INSERT INTO TestParameter (PanelID, ParameterName, Unit) VALUES
(1,'Hemoglobin','g/dL'),
(1,'WBC','10^9/L'),
(1,'RBC','10^12/L'),
(1,'Platelets','10^9/L'),
(1,'Hematocrit','%');

INSERT INTO TestParameter (PanelID, ParameterName, Unit) VALUES
(2,'Total Cholesterol','mg/dL'),
(2,'LDL','mg/dL'),
(2,'HDL','mg/dL'),
(2,'Triglycerides','mg/dL');

INSERT INTO TestParameter (PanelID, ParameterName, Unit) VALUES
(3,'Fasting Blood Sugar','mg/dL'),
(3,'Random Blood Sugar','mg/dL'),
(3,'HbA1c','%');

INSERT INTO TestParameter (PanelID, ParameterName, Unit) VALUES
(4,'Creatinine','mg/dL'),
(4,'Urea','mg/dL'),
(4,'eGFR','mL/min/1.73m2');

INSERT INTO TestParameter (PanelID, ParameterName, Unit) VALUES
(5,'ALT','U/L'),
(5,'AST','U/L'),
(5,'ALP','U/L'),
(5,'Bilirubin','mg/dL');

INSERT INTO TestParameter (PanelID, ParameterName, Unit) VALUES
(6,'TSH','mIU/L'),
(6,'T3','ng/dL'),
(6,'T4','µg/dL');

INSERT INTO TestParameter (PanelID, ParameterName, Unit) VALUES
(7,'Sodium','mmol/L'),
(7,'Potassium','mmol/L'),
(7,'Chloride','mmol/L');

-- ranges
-- 1 Hemoglobin (Male)
INSERT INTO TestRange (ParameterID, Gender, ReferenceMin, ReferenceMax, CriticalLow, CriticalHigh)
VALUES (1, 'M', 13.5, 17.5, 7.0, 20.0);

-- 1 Hemoglobin (Female)
INSERT INTO TestRange (ParameterID, Gender, ReferenceMin, ReferenceMax, CriticalLow, CriticalHigh)
VALUES (1, 'F', 12.0, 15.5, 7.0, 20.0);

-- 2 WBC
INSERT INTO TestRange VALUES (2, NULL, 4.0, 11.0, 1.0, 30.0);

-- 3 RBC (Male)
INSERT INTO TestRange VALUES (3, 'M', 4.5, 5.9, 2.5, 7.5);

-- 3 RBC (Female)
INSERT INTO TestRange VALUES (3, 'F', 4.1, 5.1, 2.5, 7.5);

-- 4 Platelets
INSERT INTO TestRange VALUES (4, NULL, 150, 450, 20, 1000);

-- 5 Hematocrit (Male)
INSERT INTO TestRange VALUES (5, 'M', 41, 53, 20, 60);

-- 5 Hematocrit (Female)
INSERT INTO TestRange VALUES (5, 'F', 36, 46, 20, 60);

-- 6 Total Cholesterol
INSERT INTO TestRange VALUES (6, NULL, 0, 200, NULL, 400);

-- 7 LDL
INSERT INTO TestRange VALUES (7, NULL, 0, 100, NULL, 250);

-- 8 HDL (Male)
INSERT INTO TestRange VALUES (8, 'M', 40, 60, 20, NULL);

-- 8 HDL (Female)
INSERT INTO TestRange VALUES (8, 'F', 50, 60, 20, NULL);

-- 9 Triglycerides
INSERT INTO TestRange VALUES (9, NULL, 0, 150, NULL, 500);

-- 10 Fasting Blood Sugar
INSERT INTO TestRange VALUES (10, NULL, 70, 99, 40, 400);

-- 11 Random Blood Sugar
INSERT INTO TestRange VALUES (11, NULL, 70, 140, 40, 400);

-- 12 HbA1c
INSERT INTO TestRange VALUES (12, NULL, 4.0, 5.6, NULL, 10);

-- 13 Creatinine (Male)
INSERT INTO TestRange VALUES (13, 'M', 0.7, 1.3, NULL, 5);

-- 13 Creatinine (Female)
INSERT INTO TestRange VALUES (13, 'F', 0.6, 1.1, NULL, 5);

-- 14 Urea
INSERT INTO TestRange VALUES (14, NULL, 7, 20, NULL, 100);

-- 15 eGFR
INSERT INTO TestRange VALUES (15, NULL, 90, 200, 15, NULL);

-- 16 ALT
INSERT INTO TestRange VALUES (16, NULL, 7, 56, NULL, 1000);

-- 17 AST
INSERT INTO TestRange VALUES (17, NULL, 10, 40, NULL, 1000);

-- 18 ALP
INSERT INTO TestRange VALUES (18, NULL, 44, 147, NULL, 1000);

-- 19 Bilirubin
INSERT INTO TestRange VALUES (19, NULL, 0.1, 1.2, NULL, 20);

-- 20 TSH
INSERT INTO TestRange VALUES (20, NULL, 0.4, 4.0, 0.01, 20);

-- 21 T3
INSERT INTO TestRange VALUES (21, NULL, 80, 200, NULL, 400);

-- 22 T4
INSERT INTO TestRange VALUES (22, NULL, 5, 12, NULL, 25);

-- 23 Sodium
INSERT INTO TestRange VALUES (23, NULL, 135, 145, 120, 160);

-- 24 Potassium
INSERT INTO TestRange VALUES (24, NULL, 3.5, 5.0, 2.5, 6.5);

-- 25 Chloride
INSERT INTO TestRange VALUES (25, NULL, 98, 106, 80, 120);

