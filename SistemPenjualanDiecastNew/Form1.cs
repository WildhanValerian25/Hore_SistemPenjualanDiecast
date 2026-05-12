using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public partial class Form1 : Form
    {
        Label lblTitle, lblUsername, lblPassword;
        TextBox txtUsername, txtPassword;
        Button btnLogin;
        LinkLabel linkRegister;

        public Form1()
        {
            InitializeComponent();
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

            // TITLE
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(330, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Login Page";

            // LABEL USERNAME
            this.lblUsername.Location = new System.Drawing.Point(200, 120);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(100, 23);
            this.lblUsername.TabIndex = 1;
            this.lblUsername.Text = "Username";

            // LABEL PASSWORD
            this.lblPassword.Location = new System.Drawing.Point(200, 160);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(100, 23);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Password";

            // TEXTBOX USERNAME
            this.txtUsername.Location = new System.Drawing.Point(320, 120);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(200, 22);
            this.txtUsername.TabIndex = 2;

            // TEXTBOX PASSWORD
            this.txtPassword.Location = new System.Drawing.Point(320, 160);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(200, 22);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.UseSystemPasswordChar = true;

            // BUTTON LOGIN
            this.btnLogin.BackColor = System.Drawing.Color.Gold;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Location = new System.Drawing.Point(374, 209);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 35);
            this.btnLogin.TabIndex = 5;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

            // LINK REGISTER
            this.linkRegister.AutoSize = true;
            this.linkRegister.Location = new System.Drawing.Point(330, 260);
            this.linkRegister.Name = "linkRegister";
            this.linkRegister.TabIndex = 6;
            this.linkRegister.TabStop = true;
            this.linkRegister.Text = "Belum memiliki akun?";

            // FORM
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
            if (txtUsername.Text == "" || txtPassword.Text == "")
            {
                MessageBox.Show("Username dan Password wajib diisi!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // ✅ Login PELANGGAN pakai Stored Procedure
                    SqlCommand cmdPelanggan = new SqlCommand("sp_LoginPelanggan", conn);
                    cmdPelanggan.CommandType = CommandType.StoredProcedure;
                    cmdPelanggan.Parameters.AddWithValue("@username", txtUsername.Text);
                    cmdPelanggan.Parameters.AddWithValue("@password", txtPassword.Text);
                    SqlDataReader reader = cmdPelanggan.ExecuteReader();

                    if (reader.Read())
                    {
                        string userLogin = reader["username"].ToString();
                        reader.Close();

                        FormUser userDashboard = new FormUser(userLogin);
                        userDashboard.Show();
                        this.Hide();
                    }
                    else
                    {
                        reader.Close();

                        // ✅ Login ADMIN pakai Stored Procedure
                        SqlCommand cmdAdmin = new SqlCommand("sp_LoginAdmin", conn);
                        cmdAdmin.CommandType = CommandType.StoredProcedure;
                        cmdAdmin.Parameters.AddWithValue("@username", txtUsername.Text);
                        cmdAdmin.Parameters.AddWithValue("@password", txtPassword.Text);
                        SqlDataReader readerAdmin = cmdAdmin.ExecuteReader();

                        if (readerAdmin.Read())
                        {
                            string adminLogin = readerAdmin["username"].ToString();
                            readerAdmin.Close();

                            FormAdminDashboard adminDashboard = new FormAdminDashboard(adminLogin);
                            adminDashboard.Show();
                            this.Hide();
                        }
                        else
                        {
                            readerAdmin.Close();
                            MessageBox.Show("Username atau Password salah!", "Akses Ditolak",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal terhubung ke database!\n" + ex.Message, "Error Koneksi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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

