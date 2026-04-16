using Microsoft.Win32;
using System;
using System.Data.SqlClient;
using System.Drawing;   // Menambah librarty dasar dan namespace sesuai judul
using System.Runtime.InteropServices;
using System.Windows.Forms;

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

        public Form1() 
        {
            InitializeComponent();

        // menghubungkan LinkLabel register dengan event handler

        linkRegister.LinkClicked += LinkRegister_LinkClicked;
        }
        private void InitializeComponent()  //  implement basic login UI layout
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.linkRegister = new System.Windows.Forms.LinkLabel();
            