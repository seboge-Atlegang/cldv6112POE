-- DATABASE CREATION SCRIPT
USE master
-- Check if database exists and drop it if it does
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'PoeDB')
    DROP DATABASE PoeDB;
CREATE DATABASE PoeDB
    
-- Use the newly created database
USE PoeDB

-- Create Venue table
CREATE TABLE Venue (
    VenueId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    Capacity INT NOT NULL,
    ImageUrl NVARCHAR(255),
    CONSTRAINT CHK_CapacityPositive CHECK (Capacity > 0)
);

-- Create Event table
CREATE TABLE Event (
    EventId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    CONSTRAINT CHK_ValidEventDates CHECK (EndDate > StartDate)
);

-- Create Booking table
CREATE TABLE Booking (
    BookingId INT PRIMARY KEY IDENTITY(1,1),
    VenueId INT NOT NULL,
    EventId INT NOT NULL,
    BookingDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Booking_Venue FOREIGN KEY (VenueId) REFERENCES Venue(VenueId),
    CONSTRAINT FK_Booking_Event FOREIGN KEY (EventId) REFERENCES Event(EventId),
    CONSTRAINT UQ_VenueEvent UNIQUE (VenueId, EventId)
);
-- Create index for performance on date queries
CREATE INDEX IX_Booking_VenueDate ON Booking(VenueId, BookingDate);
CREATE INDEX IX_Event_Dates ON Event(StartDate, EndDate);


-- Insert sample venues
INSERT INTO Venue (Name, Location, Capacity, ImageUrl)
VALUES 
('Grand Ballroom', '123 Main St, New York', 500, '/images/ballroom.jpg'),
('Riverside Terrace', '456 River Rd, Chicago', 200, '/images/terrace.jpg'),
('Skyline Conference Center', '789 High St, Los Angeles', 1000, '/images/conference.jpg');

-- Insert sample events
INSERT INTO Event (Name, Description, StartDate, EndDate)
VALUES 
('Tech Conference 2023', 'Annual technology conference', '2023-11-15 09:00:00', '2023-11-17 18:00:00'),
('Wedding Reception', 'Smith-Jones wedding', '2023-12-10 17:00:00', '2023-12-11 01:00:00'),
('Product Launch', 'New product unveiling', '2023-10-05 10:00:00', '2023-10-05 14:00:00');

-- Insert sample bookings
INSERT INTO Booking (VenueId, EventId, BookingDate)
VALUES 
(1, 1, '2023-01-15 10:00:00'),
(2, 2, '2023-02-20 11:30:00'),
(3, 3, '2023-03-10 09:15:00');

SELECT * FROM Venue;
SELECT * FROM Event;
SELECT * FROM Booking;





