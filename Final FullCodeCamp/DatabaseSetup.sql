-- ============================================================
-- APU CodeCamp Management System
-- Full Database Setup Script
-- Run this in SQL Server Management Studio (SSMS)
-- ============================================================

USE master;
GO

-- Drop and recreate database
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'APUCodeCampDB')
BEGIN
    ALTER DATABASE APUCodeCampDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE APUCodeCampDB;
END
GO

CREATE DATABASE APUCodeCampDB;
GO

USE APUCodeCampDB;
GO

-- ============================================================
-- TABLE: Users (base table for all roles)
-- ============================================================
CREATE TABLE Users (
    UserID    INT           IDENTITY(1,1) PRIMARY KEY,
    Username  NVARCHAR(50)  NOT NULL UNIQUE,
    Password  NVARCHAR(100) NOT NULL,
    Role      NVARCHAR(20)  NOT NULL CHECK (Role IN ('Administrator','Lecturer','Trainer','Student')),
    Name      NVARCHAR(100) NOT NULL,
    Email     NVARCHAR(100) NOT NULL,
    Phone     NVARCHAR(20)  NOT NULL,
    Address   NVARCHAR(255) NOT NULL,
    IsActive  BIT           NOT NULL DEFAULT 1,
    CreatedAt DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
-- TABLE: Administrators
-- ============================================================
CREATE TABLE Administrators (
    AdminID INT IDENTITY(1,1) PRIMARY KEY,
    UserID  INT NOT NULL REFERENCES Users(UserID),
    StaffID NVARCHAR(20) NOT NULL UNIQUE
);
GO

-- ============================================================
-- TABLE: Modules
-- ============================================================
CREATE TABLE Modules (
    ModuleID   INT           IDENTITY(1,1) PRIMARY KEY,
    ModuleCode NVARCHAR(20)  NOT NULL UNIQUE,
    ModuleName NVARCHAR(100) NOT NULL,
    IsActive   BIT           NOT NULL DEFAULT 1
);
GO

-- ============================================================
-- TABLE: Lecturers
-- ============================================================
CREATE TABLE Lecturers (
    LecturerID INT          IDENTITY(1,1) PRIMARY KEY,
    UserID     INT          NOT NULL REFERENCES Users(UserID),
    StaffID    NVARCHAR(20) NOT NULL UNIQUE
);
GO

-- ============================================================
-- TABLE: Trainers
-- ============================================================
CREATE TABLE Trainers (
    TrainerID   INT          IDENTITY(1,1) PRIMARY KEY,
    UserID      INT          NOT NULL REFERENCES Users(UserID),
    StaffID     NVARCHAR(20) NOT NULL UNIQUE,
    Specialisation NVARCHAR(100) NOT NULL DEFAULT '',
    IsActive    BIT          NOT NULL DEFAULT 1
);
GO

-- ============================================================
-- TABLE: TrainerModules (Trainer assigned to a module+level)
-- One trainer can only teach one module at a time
-- ============================================================
CREATE TABLE TrainerModules (
    TrainerModuleID INT          IDENTITY(1,1) PRIMARY KEY,
    TrainerID       INT          NOT NULL REFERENCES Trainers(TrainerID),
    ModuleID        INT          NOT NULL REFERENCES Modules(ModuleID),
    ClassLevel      NVARCHAR(20) NOT NULL CHECK (ClassLevel IN ('Beginner','Intermediate','Advance')),
    AssignedDate    DATETIME     NOT NULL DEFAULT GETDATE(),
    IsActive        BIT          NOT NULL DEFAULT 1,
    UNIQUE (TrainerID, ModuleID, ClassLevel)
);
GO

-- ============================================================
-- TABLE: Students
-- ============================================================
CREATE TABLE Students (
    StudentID  INT           IDENTITY(1,1) PRIMARY KEY,
    UserID     INT           NOT NULL REFERENCES Users(UserID),
    TPNumber   NVARCHAR(20)  NOT NULL UNIQUE,
    StudyLevel NVARCHAR(20)  NOT NULL CHECK (StudyLevel IN ('Foundation','Level 1','Level 2','Level 3'))
);
GO

-- ============================================================
-- TABLE: Classes (a specific coaching class instance)
-- ============================================================
CREATE TABLE Classes (
    ClassID    INT            IDENTITY(1,1) PRIMARY KEY,
    TrainerID  INT            NOT NULL REFERENCES Trainers(TrainerID),
    ModuleID   INT            NOT NULL REFERENCES Modules(ModuleID),
    ClassLevel NVARCHAR(20)   NOT NULL CHECK (ClassLevel IN ('Beginner','Intermediate','Advance')),
    Schedule   NVARCHAR(200)  NOT NULL,  -- e.g. "Mon/Wed 2pm-4pm"
    Venue      NVARCHAR(100)  NOT NULL,
    Fee        DECIMAL(10,2)  NOT NULL,
    StartDate  DATE           NOT NULL,
    EndDate    DATE           NOT NULL,
    MaxStudents INT           NOT NULL DEFAULT 20,
    IsActive   BIT            NOT NULL DEFAULT 1,
    CreatedAt  DATETIME       NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
-- TABLE: Enrolments (student enrolled in a class)
-- ============================================================
CREATE TABLE Enrolments (
    EnrolmentID     INT          IDENTITY(1,1) PRIMARY KEY,
    StudentID       INT          NOT NULL REFERENCES Students(StudentID),
    ClassID         INT          NOT NULL REFERENCES Classes(ClassID),
    MonthOfEnrolment NVARCHAR(20) NOT NULL,  -- e.g. "March 2026"
    PaymentStatus   NVARCHAR(10) NOT NULL DEFAULT 'Unpaid' CHECK (PaymentStatus IN ('Unpaid','Paid')),
    EnrolmentDate   DATETIME     NOT NULL DEFAULT GETDATE(),
    UNIQUE (StudentID, ClassID)
);
GO

-- ============================================================
-- TABLE: CoachingRequests (student requests to lecturer)
-- ============================================================
CREATE TABLE CoachingRequests (
    RequestID   INT           IDENTITY(1,1) PRIMARY KEY,
    StudentID   INT           NOT NULL REFERENCES Students(StudentID),
    LecturerID  INT           NOT NULL REFERENCES Lecturers(LecturerID),
    ModuleID    INT           NOT NULL REFERENCES Modules(ModuleID),
    ClassLevel  NVARCHAR(20)  NOT NULL CHECK (ClassLevel IN ('Beginner','Intermediate','Advance')),
    Reason      NVARCHAR(500) NOT NULL,
    Status      NVARCHAR(20)  NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending','Approved','Rejected')),
    RequestDate DATETIME      NOT NULL DEFAULT GETDATE(),
    ReviewDate  DATETIME      NULL,
    ReviewNotes NVARCHAR(500) NULL
);
GO

-- ============================================================
-- TABLE: Payments
-- ============================================================
CREATE TABLE Payments (
    PaymentID     INT           IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID   INT           NOT NULL REFERENCES Enrolments(EnrolmentID),
    Amount        DECIMAL(10,2) NOT NULL,
    PaymentMethod NVARCHAR(50)  NOT NULL,
    PaymentDate   DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
-- TABLE: Feedbacks (Trainer sends to Administrator)
-- ============================================================
CREATE TABLE Feedbacks (
    FeedbackID   INT           IDENTITY(1,1) PRIMARY KEY,
    TrainerID    INT           NOT NULL REFERENCES Trainers(TrainerID),
    FeedbackType NVARCHAR(30)  NOT NULL CHECK (FeedbackType IN ('Suggestion','Complaint','Other')),
    Subject      NVARCHAR(200) NOT NULL,
    Message      NVARCHAR(MAX) NOT NULL,
    FeedbackDate DATETIME      NOT NULL DEFAULT GETDATE(),
    IsRead       BIT           NOT NULL DEFAULT 0
);
GO

-- ============================================================
-- SEED DATA: Users
-- Password: password123 (plain text for student assignment)
-- ============================================================

-- Administrator
INSERT INTO Users (Username, Password, Role, Name, Email, Phone, Address)
VALUES ('admin01', 'admin123', 'Administrator', 'Ahmad Zulkifli', 'admin@apu.edu.my', '0123456789', 'APU Tower, Bukit Jalil, KL');

-- Lecturers
INSERT INTO Users (Username, Password, Role, Name, Email, Phone, Address)
VALUES ('lect01', 'lect123', 'Lecturer', 'Dr. Sarah Tan', 'sarah.tan@apu.edu.my', '0112233445', 'Subang Jaya, Selangor');
INSERT INTO Users (Username, Password, Role, Name, Email, Phone, Address)
VALUES ('lect02', 'lect123', 'Lecturer', 'Prof. Rajan Kumar', 'rajan.kumar@apu.edu.my', '0198765432', 'Petaling Jaya, Selangor');

-- Trainers
INSERT INTO Users (Username, Password, Role, Name, Email, Phone, Address)
VALUES ('train01', 'train123', 'Trainer', 'Lee Chong Wei', 'lcw@trainer.com', '0133221100', 'Cheras, KL');
INSERT INTO Users (Username, Password, Role, Name, Email, Phone, Address)
VALUES ('train02', 'train123', 'Trainer', 'Priya Sharma', 'priya@trainer.com', '0144556677', 'Puchong, Selangor');
INSERT INTO Users (Username, Password, Role, Name, Email, Phone, Address)
VALUES ('train03', 'train123', 'Trainer', 'David Wong', 'david@trainer.com', '0155667788', 'Ampang, KL');

-- Students
INSERT INTO Users (Username, Password, Role, Name, Email, Phone, Address)
VALUES ('tp001', 'student123', 'Student', 'Muhammad Hafiz', 'hafiz@student.apu.edu.my', '0166778899', 'Bangsar, KL');
INSERT INTO Users (Username, Password, Role, Name, Email, Phone, Address)
VALUES ('tp002', 'student123', 'Student', 'Nur Aisyah Binti Ali', 'aisyah@student.apu.edu.my', '0177889900', 'Ampang, KL');
INSERT INTO Users (Username, Password, Role, Name, Email, Phone, Address)
VALUES ('tp003', 'student123', 'Student', 'Tanveer Singh', 'tanveer@student.apu.edu.my', '0188990011', 'Klang, Selangor');
INSERT INTO Users (Username, Password, Role, Name, Email, Phone, Address)
VALUES ('tp004', 'student123', 'Student', 'Li Wei Ming', 'liwei@student.apu.edu.my', '0199001122', 'Kepong, KL');
GO

-- ============================================================
-- SEED DATA: Administrators, Lecturers, Trainers, Students
-- ============================================================
INSERT INTO Administrators (UserID, StaffID) VALUES (1, 'ADM001');

INSERT INTO Lecturers (UserID, StaffID) VALUES (2, 'STF001');
INSERT INTO Lecturers (UserID, StaffID) VALUES (3, 'STF002');

INSERT INTO Trainers (UserID, StaffID, Specialisation) VALUES (4, 'TRN001', 'C# and .NET');
INSERT INTO Trainers (UserID, StaffID, Specialisation) VALUES (5, 'TRN002', 'Python & Data Science');
INSERT INTO Trainers (UserID, StaffID, Specialisation) VALUES (6, 'TRN003', 'Java & OOP');

INSERT INTO Students (UserID, TPNumber, StudyLevel) VALUES (7,  'TP065001', 'Level 1');
INSERT INTO Students (UserID, TPNumber, StudyLevel) VALUES (8,  'TP065002', 'Level 2');
INSERT INTO Students (UserID, TPNumber, StudyLevel) VALUES (9,  'TP065003', 'Level 1');
INSERT INTO Students (UserID, TPNumber, StudyLevel) VALUES (10, 'TP065004', 'Foundation');
GO

-- ============================================================
-- SEED DATA: Modules
-- ============================================================
INSERT INTO Modules (ModuleCode, ModuleName) VALUES ('CT044', 'Introduction to Object-Oriented Programming');
INSERT INTO Modules (ModuleCode, ModuleName) VALUES ('CT098', 'Data Structures and Algorithms');
INSERT INTO Modules (ModuleCode, ModuleName) VALUES ('CT115', 'Web Development Fundamentals');
INSERT INTO Modules (ModuleCode, ModuleName) VALUES ('CT074', 'Database Management Systems');
INSERT INTO Modules (ModuleCode, ModuleName) VALUES ('CT132', 'Software Engineering Principles');
GO

-- ============================================================
-- SEED DATA: TrainerModules
-- ============================================================
INSERT INTO TrainerModules (TrainerID, ModuleID, ClassLevel) VALUES (1, 1, 'Beginner');
INSERT INTO TrainerModules (TrainerID, ModuleID, ClassLevel) VALUES (2, 2, 'Intermediate');
INSERT INTO TrainerModules (TrainerID, ModuleID, ClassLevel) VALUES (3, 3, 'Advance');
GO

-- ============================================================
-- SEED DATA: Classes
-- ============================================================
INSERT INTO Classes (TrainerID, ModuleID, ClassLevel, Schedule, Venue, Fee, StartDate, EndDate)
VALUES (1, 1, 'Beginner', 'Monday & Wednesday, 2:00PM - 4:00PM', 'Lab A, APU Tower', 150.00, '2026-03-01', '2026-03-29');

INSERT INTO Classes (TrainerID, ModuleID, ClassLevel, Schedule, Venue, Fee, StartDate, EndDate)
VALUES (2, 2, 'Intermediate', 'Tuesday & Thursday, 10:00AM - 12:00PM', 'Lab B, APU Tower', 200.00, '2026-03-03', '2026-03-31');

INSERT INTO Classes (TrainerID, ModuleID, ClassLevel, Schedule, Venue, Fee, StartDate, EndDate)
VALUES (3, 3, 'Advance', 'Friday, 2:00PM - 6:00PM', 'Lab C, APU Tower', 250.00, '2026-03-06', '2026-03-27');

INSERT INTO Classes (TrainerID, ModuleID, ClassLevel, Schedule, Venue, Fee, StartDate, EndDate)
VALUES (1, 1, 'Intermediate', 'Saturday, 9:00AM - 1:00PM', 'Lab A, APU Tower', 200.00, '2026-04-05', '2026-04-26');
GO

-- ============================================================
-- SEED DATA: Enrolments
-- ============================================================
INSERT INTO Enrolments (StudentID, ClassID, MonthOfEnrolment, PaymentStatus)
VALUES (1, 1, 'March 2026', 'Paid');

INSERT INTO Enrolments (StudentID, ClassID, MonthOfEnrolment, PaymentStatus)
VALUES (1, 2, 'March 2026', 'Unpaid');

INSERT INTO Enrolments (StudentID, ClassID, MonthOfEnrolment, PaymentStatus)
VALUES (2, 2, 'March 2026', 'Paid');

INSERT INTO Enrolments (StudentID, ClassID, MonthOfEnrolment, PaymentStatus)
VALUES (3, 3, 'March 2026', 'Unpaid');

INSERT INTO Enrolments (StudentID, ClassID, MonthOfEnrolment, PaymentStatus)
VALUES (2, 4, 'April 2026', 'Unpaid');
GO

-- ============================================================
-- SEED DATA: Payments
-- ============================================================
INSERT INTO Payments (EnrolmentID, Amount, PaymentMethod)
VALUES (1, 150.00, 'Online Transfer');

INSERT INTO Payments (EnrolmentID, Amount, PaymentMethod)
VALUES (3, 200.00, 'Cash');
GO

-- ============================================================
-- SEED DATA: CoachingRequests
-- ============================================================
INSERT INTO CoachingRequests (StudentID, LecturerID, ModuleID, ClassLevel, Reason, Status)
VALUES (1, 1, 3, 'Beginner', 'I need help with web development as I am struggling with HTML and CSS concepts.', 'Pending');

INSERT INTO CoachingRequests (StudentID, LecturerID, ModuleID, ClassLevel, Reason, Status)
VALUES (2, 1, 4, 'Intermediate', 'My SQL skills need improvement for my upcoming project assessment.', 'Approved');

INSERT INTO CoachingRequests (StudentID, LecturerID, ModuleID, ClassLevel, Reason, Status)
VALUES (3, 2, 1, 'Beginner', 'First time learning OOP concepts and need extra guidance.', 'Rejected');
GO

-- ============================================================
-- SEED DATA: Feedbacks
-- ============================================================
INSERT INTO Feedbacks (TrainerID, FeedbackType, Subject, Message)
VALUES (1, 'Suggestion', 'Better Lab Equipment Needed',
    'The computers in Lab A are quite old and slow. It would be helpful if we could get newer machines to run Visual Studio smoothly.');

INSERT INTO Feedbacks (TrainerID, FeedbackType, Subject, Message)
VALUES (2, 'Complaint', 'Air Conditioning Issue in Lab B',
    'The air conditioning in Lab B has not been working properly for the past two weeks. Students and I are uncomfortable during sessions.');
GO

-- ============================================================
-- Verification Queries
-- ============================================================
SELECT 'Users'             AS [Table], COUNT(*) AS [Rows] FROM Users             UNION ALL
SELECT 'Students',           COUNT(*) FROM Students           UNION ALL
SELECT 'Lecturers',          COUNT(*) FROM Lecturers          UNION ALL
SELECT 'Trainers',           COUNT(*) FROM Trainers           UNION ALL
SELECT 'Modules',            COUNT(*) FROM Modules            UNION ALL
SELECT 'Classes',            COUNT(*) FROM Classes            UNION ALL
SELECT 'Enrolments',         COUNT(*) FROM Enrolments         UNION ALL
SELECT 'Payments',           COUNT(*) FROM Payments           UNION ALL
SELECT 'CoachingRequests',   COUNT(*) FROM CoachingRequests   UNION ALL
SELECT 'Feedbacks',          COUNT(*) FROM Feedbacks;
GO

PRINT 'APUCodeCampDB setup complete! All tables created and seeded.';
GO
