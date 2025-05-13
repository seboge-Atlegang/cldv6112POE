-- DATABASE CREATION SCRIPT
USE master
-- Check if database exists and drop it if it does
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'PoeDB')
    DROP DATABASE PoeDB;
CREATE DATABASE PoeDB;
    
-- Use the newly created database
USE PoeDB;

-- Create Venue table
CREATE TABLE Venue (
    VenueId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    Capacity INT NOT NULL,
    ImageUrl NVARCHAR(255),
    CONSTRAINT CHK_CapacityPositive CHECK (Capacity > 0)
);

-- Create Event table with corrected structure
CREATE TABLE Event (
    EventId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    EventDate DATETIME NOT NULL,
    Description NVARCHAR(MAX),
    VenueId INT NOT NULL,  -- Add VenueId as a foreign key
    CONSTRAINT FK_Event_Venue FOREIGN KEY (VenueId) REFERENCES Venue(VenueId),
    CONSTRAINT CHK_ValidEventDates CHECK (EventDate > GETDATE()) -- EventDate should be in the future
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
CREATE INDEX IX_Event_Dates ON Event(EventDate);

-- Insert sample venues
INSERT INTO Venue (Name, Location, Capacity, ImageUrl)
VALUES 
('Grand Ballroom', '123 Main St, New York', 500, '/images/ballroom.jpg'),
('Riverside Terrace', '456 River Rd, Chicago', 200, '/images/terrace.jpg'),
('Skyline Conference Center', '789 High St, Los Angeles', 1000, '/images/conference.jpg');


-- Insert sample events with corrected field names
-- Make sure to insert after venues are created
INSERT INTO Event (Name, EventDate, Description, VenueId)
VALUES 
('Tech Conference 2023', '2026-11-15 09:00:00', 'Annual technology conference', 1),
('Wedding Reception', '2026-12-10 17:00:00', 'Smith-Jones wedding', 2),
('Product Launch', '2026-10-05 10:00:00', 'New product unveiling', 3);

-- Insert sample bookings with correct EventId values
-- Make sure that the EventId you are referencing in Booking corresponds to the inserted Event
INSERT INTO Booking (VenueId, EventId, BookingDate)
VALUES 
(1, 1, '2023-01-15 10:00:00'),  -- EventId 1
(2, 2, '2023-02-20 11:30:00'),  -- EventId 2
(3, 3, '2023-03-10 09:15:00');  -- EventId 3

-- Select data to verify insertions
SELECT * FROM Venue;
SELECT * FROM Event;
SELECT * FROM Booking
