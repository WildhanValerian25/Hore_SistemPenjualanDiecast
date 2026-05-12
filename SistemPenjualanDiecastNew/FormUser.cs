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
        TextBox txtEmail, txtAlamat, txtNoHP;
        DataGridView dgvProducts;
        Button btnBuy, btnAddDana, btnDeleteDana, btnLogout, btnEditProfile;
        Button btnRiwayat;

        // ✅ BindingSource untuk DataGridView
        private BindingSource _bindingSource = new BindingSource();

        private string _username;
        private int _currentSaldo = 0;
        private bool isEditMode = false;

        string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

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
            this.Size = new Size(920, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
        }

        private void BuildUI()
        {
            lblTitle = new Label() { Text = "USER PROFILE", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(30, 20), AutoSize = true };
            lblUsername = CreateLabel("Username: -", 50);
            lblEmail = CreateLabel("Email: -", 80);
            lblAlamat = CreateLabel("Alamat: -", 110);
            lblNoHP = CreateLabel("No HP: -", 140);

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

            btnAddDana = new Button()
            {
                Text = "Tambah Dana",
                Location = new Point(30, 210),
                Size = new Size(110, 35),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat
            };
            btnAddDana.Click += (s, e) => UpdateDana(50000);

            btnDeleteDana = new Button()
            {
                Text = "Reset Dana",
                Location = new Point(150, 210),
                Size = new Size(110, 35),
                BackColor = Color.MistyRose,
                FlatStyle = FlatStyle.Flat
            };
            btnDeleteDana.Click += (s, e) => { _currentSaldo = 0; UpdateSaldoLabel(); };

            btnEditProfile = new Button()
            {
                Text = "EDIT PROFILE",
                Location = new Point(30, 260),
                Size = new Size(230, 35),
                BackColor = Color.WhiteSmoke,
                FlatStyle = FlatStyle.Flat
            };
            btnEditProfile.Click += BtnEditProfile_Click;

            btnRiwayat = new Button()
            {
                Text = "RIWAYAT & BAYAR PESANAN",
                Location = new Point(30, 310),
                Size = new Size(230, 40),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRiwayat.Click += BtnRiwayat_Click;

            btnLogout = new Button()
            {
                Text = "LOGOUT",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(770, 15),
                Size = new Size(100, 30),
                BackColor = Color.Crimson,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogout.Click += BtnLogout_Click;

            Label lblMarket = new Label()
            {
                Text = "READY STOCK DIECAST",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(350, 20),
                AutoSize = true
            };

            dgvProducts = new DataGridView()
            {
                Location = new Point(350, 50),
                Size = new Size(520, 400),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToResizeRows = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.LightGray,
                DefaultCellStyle = { SelectionBackColor = Color.SteelBlue, SelectionForeColor = Color.White }
            };

            // ✅ Hubungkan BindingSource ke DataGridView
            dgvProducts.DataSource = _bindingSource;

            dgvProducts.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    dgvProducts.Rows[e.RowIndex].Selected = true;
            };

            btnBuy = new Button()
            {
                Text = "BELI PRODUK SEKARANG",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(350, 465),
                Size = new Size(520, 50),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBuy.Click += BtnBuy_Click;

            this.Controls.AddRange(new Control[] {
                lblTitle, lblUsername, lblEmail, lblAlamat, lblNoHP, lblSaldo,
                txtEmail, txtAlamat, txtNoHP, btnEditProfile,
                btnAddDana, btnDeleteDana, btnRiwayat,
                lblMarket, dgvProducts, btnBuy, btnLogout
            });
        }

        private void BtnRiwayat_Click(object sender, EventArgs e)
        {
            FormRiwayatPesanan formRiwayat = new FormRiwayatPesanan(_username);
            formRiwayat.ShowDialog();
            LoadProductData();
        }

        private void BtnEditProfile_Click(object sender, EventArgs e)
        {
            if (!isEditMode)
            {
                isEditMode = true;
                btnEditProfile.Text = "SIMPAN PERUBAHAN";
                btnEditProfile.BackColor = Color.LightGreen;

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

                    // ✅ Pakai Stored Procedure
                    SqlCommand cmd = new SqlCommand("sp_UpdateProfilPelanggan", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@username", _username);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text);
                    cmd.Parameters.AddWithValue("@no_telepon", txtNoHP.Text);

                    SqlParameter pHasil = new SqlParameter("@hasil", SqlDbType.Int)
                    { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(pHasil);
                    cmd.ExecuteNonQuery();

                    if (Convert.ToInt32(pHasil.Value) == 1)
                    {
                        MessageBox.Show("Profil berhasil diperbarui!", "Sukses",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        isEditMode = false;
                        btnEditProfile.Text = "EDIT PROFILE";
                        btnEditProfile.BackColor = Color.WhiteSmoke;
                        txtEmail.Visible = txtAlamat.Visible = txtNoHP.Visible = false;
                        LoadUserData();
                    }
                    else
                    {
                        MessageBox.Show("Gagal memperbarui profil!", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menyimpan: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadUserData()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT [username], email, alamat, no_telepon 
                                         FROM PELANGGAN 
                                         WHERE [username] = @u";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@u", _username);
                    SqlDataReader r = cmd.ExecuteReader();

                    if (r.Read())
                    {
                        string email = r["email"] == DBNull.Value || r["email"].ToString() == "" ? "-" : r["email"].ToString();
                        string alamat = r["alamat"] == DBNull.Value || r["alamat"].ToString() == "" ? "-" : r["alamat"].ToString();
                        string noTelepon = r["no_telepon"] == DBNull.Value || r["no_telepon"].ToString() == "" ? "-" : r["no_telepon"].ToString();

                        lblUsername.Text = "Username: " + r["username"].ToString();
                        lblEmail.Text = "Email: " + email;
                        lblAlamat.Text = "Alamat: " + alamat;
                        lblNoHP.Text = "No HP: " + noTelepon;
                    }
                    else
                    {
                        lblUsername.Text = "Username: " + _username;
                        lblEmail.Text = "Email: -";
                        lblAlamat.Text = "Alamat: -";
                        lblNoHP.Text = "No HP: -";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat profil: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadProductData()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // ✅ Pakai VIEW vw_ProdukAktif
                    string query = @"SELECT id_produk,
                        nama_produk AS Nama_Diecast,
                        merek       AS Merek,
                        harga       AS Harga,
                        stok        AS Stok
                        FROM vw_ProdukAktif
                        WHERE stok > 0";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // ✅ Set data ke BindingSource
                    _bindingSource.DataSource = dt;

                    if (dgvProducts.Columns.Contains("id_produk"))
                        dgvProducts.Columns["id_produk"].Visible = false;

                    if (dgvProducts.Columns.Contains("Harga"))
                    {
                        dgvProducts.Columns["Harga"].DefaultCellStyle.Format = "N0";
                        dgvProducts.Columns["Harga"].DefaultCellStyle.FormatProvider =
                            new System.Globalization.CultureInfo("id-ID");
                    }

                    // ✅ Warnai baris berdasarkan status stok
                    dgvProducts.CellFormatting -= DgvProducts_CellFormatting;
                    dgvProducts.CellFormatting += DgvProducts_CellFormatting;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat produk: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ✅ Warnai baris sesuai status stok
        private void DgvProducts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // ✅ Warnai berdasarkan kolom Stok, bukan Status
            if (!dgvProducts.Columns.Contains("Stok")) return;

            object stokVal = dgvProducts.Rows[e.RowIndex].Cells["Stok"].Value;
            if (stokVal == null) return;

            int stok = Convert.ToInt32(stokVal);
            Color warna;

            if (stok == 0) warna = Color.FromArgb(255, 200, 200); // Merah - Habis
            else if (stok <= 5) warna = Color.FromArgb(255, 243, 205); // Kuning - Hampir Habis
            else warna = Color.White;                    // Normal

            dgvProducts.Rows[e.RowIndex].DefaultCellStyle.BackColor = warna;
        }
        private void UpdateDana(int jumlah)
        {
            _currentSaldo += jumlah;
            UpdateSaldoLabel();
        }

        private void UpdateSaldoLabel()
        {
            lblSaldo.Text = "Saldo: Rp " + _currentSaldo.ToString("N0");
        }

        private void BtnBuy_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih produk terlebih dahulu!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataGridViewRow row = dgvProducts.SelectedRows[0];

                if (row.Cells["id_produk"].Value == null)
                {
                    MessageBox.Show("Data produk tidak valid!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int idProduk = Convert.ToInt32(row.Cells["id_produk"].Value);
                string namaProduk = row.Cells["Nama_Diecast"].Value.ToString();
                decimal harga = Convert.ToDecimal(row.Cells["Harga"].Value);
                int stok = Convert.ToInt32(row.Cells["Stok"].Value);

                FormBeli formBeli = new FormBeli(_username, idProduk, namaProduk, harga, stok);
                if (formBeli.ShowDialog() == DialogResult.OK)
                    LoadProductData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout",
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                this.Tag = "Logout";

                Form loginForm = null;
                foreach (Form f in Application.OpenForms)
                {
                    if (f is Form1) { loginForm = f; break; }
                }

                if (loginForm != null)
                {
                    loginForm.Show();
                    if (loginForm.Controls["txtUsername"] is TextBox txtU) txtU.Clear();
                    if (loginForm.Controls["txtPassword"] is TextBox txtP) txtP.Clear();
                }
                else
                {
                    new Form1().Show();
                }

                this.Close();
            }
        }

        private void FormUser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Tag == null || this.Tag.ToString() != "Logout")
                Application.Exit();
        }

        private Label CreateLabel(string text, int top)
        {
            return new Label()
            {
                Text = text,
                Location = new Point(30, top),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };
        }
    }
}