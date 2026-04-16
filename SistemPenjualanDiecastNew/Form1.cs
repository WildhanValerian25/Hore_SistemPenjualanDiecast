using System;
using System.Drawing;   // Menambah librarty dasar dan namespace sesuai judul
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SistemPenjualanDiecastNew 
{
    public partial class Form1 : Form
    {
        // Deklarasi Komponen UI
        Label lblTitle, lblUsername, lblPassword;
        TextBox txtUsername, txtPassword;
        Button btnLogin;
        LinkLabel linkRegister;

        // Baris 15: Variabel Koneksi Global (Agar tidak ada warning CS0414)
        string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=DiecastDB;Integrated Security=True"; // Menambahkan koneksi string database

       