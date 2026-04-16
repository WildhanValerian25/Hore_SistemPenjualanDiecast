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
        string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=DiecastDB;Integrated Security=True";

        public Form1()
        {
            InitializeComponent();

            // Menghubungkan LinkLabel ke method-nya
            linkRegister.LinkClicked += LinkRegister_LinkClicked;
        }

        private void InitializeComponent()
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

            // linkRegister
            this.linkRegister.AutoSize = true;
            this.linkRegister.Location = new System.Drawing.Point(330, 260);
            this.linkRegister.Name = "linkRegister";
            this.linkRegister.Text = "Belum memiliki akun?";

            // Form1 Settings
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(888, 323);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.linkRegister);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Menggunakan 'this.connStr' dari baris 15
            using (SqlConnection conn = new SqlConnection(this.connStr))
            {
                try
                {
                    conn.Open();

                    // Query yang disesuaikan agar tidak error "Invalid column name Role"
                    // Pastikan di SQL Server kamu sudah menjalankan: ALTER TABLE Users ADD Role VARCHAR(20);
                    string query = "SELECT username, Role FROM Users WHERE username = @u AND password = @p";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@u", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@p", txtPassword.Text);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string dbUsername = reader["username"].ToString();
                        string role = reader["Role"].ToString().ToLower().Trim();
                        reader.Close();

                        if (role == "admin")
                        {
                            FormAdminDashboard adminDashboard = new FormAdminDashboard(role);
                            adminDashboard.Show();
                        }
                        else
                        {
                            FormUser userDashboard = new FormUser(dbUsername);
                            userDashboard.Show();
                        }
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Username atau Password salah!", "Akses Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    // Menangkap error jika kolom 'Role' masih belum ada di database
                    MessageBox.Show("Gagal terhubung ke database!\n" + ex.Message, "Error Koneksi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LinkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FormRegister r = new FormRegister();
            r.Show();
            this.Hide();
        }
    }
}