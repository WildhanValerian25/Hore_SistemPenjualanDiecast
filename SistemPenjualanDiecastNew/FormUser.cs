using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public partial class FormUser : Form
    {
        Label lblTitle, lblUsername, lblEmail, lblAlamat, lblNoHP, lblSaldo;
        TextBox txtEmail, txtAlamat, txtNoHP;
        DataGridView dgvProducts;
        Button btnBuy, btnAddDana, btnDeleteDana, btnLogout, btnEditProfile;
        Button btnRiwayat, btnLaporanSaya;
        Panel pnlProfile, pnlMarket;

        private BindingSource _bindingSource = new BindingSource();

        private string _username;
        private int _currentSaldo = 0;
        private bool isEditMode = false;

        string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

        private readonly Color cBgDark = Color.FromArgb(8, 18, 38);
        private readonly Color cBgMid = Color.FromArgb(11, 30, 62);
        private readonly Color cCard = Color.FromArgb(14, 38, 78);
        private readonly Color cCardBord = Color.FromArgb(25, 60, 110);
        private readonly Color cAccent = Color.FromArgb(26, 86, 219);
        private readonly Color cAccentHover = Color.FromArgb(35, 100, 235);
        private readonly Color cAccentDown = Color.FromArgb(15, 60, 170);
        private readonly Color cInputBg = Color.FromArgb(10, 26, 55);
        private readonly Color cInputBrd = Color.FromArgb(40, 70, 130);
        private readonly Color cTextPri = Color.FromArgb(230, 238, 255);
        private readonly Color cTextMut = Color.FromArgb(100, 130, 180);

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
            this.Size = new Size(960, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = cBgDark;
            this.Font = new Font("Segoe UI", 9.5F);
            this.Paint += Form_Paint;
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush br = new LinearGradientBrush(
                this.ClientRectangle, cBgDark, cBgMid, LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(br, this.ClientRectangle);
            }
        }

        private void BuildUI()
        {
            // ── CARD PROFIL (kiri) ───────────────────────────────
            pnlProfile = CreateCardPanel(24, 16, 300, 568);

            lblTitle = new Label()
            {
                Text = "USER PROFILE",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = cTextPri,
                BackColor = Color.Transparent,
                Location = new Point(20, 18),
                AutoSize = true
            };

            lblUsername = CreateProfileLabel("Username: -", 58);
            lblEmail = CreateProfileLabel("Email: -", 88);
            lblAlamat = CreateProfileLabel("Alamat: -", 118);
            lblNoHP = CreateProfileLabel("No HP: -", 148);

            txtEmail = CreateDarkTextBox(20, 85, 150);
            txtAlamat = CreateDarkTextBox(20, 115, 150);
            txtNoHP = CreateDarkTextBox(20, 145, 150);
            txtEmail.Visible = txtAlamat.Visible = txtNoHP.Visible = false;

            lblSaldo = new Label()
            {
                Text = "Saldo: Rp 0",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 220, 150),
                BackColor = Color.Transparent,
                Location = new Point(20, 188),
                AutoSize = true
            };

            btnAddDana = CreateUserButton("Tambah Dana", 20, 225, 125, cAccent);
            btnAddDana.Click += (s, e) => UpdateDana(50000);

            btnDeleteDana = CreateUserButton("Reset Dana", 153, 225, 125, Color.FromArgb(150, 45, 55));
            btnDeleteDana.Click += (s, e) => { _currentSaldo = 0; UpdateSaldoLabel(); };

            btnEditProfile = CreateUserButton("EDIT PROFILE", 20, 273, 258, Color.FromArgb(20, 50, 100));
            btnEditProfile.FlatAppearance.BorderSize = 1;
            btnEditProfile.FlatAppearance.BorderColor = cInputBrd;
            btnEditProfile.Click += BtnEditProfile_Click;

            btnRiwayat = CreateUserButton("RIWAYAT & BAYAR PESANAN", 20, 321, 258, cAccent, 40);
            btnRiwayat.Click += BtnRiwayat_Click;

            // ✅ Tombol Laporan Pesanan Saya
            btnLaporanSaya = CreateUserButton("📄 LAPORAN PESANAN SAYA", 20, 373, 258,
                Color.FromArgb(40, 100, 60), 40);
            btnLaporanSaya.Click += (s, e) =>
            {
                FormLaporanUser f = new FormLaporanUser(_username);
                f.ShowDialog();
            };

            pnlProfile.Controls.AddRange(new Control[] {
                lblTitle, lblUsername, lblEmail, lblAlamat, lblNoHP,
                txtEmail, txtAlamat, txtNoHP,
                lblSaldo, btnAddDana, btnDeleteDana,
                btnEditProfile, btnRiwayat,
                btnLaporanSaya  // ✅ ditambahkan
            });

            // ── TOMBOL LOGOUT ──
            btnLogout = new Button()
            {
                Text = "LOGOUT",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(840, 16),
                Size = new Size(96, 32),
                BackColor = Color.FromArgb(150, 45, 55),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(170, 55, 65);
            ApplyRoundRegion(btnLogout, 8);
            btnLogout.Click += BtnLogout_Click;

            // ── CARD MARKET (kanan) ──────────────────────────────
            pnlMarket = CreateCardPanel(340, 16, 596, 568);

            Label lblMarket = new Label()
            {
                Text = "READY STOCK DIECAST",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = cTextPri,
                BackColor = Color.Transparent,
                Location = new Point(20, 18),
                AutoSize = true
            };

            dgvProducts = new DataGridView()
            {
                Location = new Point(20, 56),
                Size = new Size(556, 420),
                BackgroundColor = cInputBg,
                GridColor = cCardBord,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToResizeRows = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 34,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                Font = new Font("Segoe UI", 9F)
            };

            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = cCard;
            dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor = cTextPri;
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvProducts.ColumnHeadersDefaultCellStyle.SelectionBackColor = cCard;

            dgvProducts.DefaultCellStyle.BackColor = cInputBg;
            dgvProducts.DefaultCellStyle.ForeColor = cTextPri;
            dgvProducts.DefaultCellStyle.SelectionBackColor = cAccent;
            dgvProducts.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(13, 32, 64);
            dgvProducts.AlternatingRowsDefaultCellStyle.ForeColor = cTextPri;
            dgvProducts.AlternatingRowsDefaultCellStyle.SelectionBackColor = cAccent;
            dgvProducts.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            dgvProducts.RowTemplate.Height = 30;
            dgvProducts.DataSource = _bindingSource;

            dgvProducts.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    dgvProducts.Rows[e.RowIndex].Selected = true;
            };

            btnBuy = new Button()
            {
                Text = "BELI PRODUK SEKARANG",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Location = new Point(20, 490),
                Size = new Size(556, 48),
                BackColor = cAccent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBuy.FlatAppearance.BorderSize = 0;
            btnBuy.FlatAppearance.MouseOverBackColor = cAccentHover;
            btnBuy.FlatAppearance.MouseDownBackColor = cAccentDown;
            ApplyRoundRegion(btnBuy, 8);
            btnBuy.Click += BtnBuy_Click;

            pnlMarket.Controls.AddRange(new Control[] { lblMarket, dgvProducts, btnBuy });

            this.Controls.AddRange(new Control[] { pnlProfile, pnlMarket, btnLogout });
        }

        // ════════════════════════════════════════════════════════
        // HELPERS VISUAL
        // ════════════════════════════════════════════════════════
        private Panel CreateCardPanel(int x, int y, int w, int h)
        {
            Panel p = new Panel()
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = Color.Transparent
            };
            p.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle r = new Rectangle(0, 0, p.Width - 1, p.Height - 1);
                using (GraphicsPath path = RoundedPath(r, 14))
                {
                    using (SolidBrush fillBr = new SolidBrush(cCard))
                        g.FillPath(fillBr, path);
                    using (Pen pen = new Pen(cCardBord, 1f))
                        g.DrawPath(pen, path);
                    p.Region = new Region(path);
                }
            };
            return p;
        }

        private Label CreateProfileLabel(string text, int top)
        {
            return new Label()
            {
                Text = text,
                Location = new Point(20, top),
                AutoSize = true,
                BackColor = Color.Transparent,
                ForeColor = cTextMut,
                Font = new Font("Segoe UI", 9.5F)
            };
        }

        private TextBox CreateDarkTextBox(int x, int y, int width)
        {
            return new TextBox()
            {
                Location = new Point(x, y),
                Width = width,
                BackColor = cInputBg,
                ForeColor = cTextPri,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5F)
            };
        }

        private Button CreateUserButton(string text, int x, int y, int width,
            Color baseColor, int height = 35)
        {
            Button b = new Button()
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = baseColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            ApplyRoundRegion(b, 8);
            return b;
        }

        private static GraphicsPath RoundedPath(Rectangle r, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void ApplyRoundRegion(Control c, int radius)
        {
            if (c.Width <= 0 || c.Height <= 0) return;
            Rectangle r = new Rectangle(0, 0, c.Width, c.Height);
            c.Region = new Region(RoundedPath(r, radius));
        }

        // ════════════════════════════════════════════════════════
        // LOGIKA
        // ════════════════════════════════════════════════════════
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
                btnEditProfile.BackColor = Color.FromArgb(35, 130, 90);

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
                        btnEditProfile.BackColor = Color.FromArgb(20, 50, 100);
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

                    using (SqlCommand cmd = new SqlCommand("sp_GetProfilPelanggan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@username", _username);

                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
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

                    using (SqlCommand cmd = new SqlCommand("sp_GetProdukAktif", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        _bindingSource.DataSource = dt;
                    }

                    if (dgvProducts.Columns.Contains("id_produk"))
                        dgvProducts.Columns["id_produk"].Visible = false;

                    if (dgvProducts.Columns.Contains("Harga"))
                    {
                        dgvProducts.Columns["Harga"].DefaultCellStyle.Format = "N0";
                        dgvProducts.Columns["Harga"].DefaultCellStyle.FormatProvider =
                            new System.Globalization.CultureInfo("id-ID");
                    }

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

        private void DgvProducts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (!dgvProducts.Columns.Contains("Stok")) return;

            object stokVal = dgvProducts.Rows[e.RowIndex].Cells["Stok"].Value;
            if (stokVal == null) return;

            int stok = Convert.ToInt32(stokVal);
            Color warna;

            if (stok == 0) warna = Color.FromArgb(70, 25, 30);
            else if (stok <= 5) warna = Color.FromArgb(70, 60, 25);
            else warna = cInputBg;

            dgvProducts.Rows[e.RowIndex].DefaultCellStyle.BackColor = warna;
            dgvProducts.Rows[e.RowIndex].DefaultCellStyle.ForeColor = cTextPri;
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
            DialogResult confirm = MessageBox.Show(
                "Apakah Anda yakin ingin logout?", "Konfirmasi Logout",
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
    }
}