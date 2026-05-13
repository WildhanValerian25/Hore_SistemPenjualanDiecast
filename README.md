# 🚗 Sistem Informasi Penjualan Diecast
### Proyek UCP — Wildhan Valerian & Tejo Cahyo
### Program Studi Teknologi Informasi — Universitas Muhammadiyah Yogyakarta

---

## 📋 Deskripsi Aplikasi

**Sistem Informasi Penjualan Diecast** adalah aplikasi desktop berbasis **Windows Forms (C# ADO.NET)** yang dirancang untuk mengelola penjualan produk diecast (miniatur kendaraan). Aplikasi ini menggunakan arsitektur **Two-Tier** dengan koneksi langsung ke database **SQL Server**.

Aplikasi memiliki dua peran pengguna:
- **Admin** — mengelola produk, pesanan, dan pembayaran
- **Pelanggan** — melihat produk, melakukan pembelian, dan memantau pesanan

---

## 🛠️ Teknologi yang Digunakan

| Teknologi | Versi |
|---|---|
| Bahasa Pemrograman | C# (.NET Framework) |
| UI Framework | Windows Forms |
| Database | SQL Server (SSMS) |
| Database Access | ADO.NET |
| IDE | Visual Studio |

---

## 🗄️ Struktur Database

**Database:** `db_penjualan_diecast`

### Tabel Utama

| Tabel | Fungsi |
|---|---|
| `ADMIN` | Data admin beserta role dan level akses |
| `PELANGGAN` | Data pelanggan yang terdaftar |
| `PRODUK` | Katalog produk diecast |
| `PESANAN` | Header transaksi pembelian |
| `DETAIL_PESANAN` | Detail produk dalam setiap pesanan |
| `PEMBAYARAN` | Data pembayaran setiap pesanan |
| `KERANJANG` | Keranjang belanja pelanggan |
| `ITEM_KERANJANG` | Isi keranjang belanja |
| `ULASAN` | Ulasan produk dari pelanggan |
| `RIWAYAT_PASSWORD` | Riwayat perubahan password |
| `RESET_PASSWORD_TOKEN` | Token reset password |

### VIEW yang Digunakan

| VIEW | Fungsi |
|---|---|
| `vw_ProdukAktif` | Menampilkan produk aktif beserta status stok |
| `vw_PesananLengkap` | Menampilkan pesanan lengkap dengan detail produk dan pembeli |
| `vw_PembayaranMenunggu` | Menampilkan pembayaran yang menunggu konfirmasi admin |

### Stored Procedure

| Stored Procedure | Fungsi |
|---|---|
| `sp_LoginPelanggan` | Autentikasi login pelanggan |
| `sp_LoginAdmin` | Autentikasi login admin |
| `sp_InsertPelanggan` | Registrasi pelanggan baru |
| `sp_UpdateProfilPelanggan` | Update profil pelanggan |
| `sp_InsertProduk` | Tambah produk baru |
| `sp_UpdateProduk` | Update data produk |
| `sp_DeleteProduk` | Hapus produk (soft delete) |
| `sp_SearchProduk` | Pencarian produk berdasarkan keyword |
| `sp_InsertPesanan` | Buat pesanan baru |
| `sp_InsertDetailPesanan` | Insert detail pesanan + kurangi stok |
| `sp_UpdateStatusPesanan` | Update status pesanan + kembalikan stok jika dibatalkan |
| `sp_KonfirmasiPembayaran` | Konfirmasi atau tolak pembayaran |
| `sp_InsertPembayaran` | Insert data pembayaran |
| `sp_KirimBuktiPembayaran` | Upload bukti pembayaran |
| `sp_TambahStok` | Tambah stok produk |
| `sp_CountProduk` | Hitung total produk aktif (OUTPUT parameter) |

---

## 🖥️ Fitur Aplikasi

### 👤 Fitur Pelanggan
- ✅ Register akun baru (dengan validasi email, username, password minimal 8 karakter)
- ✅ Login menggunakan username dan password
- ✅ Lihat katalog produk diecast yang tersedia
- ✅ Beli produk dengan konfirmasi jumlah & alamat pengiriman
- ✅ Pilih metode pembayaran (Transfer Bank, QRIS, COD, Dompet Digital)
- ✅ Upload bukti pembayaran
- ✅ Lihat riwayat pesanan
- ✅ Konfirmasi pesanan diterima
- ✅ Lihat nomor resi pengiriman
- ✅ Edit profil (email, alamat, no HP)

### 🔧 Fitur Admin
- ✅ Login dengan level akses (Super Admin, Editor Stok, Editor)
- ✅ Dashboard ringkasan informasi (total produk, pesanan masuk, pembayaran menunggu, dll.)
- ✅ CRUD produk (Add, Update, Delete, Search) menggunakan Stored Procedure
- ✅ Manajemen pesanan masuk dengan filter status
- ✅ Konfirmasi & isi nomor resi pengiriman
- ✅ Tolak pembayaran (stok otomatis dikembalikan)
- ✅ Manajemen pembayaran (konfirmasi lunas / tolak)
- ✅ Lihat data pelanggan terdaftar
- ✅ Test koneksi database
- ✅ Reset data produk ke kondisi awal (dari backup)
- ✅ Simulasi SQL Injection (demo keamanan)

---

## 🔐 Skenario SQL Injection

### Form yang Rentan
**Form Login (Form1.cs)** sebelum diperbaiki

### Contoh Query Rentan
```csharp
// ❌ QUERY RENTAN SQL INJECTION (sebelum perbaikan)
string query = "SELECT * FROM PELANGGAN WHERE username='"
               + txtUsername.Text + "' AND password='"
               + txtPassword.Text + "'";
```

### Skenario Serangan 1 — Bypass Login
Masukkan di field **Username:**
```
' OR '1'='1
```
Masukkan di field **Password:**
```
' OR '1'='1
```

**Query yang terbentuk:**
```sql
SELECT * FROM PELANGGAN
WHERE username='' OR '1'='1'
AND password='' OR '1'='1'
```
Karena `'1'='1'` selalu **TRUE**, attacker bisa login tanpa username/password yang valid.

### Skenario Serangan 2 — DROP TABLE
Masukkan di field Username:
```
'; DROP TABLE PELANGGAN; --
```
**Query yang terbentuk:**
```sql
SELECT * FROM PELANGGAN WHERE username='';
DROP TABLE PELANGGAN; --' AND password=''
```
Ini akan **menghapus seluruh tabel PELANGGAN** dari database.

### Skenario Serangan 3 — UPDATE Massal (Demo di Aplikasi)
Input berbahaya: `' OR 1=1 --`

**Query tidak aman:**
```sql
UPDATE PRODUK SET nama_produk='HACKED'
WHERE nama_produk='' OR 1=1 --'
```
Karena `1=1` selalu TRUE, **semua produk** akan diubah namanya menjadi `HACKED`.

### ✅ Pencegahan yang Diterapkan
Menggunakan **Parameterized Query** dan **Stored Procedure:**
```csharp
// ✅ AMAN — menggunakan Stored Procedure
SqlCommand cmd = new SqlCommand("sp_LoginPelanggan", conn);
cmd.CommandType = CommandType.StoredProcedure;
cmd.Parameters.AddWithValue("@username", txtUsername.Text);
cmd.Parameters.AddWithValue("@password", txtPassword.Text);
```
Dengan parameterized query, input user **tidak akan dieksekusi** sebagai perintah SQL sehingga SQL Injection tidak bisa terjadi.

---

## 📁 Struktur File Project

```
SistemPenjualanDiecastNew/
├── Form1.cs                    # Form Login
├── FormRegister.cs             # Form Registrasi Pelanggan
├── FormUser.cs                 # Dashboard Pelanggan
├── FormBeli.cs                 # Form Konfirmasi Pembelian
├── FormPembayaran.cs           # Form Upload Bukti Pembayaran
├── FormRiwayatPesanan.cs       # Riwayat Pesanan Pelanggan
├── FormAdminDashboard.cs       # Dashboard Admin
├── FormAdminPesanan.cs         # Manajemen Pesanan (Admin)
├── FormAdminPembayaran.cs      # Manajemen Pembayaran (Admin)
├── FormAdminStok.cs            # Manajemen Stok Produk (Admin)
├── FormKonfirmasiResi.cs       # Form Input Nomor Resi (Admin)
├── UserSession.cs              # Session management
└── Program.cs                  # Entry point aplikasi
```

---

## ⚙️ Cara Instalasi & Menjalankan

### Prasyarat
- Visual Studio 2019 / 2022
- SQL Server (LocalDB / Express / Full)
- SQL Server Management Studio (SSMS)
- .NET Framework 4.7.2 atau lebih baru

### Langkah Instalasi

**1. Clone Repository**
```bash
git clone https://github.com/[username]/SistemPenjualanDiecast.git
cd SistemPenjualanDiecast
```

**2. Setup Database**

Buka SSMS, jalankan script SQL berikut secara berurutan:
```sql
-- Jalankan file: database/db_penjualan_diecast.sql
-- File ini akan membuat database, tabel, view, stored procedure, dan data awal
```

**3. Sesuaikan Connection String**

Buka setiap file `.cs` dan sesuaikan connection string dengan server kamu:
```csharp
string connStr = @"Data Source=NAMA_SERVER\INSTANCE;
                   Initial Catalog=db_penjualan_diecast;
                   Integrated Security=True";
```

**4. Jalankan Aplikasi**
```
Buka SistemPenjualanDiecastNew.sln di Visual Studio
Tekan F5 atau klik Start
```

---

## 👤 Akun Default

### Admin
| Username | Password | Level |
|---|---|---|
| `wildhan` | `admin123` | Super Admin |
| `editor` | `editor123` | Editor Stok |

### Pelanggan (Contoh)
| Username | Password |
|---|---|
| `budi` | `hash_pw_budi` |
| `sari` | `hash_pw_sari` |

> ⚠️ Ganti password default setelah instalasi pertama.

---

## 🔄 Alur Penggunaan

### Alur Pelanggan
```
Register → Login → Lihat Produk → Pilih Produk
→ Isi Form Beli (jumlah, alamat, metode bayar)
→ Upload Bukti Pembayaran
→ Tunggu Konfirmasi Admin
→ Pesanan Dikirim → Konfirmasi Diterima → Selesai
```

### Alur Admin
```
Login → Dashboard → Terima Pesanan Masuk
→ Cek Bukti Pembayaran → Konfirmasi Lunas
→ Input Nomor Resi → Status: Dikirim
→ Pelanggan Konfirmasi Diterima → Status: Selesai
```

---

## 📊 Binding & DataGridView

Aplikasi ini mengimplementasikan **Data Binding** sesuai modul praktikum:

```csharp
// Contoh implementasi BindingSource + BindingNavigator
private BindingSource _bindingSource = new BindingSource();

// Set data ke BindingSource
_bindingSource.DataSource = dt;

// Hubungkan ke DataGridView
dgvProducts.DataSource = _bindingSource;

// Hubungkan ke BindingNavigator
navigator.BindingSource = _bindingSource;
```

---

## 👨‍💻 Developer

| Nama | NIM | Peran |
|---|---|---|
| Wildhan Valerian | - | Backend, Database, UI |
| Tejo Cahyo | - | Backend, Database, UI |

**Dosen Pengampu:** Apriliya Kurnianti, S.T., M.Eng
**Mata Kuliah:** Pengembangan Aplikasi Basis Data (PABD)
**Universitas:** Universitas Muhammadiyah Yogyakarta

---

## 📝 Lisensi

Proyek ini dibuat untuk keperluan **UCP (Ujian Capaian Pembelajaran)** mata kuliah Pengembangan Aplikasi Basis Data.

© 2026 Wildhan Valerian & Tejo Cahyo — Universitas Muhammadiyah Yogyakarta
