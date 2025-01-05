ALTER TABLE Notifications
ADD bookingId INT NULL, 
    customerName NVARCHAR(255) NULL, 
    email NVARCHAR(255) NULL, 
    phoneNumber NVARCHAR(50) NULL; 

ALTER TABLE Notifications
ADD CONSTRAINT FK_Notifications_Booking FOREIGN KEY (bookingId) REFERENCES Bookings(Id);
