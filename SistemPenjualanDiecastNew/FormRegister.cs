using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SistemPenjualanDiecastNew
{
    public partial class FormRegister : Form
    {
        // ✅ Semua deklarasi dikumpulkan di atas
        Label lblTitle, lblNama, lblUsername, lblPassword, lblConfirm;
        Label lblEmail, lblAlamat, lblNoHP;
        TextBox txtNama, txtUsername, txtPassword, txtConfirm;
        TextBox txtEmail, txtAlamat, txtNoHP;
        Button btnRegister, btnBack;

        public FormRegister()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // FORM — ✅ diperbesar untuk menampung field tambahan
            this.Text = "Register";
            this.Size = new Size(800, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.LightGray;

            // TITLE
            lblTitle = new Label();
            lblTitle.Text = "Daftar Akun";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(320, 30);

            // NAMA LENGKAP
            lblNama = new Label();
            lblNama.Text = "Nama Lengkap";
            lblNama.Location = new Point(180, 100);
            lblNama.AutoSize = true;

            txtNama = new TextBox();
            txtNama.Location = new Point(350, 100);
            txtNama.Size = new Size(220, 22);

            // USERNAME
            lblUsername = new Label();
            lblUsername.Text = "Username";
            lblUsername.Location = new Point(180, 140);
            lblUsername.AutoSize = true;

            txtUsername = new TextBox();
            txtUsername.Location = new Point(350, 140);
            txtUsername.Size = new Size(220, 22);

            // PASSWORD
            lblPassword = new Label();
            lblPassword.Text = "Password";
            lblPassword.Location = new Point(180, 180);
            lblPassword.AutoSize = true;

            txtPassword = new TextBox();
            txtPassword.Location = new Point(350, 180);
            txtPassword.Size = new Size(220, 22);
            txtPassword.UseSystemPasswordChar = true;

            // KONFIRMASI PASSWORD
            lblConfirm = new Label();
            lblConfirm.Text = "Konfirmasi Password";
            lblConfirm.Location = new Point(180, 220);
            lblConfirm.AutoSize = true;

            txtConfirm = new TextBox();
            txtConfirm.Location = new Point(350, 220);
            txtConfirm.Size = new Size(220, 22);
            txtConfirm.UseSystemPasswordChar = true;

            // ✅ EMAIL
            lblEmail = new Label();
            lblEmail.Text = "Email";
            lblEmail.Location = new Point(180, 260);
            lblEmail.AutoSize = true;

            txtEmail = new TextBox();
            txtEmail.Location = new Point(350, 260);
            txtEmail.Size = new Size(220, 22);

            // ✅ ALAMAT
            lblAlamat = new Label();
            lblAlamat.Text = "Alamat";
            lblAlamat.Location = new Point(180, 300);
            lblAlamat.AutoSize = true;

            txtAlamat = new TextBox();
            txtAlamat.Location = new Point(350, 300);
            txtAlamat.Size = new Size(220, 22);

            // ✅ NO HP
            lblNoHP = new Label();
            lblNoHP.Text = "No HP";
            lblNoHP.Location = new Point(180, 340);
            lblNoHP.AutoSize = true;

            txtNoHP = new TextBox();
            txtNoHP.Location = new Point(350, 340);
            txtNoHP.Size = new Size(220, 22);

            // BUTTON DAFTAR — ✅ digeser ke bawah
            btnRegister = new Button();
            btnRegister.Text = "Daftar";
            btnRegister.Location = new Point(350, 400);
            btnRegister.Size = new Size(100, 35);
            btnRegister.BackColor = Color.LightGreen;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.Click += btnRegister_Click;

            // BUTTON KEMBALI — ✅ digeser ke bawah
            btnBack = new Button();
            btnBack.Text = "Kembali";
            btnBack.Location = new Point(470, 400);
            btnBack.Size = new Size(100, 35);
            btnBack.BackColor = Color.LightCoral;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Click += btnBack_Click;

            // TAMBAH SEMUA KE FORM
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblNama);
            this.Controls.Add(txtNama);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(lblConfirm);
            this.Controls.Add(txtConfirm);
            this.Controls.Add(lblEmail);       // ✅
            this.Controls.Add(txtEmail);       // ✅
            this.Controls.Add(lblAlamat);      // ✅
            this.Controls.Add(txtAlamat);      // ✅
            this.Controls.Add(lblNoHP);        // ✅
            this.Controls.Add(txtNoHP);        // ✅
            this.Controls.Add(btnRegister);
            this.Controls.Add(btnBack);

            this.ResumeLayout(false);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // ✅ Validasi semua field wajib diisi termasuk yang baru
            if (txtNama.Text == "" || txtUsername.Text == "" ||
                txtPassword.Text == "" || txtConfirm.Text == "" ||
                txtEmail.Text == "" || txtAlamat.Text == "" ||
                txtNoHP.Text == "")
            {
                MessageBox.Show("Semua field wajib diisi!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi username tidak boleh ada spasi
            if (txtUsername.Text.Contains(" "))
            {
                MessageBox.Show("Username tidak boleh mengandung spasi!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ Validasi format email
            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
            {
                MessageBox.Show("Format email tidak valid! Contoh: nama@email.com", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi password sama
            if (txtPassword.Text.Length < 8)
            {
                MessageBox.Show("Password minimal 8 karakter!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // Cek username sudah terdaftar atau belum
                    string cekQuery = "SELECT COUNT(*) FROM PELANGGAN WHERE [username] = @username";
                    SqlCommand cekCmd = new SqlCommand(cekQuery, conn);
                    cekCmd.Parameters.AddWithValue("@username", txtUsername.Text);
                    int sudahAda = (int)cekCmd.ExecuteScalar();

                    if (sudahAda > 0)
                    {
                        MessageBox.Show("Username sudah terdaftar, gunakan username lain!", "Peringatan",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // ✅ Cek email sudah terdaftar atau belum
                    string cekEmail = "SELECT COUNT(*) FROM PELANGGAN WHERE email = @email";
                    SqlCommand cekEmailCmd = new SqlCommand(cekEmail, conn);
                    cekEmailCmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    int emailAda = (int)cekEmailCmd.ExecuteScalar();

                    if (emailAda > 0)
                    {
                        MessageBox.Show("Email sudah terdaftar, gunakan email lain!", "Peringatan",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // ✅ INSERT lengkap dengan email, alamat, no_telepon
                    string query = @"INSERT INTO PELANGGAN 
                                        (nama, [username], password, email, no_telepon, alamat) 
                                     VALUES 
                                        (@nama, @username, @password, @email, @notelp, @alamat)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                    cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@password", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);   // ✅
                    cmd.Parameters.AddWithValue("@notelp", txtNoHP.Text);    // ✅
                    cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text);  // ✅

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Registrasi berhasil! Silakan login.", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Form1 login = new Form1();
                    login.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error register: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Form1 login = new Form1();
            login.Show();
            this.Close();
        }
    }
}