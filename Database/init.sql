-- =============================================
-- Notes Application - Database Setup Script
-- SQL Server
-- =============================================

-- Step 1: Create the database
CREATE DATABASE NotesDb;
GO

USE NotesDb;
GO

-- =============================================
-- Step 2: Create Users table
-- =============================================
CREATE TABLE Users (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    Username     NVARCHAR(100)  NOT NULL,
    Email        NVARCHAR(255)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500)  NOT NULL,
    CreatedAt    DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);

CREATE INDEX IX_Users_Email ON Users(Email);
GO

-- =============================================
-- Step 3: Create Notes table
-- =============================================
CREATE TABLE Notes (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    UserId    INT            NOT NULL,
    Title     NVARCHAR(500)  NOT NULL,
    Content   NVARCHAR(MAX)  NULL,
    CreatedAt DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2      NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Notes_Users
        FOREIGN KEY (UserId) REFERENCES Users(Id)
        ON DELETE CASCADE
);

CREATE INDEX IX_Notes_UserId   ON Notes(UserId);
CREATE INDEX IX_Notes_CreatedAt ON Notes(CreatedAt DESC);
GO

PRINT 'Database and tables created successfully.';
GO
