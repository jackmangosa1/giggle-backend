-- Step 1: Drop the existing foreign key constraint using the correct name
ALTER TABLE Payments
DROP CONSTRAINT FK__Payments__custom__245D67DE

-- Step 2: Remove the 'currency' column
ALTER TABLE Payments
DROP COLUMN currency;

-- Step 3: Alter the 'customerId' column to be VARCHAR(255) and change the foreign key reference to 'userId' in the 'Customers' table
ALTER TABLE Payments
ALTER COLUMN customerId VARCHAR(450) NOT NULL;

-- Step 4: Add a new foreign key constraint referencing 'userId' in the 'Customers' table
ALTER TABLE Payments
ADD CONSTRAINT FK_Payments_Customers
FOREIGN KEY (customerId) REFERENCES Customers(userId);

-- Step 5: Change the 'paymentMethod' column type to INT
ALTER TABLE Payments
ALTER COLUMN paymentMethod INT;

SELECT CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE TABLE_NAME = 'Payments' AND COLUMN_NAME = 'customerId';
