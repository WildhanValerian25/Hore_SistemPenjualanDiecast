using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public partial class FormUser : Form
    {
        Label lblTitle, lblUsername, lblEmail, lblAlamat, lblNoHP, lblSaldo;
        // Tambahkan TextBox untuk mode edit
        TextBox txtEmail, txtAlamat, txtNoHP;
        DataGridView dgvProducts;
        Button btnBuy, btnAddDana, btnDeleteDana, btnLogout, btnEditProfile;

        private string _username;
        private int _currentSaldo = 0;
        private bool isEditMode = false; // Flag untuk status edit

        string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=DiecastDB;Integrated Security=True";

        public FormUser(string userLogin)
        {
            _username = userLogin ?? "";
            InitializeForm();
            BuildUI();

            try
            {
                LoadUserData();
                LoadProductData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Load += (s, e) => this.Close();
            }

            this.FormClosing += FormUser_FormClosing;
        }

        private void InitializeForm()
        {
            this.Text = "User Dashboard - Diecast Store";
            this.Size = new Size(920, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
        }

        private void BuildUI()
        {
            lblTitle = new Label() { Text = "USER PROFILE", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(30, 20), AutoSize = true };
            lblUsername = CreateLabel("Username: -", 50);

            // Label Profil
            lblEmail = CreateLabel("Email: -", 80);
            lblAlamat = CreateLabel("Alamat: -", 110);
            lblNoHP = CreateLabel("No HP: -", 140);

            // TextBox Profil (Awalnya disembunyikan)
            txtEmail = new TextBox() { Location = new Point(100, 77), Width = 150, Visible = false };
            txtAlamat = new TextBox() { Location = new Point(100, 107), Width = 150, Visible = false };
            txtNoHP = new TextBox() { Location = new Point(100, 137), Width = 150, Visible = false };

            lblSaldo = new Label()
            {
                Text = "Saldo: Rp 0",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                Location = new Point(30, 180),
                AutoSize = true
            };

            btnAddDana = new Button() { Text = "Tambah Dana", Location = new Point(30, 210), Size = new Size(110, 35), BackColor = Color.LightBlue, FlatStyle = FlatStyle.Flat };
            btnAddDana.Click += (s, e) => UpdateDana(50000);

            btnDeleteDana = new Button() { Text = "Reset Dana", Location = new Point(150, 210), Size = new Size(110, 35), BackColor = Color.MistyRose, FlatStyle = FlatStyle.Flat };
            btnDeleteDana.Click += (s, e) => { _currentSaldo = 0; UpdateSaldoLabel(); };

            // ================= EDIT PROFILE LOGIC =================
            btnEditProfile = new Button() { Text = "EDIT PROFILE", Location = new Point(30, 260), Size = new Size(230, 35), BackColor = Color.WhiteSmoke, FlatStyle = FlatStyle.Flat };
            btnEditProfile.Click += BtnEditProfile_Click;

            // ================= LOGOUT LOGIC =================
            btnLogout = new Button() { Text = "LOGOUT", Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(770, 15), Size = new Size(100, 30), BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnLogout.Click += BtnLogout_Click;

            Label lblMarket = new Label() { Text = "READY STOCK DIECAST", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(350, 20), AutoSize = true };

            dgvProducts = new DataGridView();
            dgvProducts.Location = new Point(350, 50);
            dgvProducts.Size = new Size(520, 350);
            dgvProducts.BackgroundColor = Color.White;
            dgvProducts.BorderStyle = BorderStyle.None;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.MultiSelect = false;
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.ReadOnly = true;
            dgvProducts.RowHeadersVisible = false;

            btnBuy = new Button() { Text = "BELI PRODUCT SEKARANG", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(350, 420), Size = new Size(520, 50), BackColor = Color.Black, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnBuy.Click += BtnBuy_Click;

            this.Controls.AddRange(new Control[] {
                lblTitle, lblUsername, lblEmail, lblAlamat, lblNoHP, lblSaldo,
                txtEmail, txtAlamat, txtNoHP, btnEditProfile,
                btnAddDana, btnDeleteDana, lblMarket, dgvProducts, btnBuy, btnLogout
            });
        }

        private void BtnEditProfile_Click(object sender, EventArgs e)
        {
            if (!isEditMode)
            {
                // Masuk ke Mode Edit
                isEditMode = true;
                btnEditProfile.Text = "SIMPAN PERUBAHAN";
                btnEditProfile.BackColor = Color.LightGreen;

                // Tampilkan TextBox, sembunyikan nilai statis label
                txtEmail.Text = lblEmail.Text.Replace("Email: ", "");
                txtAlamat.Text = lblAlamat.Text.Replace("Alamat: ", "");
                txtNoHP.Text = lblNoHP.Text.Replace("No HP: ", "");

                lblEmail.Text = "Email:";
                lblAlamat.Text = "Alamat:";
                lblNoHP.Text = "No HP:";

                txtEmail.Visible = txtAlamat.Visible = txtNoHP.Visible = true;
            }
            else
            {
                // Simpan Data ke Database
                SaveProfileChanges();
            }
        }

        private void SaveProfileChanges()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE Users SET Email=@e, Alamat=@a, NoHP=@n WHERE Username=@u";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@e", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@a", txtAlamat.Text);
                    cmd.Parameters.AddWithValue("@n", txtNoHP.Text);
                    cmd.Parameters.AddWithValue("@u", _username);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Profil berhasil diperbarui dan tersimpan di database!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Keluar dari mode edit
                    isEditMode = false;
                    btnEditProfile.Text = "EDIT PROFILE";
                    btnEditProfile.BackColor = Color.WhiteSmoke;
                    txtEmail.Visible = txtAlamat.Visible = txtNoHP.Visible = false;

                    LoadUserData(); // Muat ulang data ke label
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menyimpan: " + ex.Message);
                }
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout",
                                  MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                // 1. Beri tanda Tag agar event FormClosing tahu ini adalah logout resmi
                this.Tag = "Logout";

                // 2. Cari Form1 (Login) yang sedang di-hide
                Form loginForm = null;
                foreach (Form f in Application.OpenForms)
                {
                    if (f is Form1) // Mencari berdasarkan tipe class Form1
                    {
                        loginForm = f;
                        break;
                    }
                }

                // 3. Tampilkan kembali halaman login
                if (loginForm != null)
                {
                    loginForm.Show();

                    // Opsional: Reset input di Form1 jika ingin bersih kembali
                    if (loginForm.Controls["txtUsername"] is TextBox txtU) txtU.Clear();
                    if (loginForm.Controls["txtPassword"] is TextBox txtP) txtP.Clear();
                }
                else
                {
                    // Jika Form1 entah bagaimana sudah tertutup, buat baru
                    Form1 newLogin = new Form1();
                    newLogin.Show();
                }

                // 4. Tutup Dashboard User
                this.Close();
            }
        }

        private void FormUser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Tag == null || this.Tag.ToString() != "Logout")
            {
                Application.Exit();
            }
        }

        