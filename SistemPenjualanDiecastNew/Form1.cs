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
            this.SuspendLayout();

            // lblTitle  
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(330, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(159, 37);
            this.lblTitle.Text = "Login Page";

            // lblUsername
            this.lblUsername.Location = new System.Drawing.Point(200, 120);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Text = "Username";

            // lblPassword
            this.lblPassword.Location = new System.Drawing.Point(200, 160);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Text = "Password";

            // txtUsername
            this.txtUsername.Location = new System.Drawing.Point(320, 120);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(200, 22);

            // txtPassword
            this.txtPassword.Location = new System.Drawing.Point(320, 160);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(200, 22);
            this.txtPassword.UseSystemPasswordChar = true;

            // btnLogin
            this.btnLogin.BackColor = System.Drawing.Color.Gold;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Location = new System.Drawing.Point(340, 210);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 35);
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = false;

            // Memastikan nama Event Handler sesuai (untuk mencegah Error CS0103)
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

            

