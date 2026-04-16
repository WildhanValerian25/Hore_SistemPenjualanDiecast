CREATE DATABASE DiecastDB;
GO

USE DiecastDB;
GO

-- =========================================
-- 1. PEMBUATAN TABEL MASTER
-- =========================================

-- Tabel Roles (Tingkatan Hak Akses)
CREATE TABLE Roles (
    role_id INT PRIMARY KEY, 
    role_name VARCHAR(50) NOT NULL -- 'Admin' atau 'User'
);

-- =========================================
-- 2. PEMBUATAN TABEL TRANSAKSIONAL
-- =========================================

-- Tabel Users (Mengikuti struktur Accounts di gambar kamu)
CREATE TABLE Users (
    id INT PRIMARY KEY IDENTITY(1,1),
    role_id INT NOT NULL,
    username VARCHAR(50) UNIQUE NOT NULL, 
    password VARCHAR(50) NOT NULL,
    nama VARCHAR(100) NOT NULL,
    email VARCHAR(100),
    alamat TEXT,
    no_hp VARCHAR(20),
    FOREIGN KEY (role_id) REFERENCES Roles(role_id)
);

-- Tabel Produk (Katalog Diecast)
CREATE TABLE Produk (
    id_diecast INT PRIMARY KEY IDENTITY(1,1), -- ID unik untuk tiap diecast
    nama_diecast VARCHAR(200) NOT NULL,
    harga DECIMAL(18,2) NOT NULL,
    stok INT NOT NULL,
    jenis VARCHAR(100),
    is_deleted BIT DEFAULT 0 -- 0=Tersedia, 1=Dihapus
);

-- =========================================
-- 3. INSERT DATA AWAL (SEEDING)
-- =========================================

-- Memasukkan role dasar
INSERT INTO Roles (role_id, role_name) 
VALUES (1, 'Admin'), (2, 'User');

-- Membuat 1 akun Admin bawaan (Sesuai keinginan kamu)
INSERT INTO Users (role_id, username, password, nama) 
VALUES (1, 'admin', 'admin123', 'Wildhan');

-- Membuat 1 akun User untuk testing
INSERT INTO Users (role_id, username, password, nama) 
VALUES (2, 'Aryasuki', 'user123', 'Arya');

-- Memasukkan beberapa produk awal
INSERT INTO Produk (nama_diecast, harga, stok, jenis) 
VALUES 
('Hot Wheels Skyline', 55000, 10, 'JDM'),
('Tomica Civic Type R', 90000, 5, 'Sport'),
('Majorette Nissan GT-R', 75000, 7, 'JDM');

-- =========================================
-- 4. VERIFIKASI DATA
-- =========================================
SELECT u.username, u.nama, r.role_name 
FROM Users u 
JOIN Roles r ON u.role_id = r.role_id;

SELECT * FROM Produk WHERE is_deleted = 0;

-- Tambahkan kolom Role ke tabel Users agar sesuai dengan kode C#
ALTER TABLE Users ADD Role VARCHAR(20);
UPDATE Users SET Role = 'admin' WHERE username = 'admin';
UPDATE Users SET Role = 'user' WHERE username = 'Aryasuki';
-- Tambahkan kolom NoHP ke tabel Users
ALTER TABLE Users ADD NoHP VARCHAR(15);

CREATE TABLE Pesanan (
    pesanan_id INT PRIMARY KEY IDENTITY(1,1),
    username VARCHAR(50),      -- Siapa yang pesan
    movie_id INT,              -- Produk apa (sesuai kolom primary key tabel Produk kamu)
    jumlah INT,
    total_harga DECIMAL(18,2),
    status_pesanan VARCHAR(50) DEFAULT 'Pending', -- Pending, Dikirim, Selesai
    tanggal_pesan DATETIME DEFAULT GETDATE(),
    no_resi VARCHAR(100) DEFAULT '-'
);

-- Perbaikan kolom NoHP yang error di gambar sebelumnya
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'NoHP' AND Object_ID = OBJECT_ID('Users'))
BEGIN
    ALTER TABLE Users ADD NoHP VARCHAR(15);
END

-- Memperbarui data produk berdasarkan ID uniknya
UPDATE Produk 
SET 
    nama_diecast = @nama, 
    harga = @harga, 
    stok = @stok, 
    jenis = @jenis
WHERE 
    id_diecast = @id_diecast;

-- 1. DEKLARASI VARIABEL
DECLARE @nama_diecast VARCHAR(200) = 'Hot Wheels Skyline R34';
DECLARE @harga DECIMAL(18,2) = 65000.00;
DECLARE @stok INT = 15;
DECLARE @jenis VARCHAR(100) = 'JDM';
DECLARE @id_diecast INT = 1; -- Ganti dengan ID produk yang ingin diubah

-- 2. EKSEKUSI QUERY UPDATE
UPDATE Produk
SET 
    nama_diecast = @nama_diecast,
    harga = @harga,
    stok = @stok,
    jenis = @jenis
WHERE 
    id_diecast = @id_diecast;

-- 3. VERIFIKASI HASIL
SELECT * FROM Produk WHERE id_diecast = @id_diecast;

SELECT Nama_Diecast, Harga, Stok, Jenis 
FROM Produk 
WHERE is_deleted = 0;`