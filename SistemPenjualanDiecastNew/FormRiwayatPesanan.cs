using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public class FormRiwayatPesanan : Form
    {
        Label lblTitle, lblInfo;
        DataGridView dgvPesanan;
        Button btnBayar, btnRefresh, btnLihatResi, btnPesananDiterima, btnBatalkan; // ✅ tambah btnBatalkan
        ComboBox cmbFilter;
        Label lblFilter, lblInfoData;

        private BindingSource _bindingSource = new BindingSource();

        private string _username;
        string connStr = Koneksi.GetConnectionString();

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

        public FormRiwayatPesanan(string username)
        {
            _username = username;

            this.Text = "Riwayat Pesanan Saya";
            this.Size = new Size(900, 680);   // ✅ sedikit lebih tinggi untuk tombol baru
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = cBgDark;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 9.5F);
            this.Paint += Form_Paint;

            BuildUI();
            LoadPesanan();
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
            lblTitle = new Label()
            {
                Text = "Riwayat Pesanan Saya",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = cTextPri,
                BackColor = Color.Transparent,
                Location = new Point(20, 15),
                AutoSize = true
            };

            lblFilter = new Label()
            {
                Text = "Filter Status:",
                Location = new Point(20, 58),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.Transparent,
                ForeColor = cTextMut
            };

            cmbFilter = new ComboBox()
            {
                Location = new Point(110, 55),
                Size = new Size(180, 22),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                BackColor = cInputBg,
                ForeColor = cTextPri
            };
            cmbFilter.Items.AddRange(new string[] {
                "Semua", "Pending", "Dikonfirmasi", "Diproses", "Dikirim", "Selesai", "Dibatalkan"
            });
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += (s, e) => LoadPesanan();

            lblInfo = new Label()
            {
                Text = "Pilih pesanan berstatus 'Pending' lalu klik BAYAR SEKARANG.",
                Font = new Font("Segoe UI", 8),
                ForeColor = cTextMut,
                BackColor = Color.Transparent,
                Location = new Point(20, 85),
                AutoSize = true
            };

            BindingNavigator navigator = new BindingNavigator(true)
            {
                BindingSource = _bindingSource,
                Location = new Point(20, 108),
                Size = new Size(840, 25),
                BackColor = cCard,
                ForeColor = cTextPri
            };
            foreach (ToolStripItem item in navigator.Items)
                item.ForeColor = cTextPri;

            dgvPesanan = new DataGridView()
            {
                Location = new Point(20, 138),
                Size = new Size(840, 330),
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
                Font = new Font("Segoe UI", 9F),
                DataSource = _bindingSource
            };

            dgvPesanan.ColumnHeadersDefaultCellStyle.BackColor = cCard;
            dgvPesanan.ColumnHeadersDefaultCellStyle.ForeColor = cTextPri;
            dgvPesanan.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvPesanan.ColumnHeadersDefaultCellStyle.SelectionBackColor = cCard;
            dgvPesanan.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvPesanan.ColumnHeadersHeight = 34;

            dgvPesanan.DefaultCellStyle.BackColor = cInputBg;
            dgvPesanan.DefaultCellStyle.ForeColor = cTextPri;
            dgvPesanan.DefaultCellStyle.SelectionBackColor = cAccent;
            dgvPesanan.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvPesanan.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(13, 32, 64);
            dgvPesanan.AlternatingRowsDefaultCellStyle.ForeColor = cTextPri;
            dgvPesanan.AlternatingRowsDefaultCellStyle.SelectionBackColor = cAccent;
            dgvPesanan.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            dgvPesanan.RowTemplate.Height = 30;
            dgvPesanan.CellFormatting += DgvPesanan_CellFormatting;
            dgvPesanan.SelectionChanged += DgvPesanan_SelectionChanged;

            lblInfoData = new Label()
            {
                Text = "",
                Font = new Font("Segoe UI", 8),
                ForeColor = cTextMut,
                BackColor = Color.Transparent,
                Location = new Point(20, 475),
                AutoSize = true
            };

            // ── Baris tombol 1 (y=498) ──────────────────────────────
            btnBayar = CreateActionButton("BAYAR SEKARANG", 20,
                Color.FromArgb(35, 130, 90), 498);
            btnBayar.Enabled = false;
            btnBayar.Click += BtnBayar_Click;

            btnLihatResi = CreateActionButton("LIHAT NOMOR RESI", 190,
                Color.FromArgb(200, 130, 30), 498);
            btnLihatResi.Enabled = false;
            btnLihatResi.Click += BtnLihatResi_Click;

            btnPesananDiterima = CreateActionButton("PESANAN DITERIMA", 360,
                Color.FromArgb(20, 140, 90), 498);
            btnPesananDiterima.Enabled = false;
            btnPesananDiterima.Click += BtnPesananDiterima_Click;

            btnRefresh = new Button()
            {
                Text = "REFRESH",
                Location = new Point(530, 498),
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
            btnRefresh.Click += (s, e) => LoadPesanan();

            // ── Baris tombol 2 (y=548): tombol BATALKAN ✅ ──────────
            btnBatalkan = CreateActionButton("BATALKAN PESANAN", 20,
                Color.FromArgb(180, 40, 40), 548);
            btnBatalkan.Enabled = false;
            btnBatalkan.Click += BtnBatalkan_Click;

            this.Controls.AddRange(new Control[] {
                lblTitle, lblFilter, cmbFilter, lblInfo,
                navigator, dgvPesanan, lblInfoData,
                btnBayar, btnLihatResi, btnPesananDiterima, btnRefresh,
                btnBatalkan   // ✅
            });

            _bindingSource.ListChanged += (s, e) =>
            {
                lblInfoData.Text = $"Total {_bindingSource.Count} pesanan ditemukan";
            };
        }

        // ════════════════════════════════════════════════════════
        // HELPERS VISUAL
        // ════════════════════════════════════════════════════════
        private Button CreateActionButton(string text, int x, Color baseColor, int y = 498)
        {
            Button b = new Button()
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(160, 42),
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
            c.Region = new Region(RoundedPath(new Rectangle(0, 0, c.Width, c.Height), radius));
        }

        // ════════════════════════════════════════════════════════
        // LOAD DATA via SP
        // ════════════════════════════════════════════════════════
        private void LoadPesanan()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string filter = cmbFilter.SelectedItem.ToString();

                    using (SqlCommand cmd = new SqlCommand("sp_GetRiwayatPesanan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@username", _username);
                        cmd.Parameters.AddWithValue("@status",
                            filter == "Semua" ? (object)DBNull.Value : filter);

                        DataTable dt = new DataTable();
                        new SqlDataAdapter(cmd).Fill(dt);
                        _bindingSource.DataSource = dt;
                    }

                    btnBayar.Enabled = false;
                    btnLihatResi.Enabled = false;
                    btnPesananDiterima.Enabled = false;
                    btnBatalkan.Enabled = false;  // ✅ reset
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat pesanan: " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ════════════════════════════════════════════════════════
        // SELECTION CHANGED — aktifkan/nonaktifkan tombol
        // ════════════════════════════════════════════════════════
        private void DgvPesanan_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPesanan.SelectedRows.Count == 0)
            {
                btnBayar.Enabled = false;
                btnLihatResi.Enabled = false;
                btnPesananDiterima.Enabled = false;
                btnBatalkan.Enabled = false;
                return;
            }

            string statusPesanan = dgvPesanan.SelectedRows[0].Cells["Status Pesanan"].Value?.ToString();
            string statusBayar = dgvPesanan.SelectedRows[0].Cells["Status Bayar"].Value?.ToString();
            string noResi = dgvPesanan.SelectedRows[0].Cells["No Resi"].Value?.ToString();

            btnBayar.Enabled = statusPesanan == "Pending" && statusBayar == "Menunggu";
            btnLihatResi.Enabled = (statusPesanan == "Dikirim" || statusPesanan == "Selesai") && noResi != "-";
            btnPesananDiterima.Enabled = statusPesanan == "Dikirim";

            // ✅ Batalkan hanya aktif saat Pending atau Dikonfirmasi
            btnBatalkan.Enabled = statusPesanan == "Pending" || statusPesanan == "Dikonfirmasi";
        }

        // ════════════════════════════════════════════════════════
        // TOMBOL: BATALKAN PESANAN ✅
        // ════════════════════════════════════════════════════════
        private void BtnBatalkan_Click(object sender, EventArgs e)
        {
            if (dgvPesanan.SelectedRows.Count == 0) return;

            int idPesanan = Convert.ToInt32(dgvPesanan.SelectedRows[0].Cells["ID Pesanan"].Value);
            string produk = dgvPesanan.SelectedRows[0].Cells["Produk"].Value?.ToString();
            string status = dgvPesanan.SelectedRows[0].Cells["Status Pesanan"].Value?.ToString();

            // Konfirmasi ke user
            DialogResult konfirmasi = MessageBox.Show(
                $"Yakin ingin membatalkan pesanan ini?\n\n" +
                $"ID Pesanan : #{idPesanan}\n" +
                $"Produk     : {produk}\n" +
                $"Status     : {status}\n\n" +
                $"Stok akan dikembalikan otomatis.",
                "Konfirmasi Batalkan Pesanan",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (konfirmasi != DialogResult.Yes) return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_BatalkanPesanan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_pesanan", idPesanan);
                        cmd.Parameters.AddWithValue("@username", _username);

                        SqlParameter pHasil = new SqlParameter("@hasil", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(pHasil);

                        cmd.ExecuteNonQuery();

                        int hasil = Convert.ToInt32(pHasil.Value);

                        switch (hasil)
                        {
                            case 1:
                                MessageBox.Show(
                                    "Pesanan berhasil dibatalkan. Stok telah dikembalikan.",
                                    "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadPesanan();
                                break;

                            case 0:
                                MessageBox.Show(
                                    "Pesanan tidak ditemukan atau bukan milik Anda.",
                                    "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;

                            case -1:
                                MessageBox.Show(
                                    $"Pesanan dengan status '{status}' tidak dapat dibatalkan.\n" +
                                    "Hanya pesanan berstatus Pending atau Dikonfirmasi yang bisa dibatalkan.",
                                    "Tidak Bisa Dibatalkan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;

                            default:
                                MessageBox.Show(
                                    "Terjadi kesalahan saat membatalkan pesanan.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ════════════════════════════════════════════════════════
        // TOMBOL: BAYAR SEKARANG
        // ════════════════════════════════════════════════════════
        private void BtnBayar_Click(object sender, EventArgs e)
        {
            if (dgvPesanan.SelectedRows.Count == 0) return;

            int idPesanan = Convert.ToInt32(dgvPesanan.SelectedRows[0].Cells["ID Pesanan"].Value);
            string metode = dgvPesanan.SelectedRows[0].Cells["Metode Bayar"].Value?.ToString() ?? "Transfer Bank";
            decimal total = GetTotalHarga(idPesanan);

            FormPembayaran formBayar = new FormPembayaran(idPesanan, total, metode);
            if (formBayar.ShowDialog() == DialogResult.OK)
            {
                LoadPesanan();
                MessageBox.Show("Bukti pembayaran berhasil dikirim! Admin akan segera mengkonfirmasi.",
                    "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ════════════════════════════════════════════════════════
        // TOMBOL: PESANAN DITERIMA
        // ════════════════════════════════════════════════════════
        private void BtnPesananDiterima_Click(object sender, EventArgs e)
        {
            if (dgvPesanan.SelectedRows.Count == 0) return;

            int idPesanan = Convert.ToInt32(dgvPesanan.SelectedRows[0].Cells["ID Pesanan"].Value);
            string produk = dgvPesanan.SelectedRows[0].Cells["Produk"].Value?.ToString();
            string noResi = dgvPesanan.SelectedRows[0].Cells["No Resi"].Value?.ToString();

            if (MessageBox.Show(
                $"Konfirmasi pesanan diterima?\n\nID  : #{idPesanan}\nProduk : {produk}\nResi   : {noResi}",
                "Konfirmasi Diterima", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("sp_KonfirmasiPesananDiterima", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@id_pesanan", idPesanan);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Pesanan dikonfirmasi selesai! Terima kasih.",
                            "Selesai", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPesanan();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal: " + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // ════════════════════════════════════════════════════════
        // TOMBOL: LIHAT NOMOR RESI
        // ════════════════════════════════════════════════════════
        private void BtnLihatResi_Click(object sender, EventArgs e)
        {
            if (dgvPesanan.SelectedRows.Count == 0) return;

            string noResi = dgvPesanan.SelectedRows[0].Cells["No Resi"].Value?.ToString();
            int idPesanan = Convert.ToInt32(dgvPesanan.SelectedRows[0].Cells["ID Pesanan"].Value);
            string produk = dgvPesanan.SelectedRows[0].Cells["Produk"].Value?.ToString();

            if (string.IsNullOrEmpty(noResi) || noResi == "-")
            {
                MessageBox.Show("Nomor resi belum tersedia.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Form popup = new Form()
            {
                Text = "Informasi Pengiriman",
                Size = new Size(400, 270),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                BackColor = cCard
            };

            Panel hdr = new Panel() { Dock = DockStyle.Top, Height = 45, BackColor = cAccent };
            hdr.Controls.Add(new Label()
            {
                Text = "Info Pengiriman",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(15, 10),
                AutoSize = true
            });

            popup.Controls.AddRange(new Control[] {
                hdr,
                new Label() { Text = $"ID Pesanan : #{idPesanan}", Font = new Font("Segoe UI",10), ForeColor = cTextPri, BackColor = Color.Transparent, Location = new Point(20,55),  AutoSize=true },
                new Label() { Text = $"Produk     : {produk}",     Font = new Font("Segoe UI",10), ForeColor = cTextPri, BackColor = Color.Transparent, Location = new Point(20,80),  AutoSize=true },
                new Label() { Text = "Nomor Resi :",               Font = new Font("Segoe UI", 9), ForeColor = cTextMut, BackColor = Color.Transparent, Location = new Point(20,110), AutoSize=true },
                new Label() { Text = noResi, Font = new Font("Segoe UI",15,FontStyle.Bold), ForeColor = Color.FromArgb(255,190,110), BackColor = Color.Transparent, Location = new Point(20,130), AutoSize=true },
            });

            Button btnSalin = new Button()
            {
                Text = "Salin Resi",
                Location = new Point(20, 185),
                Size = new Size(130, 35),
                BackColor = Color.FromArgb(200, 130, 30),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSalin.FlatAppearance.BorderSize = 0;
            ApplyRoundRegion(btnSalin, 8);
            btnSalin.Click += (s, ev) =>
            {
                Clipboard.SetText(noResi);
                MessageBox.Show("Nomor resi disalin!");
            };

            Button btnTutup = new Button()
            {
                Text = "Tutup",
                Location = new Point(160, 185),
                Size = new Size(90, 35),
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(20, 50, 100),
                ForeColor = cTextPri,
                Cursor = Cursors.Hand
            };
            btnTutup.FlatAppearance.BorderSize = 1;
            btnTutup.FlatAppearance.BorderColor = cInputBrd;
            ApplyRoundRegion(btnTutup, 8);

            popup.Controls.AddRange(new Control[] { btnSalin, btnTutup });
            popup.ShowDialog();
        }

        // ════════════════════════════════════════════════════════
        // HELPER: CELL FORMATTING (warna baris per status)
        // ════════════════════════════════════════════════════════
        private void DgvPesanan_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string status = dgvPesanan.Rows[e.RowIndex].Cells["Status Pesanan"]?.Value?.ToString();
            Color warna;
            switch (status)
            {
                case "Pending": warna = Color.FromArgb(70, 60, 25); break;
                case "Dikonfirmasi":
                case "Diproses": warna = Color.FromArgb(25, 45, 80); break;
                case "Dikirim": warna = Color.FromArgb(70, 55, 15); break;
                case "Selesai": warna = Color.FromArgb(20, 55, 35); break;
                case "Dibatalkan": warna = Color.FromArgb(70, 25, 30); break;
                default: warna = cInputBg; break;
            }
            dgvPesanan.Rows[e.RowIndex].DefaultCellStyle.BackColor = warna;
            dgvPesanan.Rows[e.RowIndex].DefaultCellStyle.ForeColor = cTextPri;
        }

        // ════════════════════════════════════════════════════════
        // HELPER: AMBIL TOTAL HARGA via SP
        // ════════════════════════════════════════════════════════
        private decimal GetTotalHarga(int idPesanan)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_GetTotalHargaPesanan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_pesanan", idPesanan);
                        object r = cmd.ExecuteScalar();
                        return r != null ? Convert.ToDecimal(r) : 0;
                    }
                }
                catch { return 0; }
            }
        }
    }
}