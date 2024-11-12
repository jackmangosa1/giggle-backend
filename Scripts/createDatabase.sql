-- ASP.NET Identity Tables --

-- Users Table
CREATE TABLE AspNetUsers (
    Id NVARCHAR(450) NOT NULL PRIMARY KEY,
    UserName NVARCHAR(256) NULL,
    NormalizedUserName NVARCHAR(256) NULL,
    Email NVARCHAR(256) NULL,
    NormalizedEmail NVARCHAR(256) NULL,
    EmailConfirmed BIT NOT NULL,
    PasswordHash NVARCHAR(MAX) NULL,
    SecurityStamp NVARCHAR(MAX) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    PhoneNumber NVARCHAR(20) NULL,
    PhoneNumberConfirmed BIT NOT NULL,
    TwoFactorEnabled BIT NOT NULL,
    LockoutEnd DATETIMEOFFSET NULL,
    LockoutEnabled BIT NOT NULL,
    AccessFailedCount INT NOT NULL
);

-- Roles Table
CREATE TABLE AspNetRoles (
    Id NVARCHAR(450) NOT NULL PRIMARY KEY,
    Name NVARCHAR(256) NULL,
    NormalizedName NVARCHAR(256) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL
);

-- User Roles Table
CREATE TABLE AspNetUserRoles (
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);

-- Claims Table
CREATE TABLE AspNetUserClaims (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- Role Claims Table
CREATE TABLE AspNetRoleClaims (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    RoleId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);

-- Logins Table
CREATE TABLE AspNetUserLogins (
    LoginProvider NVARCHAR(450) NOT NULL,
    ProviderKey NVARCHAR(450) NOT NULL,
    ProviderDisplayName NVARCHAR(256) NULL,
    UserId NVARCHAR(450) NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- Tokens Table
CREATE TABLE AspNetUserTokens (
    UserId NVARCHAR(450) NOT NULL,
    LoginProvider NVARCHAR(450) NOT NULL,
    Name NVARCHAR(450) NOT NULL,
    Value NVARCHAR(MAX) NULL,
    PRIMARY KEY (UserId, LoginProvider, Name),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- Create Customers table
CREATE TABLE Customers (
    id INT PRIMARY KEY IDENTITY(1,1),
    userId NVARCHAR(450) NOT NULL,  
    FullName NVARCHAR(200), 
    ProfilePictureUrl NVARCHAR(255),
    address NVARCHAR(255),
    phoneNumber NVARCHAR(20),
    preferredPaymentMethod INT,
    FOREIGN KEY (userId) REFERENCES AspNetUsers(Id)
);

-- Create ServiceProviders table
CREATE TABLE Providers (
    id INT PRIMARY KEY IDENTITY(1,1),
    userId NVARCHAR(450) NOT NULL,  
    DisplayName NVARCHAR(200), 
    phoneNumber NVARCHAR(20),
    ProfilePictureUrl NVARCHAR(255),
    bio NVARCHAR(MAX),
    FOREIGN KEY (userId) REFERENCES AspNetUsers(Id)
);

-- Create the Skills table
CREATE TABLE Skills (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE
);

-- Create the junction table for the many-to-many relationship
CREATE TABLE ProviderSkills (
    ProviderId INT,
    SkillId INT,
    FOREIGN KEY (ProviderId) REFERENCES Providers(Id) ON DELETE CASCADE,
    FOREIGN KEY (SkillId) REFERENCES Skills(Id) ON DELETE CASCADE,
    PRIMARY KEY (ProviderId, SkillId)
);

-- Create ServiceCategory table
CREATE TABLE ServiceCategory (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL
);

-- Create Services table
CREATE TABLE Services (
    id INT PRIMARY KEY IDENTITY(1,1),
    providerId INT NOT NULL,       
    categoryId INT NOT NULL,       
    name NVARCHAR(100) NOT NULL,
    description NVARCHAR(MAX),
    price DECIMAL(10, 2) NULL,      
    priceType INT NOT NULL,        
    mediaURL NVARCHAR(255),
    FOREIGN KEY (providerId) REFERENCES Providers(id),
    FOREIGN KEY (categoryId) REFERENCES ServiceCategory(id)
);


-- Create Bookings table
CREATE TABLE Bookings (
    id INT PRIMARY KEY IDENTITY(1,1),
    serviceId INT NOT NULL,        
    customerId INT NOT NULL,       
    totalPrice DECIMAL(10, 2) NOT NULL,
    status INT NOT NULL,
    paymentStatus INT NOT NULL,
    createdAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    scheduledAt DATETIME2 NOT NULL,
    completedAt DATETIME2,
    FOREIGN KEY (serviceId) REFERENCES Services(id),
    FOREIGN KEY (customerId) REFERENCES Customers(id)
);

-- Create CompletedServices table
CREATE TABLE CompletedServices (
    id INT PRIMARY KEY IDENTITY(1,1),
    bookingId INT NOT NULL,       
    description NVARCHAR(MAX),
    mediaURL NVARCHAR(255),
    completedAt DATETIME2 NOT NULL,
    FOREIGN KEY (bookingId) REFERENCES Bookings(id)
);

-- Create Reviews table
CREATE TABLE Reviews (
    id INT PRIMARY KEY IDENTITY(1,1),
    userId NVARCHAR(450) NOT NULL,
    completedServiceId INT NOT NULL,
    rating INT NOT NULL CHECK (rating BETWEEN 1 AND 5),
    comment NVARCHAR(MAX),
    createdAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (userId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (completedServiceId) REFERENCES CompletedServices(id)
);

-- Create Messages table
CREATE TABLE Messages (
    id INT PRIMARY KEY IDENTITY(1,1),
    senderId NVARCHAR(450) NOT NULL, 
    receiverId NVARCHAR(450) NOT NULL, 
    IsRead BIT NOT NULL DEFAULT 0,
    content NVARCHAR(MAX) NOT NULL,
    sentAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (senderId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (receiverId) REFERENCES AspNetUsers(Id)
);

-- Create Payments table
CREATE TABLE Payments (
    id INT PRIMARY KEY IDENTITY(1,1),
    bookingId INT NOT NULL,          
    amount DECIMAL(10, 2) NOT NULL,
    method INT NOT NULL,
    TransactionId NVARCHAR(100) NOT NULL,
    TxRef NVARCHAR(100) NOT NULL,          
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),  
    CompletedAt DATETIME NULL,             
    FailureReason NVARCHAR(255) NULL,
    status INT NOT NULL,
    FOREIGN KEY (bookingId) REFERENCES Bookings(id)
);
-- Create Notifications table
CREATE TABLE Notifications (
    id INT PRIMARY KEY IDENTITY(1,1),
    userId NVARCHAR(450) NOT NULL,  
    type INT NOT NULL,            
    isRead BIT NOT NULL DEFAULT 0,
    createdAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (userId) REFERENCES AspNetUsers(Id) 
);
