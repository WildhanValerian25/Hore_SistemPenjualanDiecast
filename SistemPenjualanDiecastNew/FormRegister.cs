using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SistemPenjualanDiecastNew
{
    public partial class FormRegister : Form
    {
        Label lblTitle, lblNama, lblUsername, lblPassword, lblConfirm;
        TextBox txtNama, txtUsername, txtPassword, txtConfirm;
        Button btnRegister, btnBack;

        public FormRegister()
        {
            InitializeComponent();

        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // FORM
            this.Text = "Register";
            this.Size = new Size(800, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.LightGray;

            // TITLE
            lblTitle = new Label();
            lblTitle.Text = "Daftar Akun";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(320, 30);

            // NAMA
            lblNama = new Label();
            lblNama.Text = "Nama Lengkap";
            lblNama.Location = new Point(180, 120);
            lblNama.AutoSize = true;

            txtNama = new TextBox();
            txtNama.Location = new Point(350, 120);
            txtNama.Size = new Size(220, 22);

            // USERNAME
            lblUsername = new Label();
            lblUsername.Text = "Username";
            lblUsername.Location = new Point(180, 160);
            lblUsername.AutoSize = true;

            txtUsername = new TextBox();
            txtUsername.Location = new Point(350, 160);
            txtUsername.Size = new Size(220, 22);

            // PASSWORD
            lblPassword = new Label();
            lblPassword.Text = "Password";
            lblPassword.Location = new Point(180, 200);
            lblPassword.AutoSize = true;

            txtPassword = new TextBox();
            txtPassword.Location = new Point(350, 200);
            txtPassword.Size = new Size(220, 22);
            txtPassword.UseSystemPasswordChar = true;

            // CONFIRM PASSWORD
            lblConfirm = new Label();
            lblConfirm.Text = "Konfirmasi Password";
            lblConfirm.Location = new Point(180, 240);
            lblConfirm.AutoSize = true;

            txtConfirm = new TextBox();
            txtConfirm.Location = new Point(350, 240);
            txtConfirm.Size = new Size(220, 22);
            txtConfirm.UseSystemPasswordChar = true;

            // BUTTON REGISTER
            btnRegister = new Button();
            btnRegister.Text = "Daftar";
            btnRegister.Location = new Point(350, 290);
            btnRegister.Size = new Size(100, 35);
            btnRegister.BackColor = Color.LightGreen;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.Click += btnRegister_Click;

            // BUTTON BACK
            btnBack = new Button();
            btnBack.Text = "Kembali";
            btnBack.Location = new Point(470, 290);
            btnBack.Size = new Size(100, 35);
            btnBack.BackColor = Color.LightCoral;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Click += btnBack_Click;

            // ADD CONTROL
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblNama);
            this.Controls.Add(txtNama);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(lblConfirm);
            this.Controls.Add(txtConfirm);
            this.Controls.Add(btnRegister);
            this.Controls.Add(btnBack);

            this.ResumeLayout(false);
        }

        // EVENT REGISTER
        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (txtNama.Text == "" || txtUsername.Text == "" || txtPassword.Text == "" || txtConfirm.Text == "")
            {
                MessageBox.Show("Semua field wajib diisi!");
                return;
            }

            if (txtPassword.Text != txtConfirm.Text)
            {
                MessageBox.Show("Password tidak sama!");
                return;
            }

            string connStr = "Data Source=LAPTOP-24A5CGHI\\WILDHANFIGHT;Initial Catalog=DiecastDB;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open(); // 🔥 WAJIB DI SINI

                    string query = "INSERT INTO Users (Nama, Username, Password, Role) VALUES (@nama, @username, @password, @role)";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                    cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@password", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@role", "User"); // default user

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Registrasi berhasil!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error register: " + ex.Message);
                }
            }

            Form1 login = new Form1();
            login.Show();
            this.Close();
        }

        // EVENT BACK
        private void btnBack_Click(object sender, EventArgs e)
        {
            Form1 login = new Form1();
            login.Show();
            this.Close();
        }
    }
}