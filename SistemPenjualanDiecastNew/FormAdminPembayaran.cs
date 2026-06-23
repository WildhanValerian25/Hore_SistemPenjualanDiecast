using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public class FormAdminPembayaran : Form
    {
        DataGridView dgvPembayaran;
        Button btnKonfirmasiLunas, btnTolak, btnRefresh, btnLihatBukti, btnInputResi, btnBack;
        Label lblTitle;
        ComboBox cmbFilter;
        Label lblFilter;
        PictureBox picBukti;
        Panel panelBukti;
        Label lblBuktiTitle;

        private BindingSource _bindingSource = new BindingSource();
        private BindingNavigator _navigator;

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

        public FormAdminPembayaran()
        {
            this.Text = "Konfirmasi Pembayaran";
            this.Size = new Size(1100, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = cBgDark;
            this.Font = new Font("Segoe UI", 9);
            this.Paint += Form_Paint;

            BuildUI();
            LoadPembayaran();
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
            btnBack = new Button()
            {
                Text = "← Kembali",
                Location = new Point(20, 15),
                Size = new Size(100, 32),
                BackColor = Color.FromArgb(20, 50, 100),
                ForeColor = Color.FromArgb(150, 180, 230),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 1;
            btnBack.FlatAppearance.BorderColor = cInputBrd;
            btnBack.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 65, 120);
            ApplyRoundRegion(btnBack, 8);
            btnBack.Click += (s, e) => this.Close();

            lblTitle = new Label()
            {
                Text = "MANAJEMEN PEMBAYARAN",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(132, 19),
                AutoSize = true,
                BackColor = Color.Transparent,
                ForeColor = cTextPri
            };

            lblFilter = new Label()
            {
                Text = "Filter:",
                Location = new Point(20, 65),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.Transparent,
                ForeColor = cTextMut
            };

            cmbFilter = new ComboBox()
            {
                Location = new Point(65, 62),
                Size = new Size(160, 22),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                BackColor = cInputBg,
                ForeColor = cTextPri
            };
            cmbFilter.Items.AddRange(new string[] {
                "Semua", "Menunggu", "Lunas", "Gagal", "Dikembalikan"
            });
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += (s, e) => LoadPembayaran();

            _navigator = new BindingNavigator(true)
            {
                BindingSource = _bindingSource,
                Location = new Point(20, 92),
                Size = new Size(650, 25),
                BackColor = cCard,
                ForeColor = cTextPri
            };
            foreach (ToolStripItem item in _navigator.Items)
                item.ForeColor = cTextPri;

            dgvPembayaran = new DataGridView()
            {
                Location = new Point(20, 122),
                Size = new Size(650, 380),
                BackgroundColor = cInputBg,
                GridColor = cCardBord,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9),
                DataSource = _bindingSource
            };

            dgvPembayaran.ColumnHeadersDefaultCellStyle.BackColor = cCard;
            dgvPembayaran.ColumnHeadersDefaultCellStyle.ForeColor = cTextPri;
            dgvPembayaran.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvPembayaran.ColumnHeadersDefaultCellStyle.SelectionBackColor = cCard;
            dgvPembayaran.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvPembayaran.ColumnHeadersHeight = 34;

            dgvPembayaran.DefaultCellStyle.BackColor = cInputBg;
            dgvPembayaran.DefaultCellStyle.ForeColor = cTextPri;
            dgvPembayaran.DefaultCellStyle.SelectionBackColor = cAccent;
            dgvPembayaran.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvPembayaran.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(13, 32, 64);
            dgvPembayaran.AlternatingRowsDefaultCellStyle.ForeColor = cTextPri;
            dgvPembayaran.AlternatingRowsDefaultCellStyle.SelectionBackColor = cAccent;
            dgvPembayaran.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            dgvPembayaran.RowTemplate.Height = 30;
            dgvPembayaran.SelectionChanged += DgvPembayaran_SelectionChanged;
            dgvPembayaran.CellFormatting += DgvPembayaran_CellFormatting;

            // Panel preview bukti
            panelBukti = new Panel()
            {
                Location = new Point(690, 92),
                Size = new Size(385, 410),
                BackColor = Color.Transparent
            };
            panelBukti.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle r = new Rectangle(0, 0, panelBukti.Width - 1, panelBukti.Height - 1);
                using (GraphicsPath path = RoundedPath(r, 12))
                {
                    using (SolidBrush fillBr = new SolidBrush(cCard))
                        g.FillPath(fillBr, path);
                    using (Pen pen = new Pen(cCardBord, 1f))
                        g.DrawPath(pen, path);
                    panelBukti.Region = new Region(path);
                }
            };

            lblBuktiTitle = new Label()
            {
                Text = "Bukti Pembayaran",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(14, 14),
                AutoSize = true,
                BackColor = Color.Transparent,
                ForeColor = cTextPri
            };

            picBukti = new PictureBox()
            {
                Location = new Point(14, 46),
                Size = new Size(357, 320),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = cInputBg,
                BorderStyle = BorderStyle.FixedSingle
            };
            picBukti.Controls.Add(new Label()
            {
                Text = "Pilih baris untuk\nmelihat bukti pembayaran",
                ForeColor = cTextMut,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(85, 140),
                AutoSize = true
            });

            btnLihatBukti = new Button()
            {
                Text = "Lihat Bukti Full",
                Location = new Point(14, 372),
                Size = new Size(357, 32),
                BackColor = cAccent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLihatBukti.FlatAppearance.BorderSize = 0;
            btnLihatBukti.FlatAppearance.MouseOverBackColor = cAccentHover;
            btnLihatBukti.FlatAppearance.MouseDownBackColor = cAccentDown;
            ApplyRoundRegion(btnLihatBukti, 8);
            btnLihatBukti.Click += BtnLihatBukti_Click;

            panelBukti.Controls.AddRange(new Control[] { lblBuktiTitle, picBukti, btnLihatBukti });

            // Tombol aksi
            btnKonfirmasiLunas = new Button()
            {
                Text = "KONFIRMASI LUNAS",
                Location = new Point(20, 538),
                Size = new Size(155, 42),
                BackColor = Color.FromArgb(35, 130, 90),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnKonfirmasiLunas.FlatAppearance.BorderSize = 0;
            btnKonfirmasiLunas.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 150, 105);
            ApplyRoundRegion(btnKonfirmasiLunas, 8);
            btnKonfirmasiLunas.Click += BtnKonfirmasiLunas_Click;

            btnTolak = new Button()
            {
                Text = "TOLAK PEMBAYARAN",
                Location = new Point(185, 538),
                Size = new Size(155, 42),
                BackColor = Color.FromArgb(150, 45, 55),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnTolak.FlatAppearance.BorderSize = 0;
            btnTolak.FlatAppearance.MouseOverBackColor = Color.FromArgb(170, 55, 65);
            ApplyRoundRegion(btnTolak, 8);
            btnTolak.Click += BtnTolak_Click;

            btnInputResi = new Button()
            {
                Text = "INPUT NOMOR RESI",
                Location = new Point(350, 538),
                Size = new Size(155, 42),
                BackColor = Color.FromArgb(200, 130, 30),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnInputResi.FlatAppearance.BorderSize = 0;
            btnInputResi.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 145, 40);
            ApplyRoundRegion(btnInputResi, 8);
            btnInputResi.Click += BtnInputResi_Click;

            btnRefresh = new Button()
            {
                Text = "REFRESH",
                Location = new Point(515, 538),
                Size = new Size(110, 42),
                BackColor = cAccent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.FlatAppearance.MouseOverBackColor = cAccentHover;
            ApplyRoundRegion(btnRefresh, 8);
            btnRefresh.Click += (s, e) => LoadPembayaran();

            Label lblInfo = new Label()
            {
                Name = "lblInfoData",
                Text = "",
                Font = new Font("Segoe UI", 8),
                ForeColor = cTextMut,
                BackColor = Color.Transparent,
                Location = new Point(20, 596),
                AutoSize = true
            };

            this.Controls.AddRange(new Control[] {
                btnBack, lblTitle, lblFilter, cmbFilter,
                _navigator, dgvPembayaran, panelBukti,
                btnKonfirmasiLunas, btnTolak, btnInputResi, btnRefresh,
                lblInfo
            });

            _bindingSource.ListChanged += (s, e) =>
            {
                lblInfo.Text = $"Total data: {_bindingSource.Count} baris";
            };
        }

        // ════════════════════════════════════════════════════════
        // HELPERS VISUAL
        // ════════════════════════════════════════════════════════
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
        // ✅ LoadPembayaran — pakai sp_GetPembayaranAdmin
        // ════════════════════════════════════════════════════════
        private void LoadPembayaran()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string filter = cmbFilter.SelectedItem.ToString();

                    SqlCommand cmd = new SqlCommand("sp_GetPembayaranAdmin", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@status",
                        filter == "Semua" ? (object)DBNull.Value : filter);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    _bindingSource.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal load pembayaran: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DgvPembayaran_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string status = dgvPembayaran.Rows[e.RowIndex].Cells["Status Bayar"]?.Value?.ToString();
            Color warna;
            switch (status)
            {
                case "Menunggu": warna = Color.FromArgb(70, 60, 25); break;
                case "Lunas": warna = Color.FromArgb(20, 55, 35); break;
                case "Gagal": warna = Color.FromArgb(70, 25, 30); break;
                case "Dikembalikan": warna = Color.FromArgb(25, 45, 80); break;
                default: warna = cInputBg; break;
            }
            dgvPembayaran.Rows[e.RowIndex].DefaultCellStyle.BackColor = warna;
            dgvPembayaran.Rows[e.RowIndex].DefaultCellStyle.ForeColor = cTextPri;
        }

        private void DgvPembayaran_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPembayaran.SelectedRows.Count == 0) return;
            if (dgvPembayaran.SelectedRows[0].Cells["ID Bayar"].Value == null) return;
            int idPembayaran = Convert.ToInt32(dgvPembayaran.SelectedRows[0].Cells["ID Bayar"].Value);
            TampilkanBukti(idPembayaran);
        }

        // ════════════════════════════════════════════════════════
        // ✅ TampilkanBukti — pakai sp_GetBuktiPembayaran
        // ════════════════════════════════════════════════════════
        private void TampilkanBukti(int idPembayaran)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("sp_GetBuktiPembayaran", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_pembayaran", idPembayaran);

                    object result = cmd.ExecuteScalar();

                    picBukti.Controls.Clear();

                    if (result != null && result != DBNull.Value)
                    {
                        byte[] imgBytes = (byte[])result;
                        using (var ms = new System.IO.MemoryStream(imgBytes))
                            picBukti.Image = System.Drawing.Image.FromStream(ms);

                        lblBuktiTitle.Text = "Bukti Pembayaran — Ada";
                        lblBuktiTitle.ForeColor = Color.FromArgb(120, 220, 150);
                    }
                    else
                    {
                        picBukti.Image = null;
                        lblBuktiTitle.Text = "Bukti Pembayaran — Tidak Ada";
                        lblBuktiTitle.ForeColor = Color.FromArgb(255, 120, 130);

                        picBukti.Controls.Add(new Label()
                        {
                            Text = "Tidak ada bukti pembayaran",
                            ForeColor = cTextMut,
                            BackColor = Color.Transparent,
                            Font = new Font("Segoe UI", 9),
                            TextAlign = ContentAlignment.MiddleCenter,
                            Location = new Point(75, 140),
                            AutoSize = true
                        });
                    }
                }
                catch { picBukti.Image = null; }
            }
        }

        private void BtnLihatBukti_Click(object sender, EventArgs e)
        {
            if (picBukti.Image == null)
            {
                MessageBox.Show("Tidak ada bukti pembayaran untuk pesanan ini.", "Info");
                return;
            }
            Form f = new Form()
            {
                Text = "Bukti Pembayaran",
                Size = new Size(700, 600),
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = cBgDark
            };
            f.Controls.Add(new PictureBox()
            {
                Dock = DockStyle.Fill,
                Image = picBukti.Image,
                SizeMode = PictureBoxSizeMode.Zoom
            });
            f.Show();
        }

        // ════════════════════════════════════════════════════════
        // ✅ BtnKonfirmasiLunas_Click — pakai sp_KonfirmasiLunas
        // ════════════════════════════════════════════════════════
        private void BtnKonfirmasiLunas_Click(object sender, EventArgs e)
        {
            if (dgvPembayaran.SelectedRows.Count == 0)
            { MessageBox.Show("Pilih data pembayaran terlebih dahulu!"); return; }

            int idPesanan = Convert.ToInt32(dgvPembayaran.SelectedRows[0].Cells["ID Pesanan"].Value);
            string statusBayar = dgvPembayaran.SelectedRows[0].Cells["Status Bayar"].Value.ToString();

            if (statusBayar == "Lunas")
            { MessageBox.Show("Pembayaran ini sudah dikonfirmasi Lunas.", "Info"); return; }

            if (statusBayar == "Gagal" || statusBayar == "Dikembalikan")
            { MessageBox.Show($"Status '{statusBayar}', tidak bisa dikonfirmasi.", "Info"); return; }

            if (MessageBox.Show($"Konfirmasi pesanan #{idPesanan} sebagai LUNAS?",
                "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("sp_KonfirmasiLunas", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_pesanan", idPesanan);

                    SqlParameter pHasil = new SqlParameter("@hasil", SqlDbType.Int)
                    { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(pHasil);

                    cmd.ExecuteNonQuery();
                    int hasil = Convert.ToInt32(pHasil.Value);

                    if (hasil == 1)
                    {
                        MessageBox.Show(
                            $"Pesanan #{idPesanan} dikonfirmasi Lunas!\nStatus otomatis 'Diproses'.",
                            "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPembayaran();
                    }
                    else
                    {
                        MessageBox.Show("Gagal konfirmasi. Coba lagi.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal konfirmasi: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ════════════════════════════════════════════════════════
        // ✅ BtnTolak_Click — pakai sp_TolakPembayaranAdmin
        // ════════════════════════════════════════════════════════
        private void BtnTolak_Click(object sender, EventArgs e)
        {
            if (dgvPembayaran.SelectedRows.Count == 0)
            { MessageBox.Show("Pilih data pembayaran terlebih dahulu!"); return; }

            int idPesanan = Convert.ToInt32(dgvPembayaran.SelectedRows[0].Cells["ID Pesanan"].Value);
            string statusBayar = dgvPembayaran.SelectedRows[0].Cells["Status Bayar"].Value.ToString();

            if (statusBayar == "Lunas")
            { MessageBox.Show("Pembayaran sudah Lunas, tidak bisa ditolak!", "Info"); return; }

            if (statusBayar == "Gagal" || statusBayar == "Dikembalikan")
            { MessageBox.Show($"Status sudah '{statusBayar}'.", "Info"); return; }

            if (MessageBox.Show(
                $"Tolak pembayaran pesanan #{idPesanan}?\nStatus pesanan kembali ke 'Dibatalkan'.",
                "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("sp_TolakPembayaranAdmin", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_pesanan", idPesanan);

                    SqlParameter pHasil = new SqlParameter("@hasil", SqlDbType.Int)
                    { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(pHasil);

                    cmd.ExecuteNonQuery();
                    int hasil = Convert.ToInt32(pHasil.Value);

                    if (hasil == 1)
                    {
                        MessageBox.Show(
                            $"Pembayaran pesanan #{idPesanan} ditolak.\nPesanan kembali ke 'Dibatalkan'.",
                            "Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPembayaran();
                    }
                    else
                    {
                        MessageBox.Show("Gagal menolak pembayaran. Coba lagi.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal tolak: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ════════════════════════════════════════════════════════
        // ✅ BtnInputResi_Click — pakai sp_InputResiPesanan
        // ════════════════════════════════════════════════════════
        private void BtnInputResi_Click(object sender, EventArgs e)
        {
            if (dgvPembayaran.SelectedRows.Count == 0)
            { MessageBox.Show("Pilih data pesanan terlebih dahulu!"); return; }

            string statusBayar = dgvPembayaran.SelectedRows[0].Cells["Status Bayar"].Value.ToString();
            if (statusBayar != "Lunas")
            {
                MessageBox.Show("Hanya pesanan LUNAS yang bisa diinput resi!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idPesanan = Convert.ToInt32(dgvPembayaran.SelectedRows[0].Cells["ID Pesanan"].Value);
            string resiLama = dgvPembayaran.SelectedRows[0].Cells["No Resi"].Value?.ToString();
            if (resiLama == "-") resiLama = "";

            // Dialog input resi
            Form dialog = new Form()
            {
                Text = "Input Nomor Resi",
                Size = new Size(420, 210),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = cCard
            };

            Label lbl = new Label()
            {
                Text = $"Nomor Resi Pesanan #{idPesanan}:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = cTextPri,
                BackColor = Color.Transparent
            };

            TextBox txtR = new TextBox()
            {
                Location = new Point(20, 55),
                Size = new Size(360, 28),
                Font = new Font("Segoe UI", 11),
                Text = resiLama,
                BackColor = cInputBg,
                ForeColor = cTextPri,
                BorderStyle = BorderStyle.FixedSingle
            };

            Button btnOk = new Button()
            {
                Text = "SIMPAN",
                Location = new Point(20, 110),
                Size = new Size(120, 38),
                BackColor = Color.FromArgb(200, 130, 30),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                DialogResult = DialogResult.OK,
                Cursor = Cursors.Hand
            };
            btnOk.FlatAppearance.BorderSize = 0;

            Button btnBtl = new Button()
            {
                Text = "Batal",
                Location = new Point(155, 110),
                Size = new Size(100, 38),
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel,
                BackColor = Color.FromArgb(20, 50, 100),
                ForeColor = cTextPri,
                Cursor = Cursors.Hand
            };
            btnBtl.FlatAppearance.BorderSize = 1;
            btnBtl.FlatAppearance.BorderColor = cInputBrd;

            ApplyRoundRegion(btnOk, 8);
            ApplyRoundRegion(btnBtl, 8);

            dialog.Controls.AddRange(new Control[] { lbl, txtR, btnOk, btnBtl });
            dialog.AcceptButton = btnOk;
            dialog.CancelButton = btnBtl;

            if (dialog.ShowDialog() != DialogResult.OK) return;

            string resi = txtR.Text.Trim();
            if (string.IsNullOrEmpty(resi))
            { MessageBox.Show("Nomor resi tidak boleh kosong!"); return; }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("sp_InputResiPesanan", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_pesanan", idPesanan);
                    cmd.Parameters.AddWithValue("@nomor_resi", resi);

                    SqlParameter pHasil = new SqlParameter("@hasil", SqlDbType.Int)
                    { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(pHasil);

                    cmd.ExecuteNonQuery();
                    int hasil = Convert.ToInt32(pHasil.Value);

                    if (hasil == 1)
                    {
                        MessageBox.Show(
                            $"Nomor resi disimpan!\nNo Resi: {resi}\nStatus: Dikirim",
                            "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPembayaran();
                    }
                    else
                    {
                        MessageBox.Show("Gagal simpan resi. Pesanan tidak ditemukan.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal simpan resi: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}