-- Script para crear base de datos y tablas (MySQL) - CORREGIDO para SupplierId NULLABLE
CREATE DATABASE IF NOT EXISTS inventorydb;
USE inventorydb;

CREATE TABLE IF NOT EXISTS Suppliers (
  SupplierId INT AUTO_INCREMENT PRIMARY KEY,
  Name VARCHAR(200) NOT NULL,
  Contact VARCHAR(200)
);

CREATE TABLE IF NOT EXISTS Products (
  ProductId INT AUTO_INCREMENT PRIMARY KEY,
  Name VARCHAR(200) NOT NULL,
  Description TEXT,
  Stock INT NOT NULL DEFAULT 0,
  Price DECIMAL(10,2) NOT NULL DEFAULT 0,
  SupplierId INT NULL,
  FOREIGN KEY (SupplierId) REFERENCES Suppliers(SupplierId) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS Entries (
  EntryId INT AUTO_INCREMENT PRIMARY KEY,
  ProductId INT,
  Quantity INT NOT NULL,
  EntryDate DATETIME NOT NULL,
  Notes TEXT,
  FOREIGN KEY (ProductId) REFERENCES Products(ProductId) ON DELETE CASCADE
);

-- Datos de ejemplo
INSERT INTO Suppliers (Name, Contact) VALUES ('Proveedor A', 'contacto@a.com'), ('Proveedor B', 'contacto@b.com');

INSERT INTO Products (Name, Description, Stock, Price, SupplierId) VALUES 
('Producto 1', 'Descripción 1', 3, 10.50, 1),
('Producto 2', 'Descripción 2', 12, 5.00, 2),
('Producto 3', 'Descripción 3', 1, 20.00, 1);

INSERT INTO Entries (ProductId, Quantity, EntryDate, Notes) VALUES
(1, 3, NOW(), 'Ingreso inicial'),
(2, 12, NOW(), 'Ingreso inicial'),
(3, 1, NOW(), 'Ingreso inicial');
