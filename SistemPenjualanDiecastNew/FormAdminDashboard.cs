using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public partial class FormAdminDashboard : Form
    {
        Panel header, sidebar, content;
        Label lblTitle, lblRole;
        Label lblNama, lblHarga, lblStok, lblJenis;
        Button btnDashboard, btnProduk, btnUser, btnLogout;
        Button btnAdd, btnUpdate, btnDelete;
        TextBox txtNamaProduk, txtHarga, txtStok, txtJenisProduk;
        DataGridView grid;

        bool isDashboardActive = false;
        string userRole;
        string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

        // ── PALET WARNA (disamakan dengan FormLogin / FormRegister) ──
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
        private readonly Color cSidebar = Color.FromArgb(11, 26, 53);
        private readonly Color cSidebarBtn = Color.FromArgb(18, 44, 88);
        private readonly Color cSidebarBtnHover = Color.FromArgb(26, 86, 219);
        private readonly Color cHeader = Color.FromArgb(11, 30, 62);

        public FormAdminDashboard(string role)
        {
            userRole = role;
            InitializeComponent();
            if (lblRole != null) lblRole.Text = "ROLE: " + userRole.ToUpper();

            try
            {
                SetupAccess();
                LoadDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Admin Dashboard";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = cBgDark;
            this.Font = new Font("Segoe UI", 9.5F);
            this.Paint += Form_Paint;

            // HEADER
            header = new Panel() { Size = new Size(1000, 60), Dock = DockStyle.Top, BackColor = cHeader };
            header.Paint += (s, e) =>
            {
                using (Pen p = new Pen(cCardBord, 1))
                    e.Graphics.DrawLine(p, 0, header.Height - 1, header.Width, header.Height - 1);
            };

            lblTitle = new Label()
            {
                Text = "ADMIN DASHBOARD",
                ForeColor = cTextPri,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 15),
                BackColor = Color.Transparent,
                AutoSize = true
            };
            lblRole = new Label()
            {
                ForeColor = cAccentHover,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(800, 22),
                BackColor = Color.Transparent,
                AutoSize = true
            };
            header.Controls.AddRange(new Control[] { lblTitle, lblRole });

            // SIDEBAR
            sidebar = new Panel() { Size = new Size(200, 600), Dock = DockStyle.Left, BackColor = cSidebar };

            btnDashboard = CreateSidebarButton("Dashboard", 20);
            btnProduk = CreateSidebarButton("Produk", 68);

            Button btnPembayaran = CreateSidebarButton("Pembayaran", 116);
            btnPembayaran.Click += (s, e) =>
            {
                FormAdminPembayaran f = new FormAdminPembayaran();
                f.ShowDialog();
                if (isDashboardActive) LoadDashboard();
            };

            btnUser = CreateSidebarButton("User", 164);
            // Button btnLaporan
            Button btnLaporan = CreateSidebarButton("Laporan", 404);
            btnLaporan.Click += (s, e) =>
            {
                FormLaporanPenjualan f = new FormLaporanPenjualan();
                f.ShowDialog();
            };
            sidebar.Controls.Add(btnLaporan);
            // Tombol Test Koneksi
            Button btnConnect = CreateSidebarButton("Test Koneksi", 212);
            btnConnect.Click += BtnConnect_Click;

            // Tombol Reset Data
            Button btnResetData = CreateSidebarButton("Reset Data", 260);
            btnResetData.Click += BtnResetData_Click;
            // Di dalam InitializeComponent(), di blok sidebar setelah btnResetData:

            Button btnImportExcel = CreateSidebarButton("Import Excel", 356);
            btnImportExcel.Click += (s, e) =>
            {
                // Ambil id_admin dari username yang login
                int idAdmin = GetIdAdmin();
                FormImportExcel f = new FormImportExcel(idAdmin);
                f.ShowDialog();
                if (isDashboardActive) LoadDashboard();
                else LoadProduk();
            };

            Button btnExportExcel = CreateSidebarButton("Export Excel", 452);
            btnExportExcel.Click += (s, e) =>
            {
                FormExportExcel f = new FormExportExcel();
                f.ShowDialog();
            };
            sidebar.Controls.Add(btnExportExcel);
            sidebar.Controls.Add(btnImportExcel);
            // Tombol Test SQL Injection
            Button btnTestInjection = CreateSidebarButton("Test Injection", 308);
            btnTestInjection.Click += BtnTestInjection_Click;

            btnLogout = CreateSidebarButton("Logout", 530);
            btnLogout.BackColor = Color.FromArgb(120, 30, 40);

            btnDashboard.Click += (s, e) => { isDashboardActive = true; LoadDashboard(); };
            btnProduk.Click += (s, e) => { isDashboardActive = false; RebuildContentArea(); LoadProduk(); };
            btnUser.Click += (s, e) => { isDashboardActive = false; RebuildContentAreaUser(); BtnUser_Click(s, e); };
            btnLogout.Click += BtnLogout_Click;

            sidebar.Controls.AddRange(new Control[] {
                btnDashboard, btnProduk, btnPembayaran, btnUser,
                btnConnect, btnResetData, btnTestInjection,
                btnLogout
            });

            // CONTENT
            content = new Panel() { Dock = DockStyle.Fill, BackColor = cBgMid };
            content.Paint += Content_PaintBackground;

            lblNama = CreateFieldLabel("Nama Produk", 20);
            lblHarga = CreateFieldLabel("Harga", 180);
            lblStok = CreateFieldLabel("Stok", 310);
            lblJenis = CreateFieldLabel("Merek", 440);

            txtNamaProduk = CreateDarkTextBox(20, 150);
            txtHarga = CreateDarkTextBox(180, 120);
            txtStok = CreateDarkTextBox(310, 120);
            txtJenisProduk = CreateDarkTextBox(440, 120);

            btnAdd = CreateActionButton("Add", 580, cAccent);
            btnUpdate = CreateActionButton("Update", 660, Color.FromArgb(40, 130, 90));
            btnDelete = CreateActionButton("Delete", 740, Color.FromArgb(150, 45, 55));

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;

            grid = CreateDarkGrid(20, 80, 740, 420);
            grid.CellClick += Grid_CellClick;

            content.Controls.AddRange(new Control[] {
                lblNama, lblHarga, lblStok, lblJenis,
                txtNamaProduk, txtHarga, txtStok, txtJenisProduk,
                btnAdd, btnUpdate, btnDelete, grid
            });

            this.Controls.Add(content);
            this.Controls.Add(sidebar);
            this.Controls.Add(header);
        }

        // =============================================
        // BACKGROUND & VISUAL HELPERS
        // =============================================
        private void Form_Paint(object sender, PaintEventArgs e)
        {
            // Tidak menggambar full gradient di sini karena header/sidebar/content
            // sudah Dock-filled menutupi client area; cukup warnai pinggiran jika ada.
        }

        private void Content_PaintBackground(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush br = new LinearGradientBrush(
                content.ClientRectangle, cBgDark, cBgMid, LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(br, content.ClientRectangle);
            }
        }

        private Button CreateSidebarButton(string text, int top)
        {
            Button b = new Button()
            {
                Text = text,
                Width = 176,
                Height = 38,
                Location = new Point(12, top),
                FlatStyle = FlatStyle.Flat,
                BackColor = cSidebarBtn,
                ForeColor = cTextPri,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(14, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = cSidebarBtnHover;
            b.FlatAppearance.MouseDownBackColor = cAccentDown;
            ApplyRoundRegion(b, 8);
            b.Resize += (s, e) => ApplyRoundRegion(b, 8);
            return b;
        }

        private Label CreateFieldLabel(string text, int x)
        {
            return new Label()
            {
                Text = text,
                Location = new Point(x, 10),
                AutoSize = true,
                BackColor = Color.Transparent,
                ForeColor = cTextMut,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold)
            };
        }

        private TextBox CreateDarkTextBox(int x, int width)
        {
            TextBox t = new TextBox()
            {
                Location = new Point(x, 30),
                Width = width,
                BackColor = cInputBg,
                ForeColor = cTextPri,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5F)
            };
            return t;
        }

        private Button CreateActionButton(string text, int x, Color baseColor)
        {
            Button b = new Button()
            {
                Text = text,
                Location = new Point(x, 28),
                Width = 70,
                Height = 28,
                BackColor = baseColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            ApplyRoundRegion(b, 6);
            return b;
        }

        private DataGridView CreateDarkGrid(int x, int y, int w, int h)
        {
            DataGridView g = new DataGridView()
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = cInputBg,
                GridColor = cCardBord,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };

            g.ColumnHeadersDefaultCellStyle.BackColor = cCard;
            g.ColumnHeadersDefaultCellStyle.ForeColor = cTextPri;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = cCard;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 34;

            g.DefaultCellStyle.BackColor = cInputBg;
            g.DefaultCellStyle.ForeColor = cTextPri;
            g.DefaultCellStyle.SelectionBackColor = cAccent;
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.DefaultCellStyle.Font = new Font("Segoe UI", 9F);

            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(13, 32, 64);
            g.AlternatingRowsDefaultCellStyle.ForeColor = cTextPri;
            g.AlternatingRowsDefaultCellStyle.SelectionBackColor = cAccent;
            g.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            g.RowTemplate.Height = 30;

            return g;
        }

        private static void ApplyRoundRegion(Control c, int radius)
        {
            if (c.Width <= 0 || c.Height <= 0) return;
            Rectangle r = new Rectangle(0, 0, c.Width, c.Height);
            c.Region = new Region(RoundedPath(r, radius));
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

        // Panel "card" rounded dark, dipakai untuk statistik & wrapper grid di dashboard
        private Panel CreateCardPanel(int x, int y, int w, int h, Color bg)
        {
            Panel p = new Panel() { Location = new Point(x, y), Size = new Size(w, h), BackColor = Color.Transparent };
            p.Paint += (s, e) =>
            {
                Graphics gfx = e.Graphics;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, p.Width - 1, p.Height - 1);
                using (GraphicsPath path = RoundedPath(rect, 10))
                {
                    using (SolidBrush fillBr = new SolidBrush(bg))
                        gfx.FillPath(fillBr, path);
                    using (Pen pen = new Pen(cCardBord, 1f))
                        gfx.DrawPath(pen, path);
                    p.Region = new Region(path);
                }
            };
            return p;
        }

        // =============================================
        // LOAD PRODUK — Pakai SP + BindingSource
        // =============================================
        private void LoadProduk(string keyword = "")
        {
            if (grid == null || grid.IsDisposed) return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("sp_SearchProduk", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@keyword", keyword);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    BindingSource bs = new BindingSource();
                    bs.DataSource = dt;
                    grid.DataSource = bs;

                    if (grid.Columns.Contains("id_produk")) grid.Columns["id_produk"].Visible = false;
                    if (grid.Columns.Contains("is_aktif")) grid.Columns["is_aktif"].Visible = false;
                    if (grid.Columns.Contains("created_at")) grid.Columns["created_at"].Visible = false;
                    if (grid.Columns.Contains("nama_produk")) grid.Columns["nama_produk"].HeaderText = "Nama Produk";
                    if (grid.Columns.Contains("merek")) grid.Columns["merek"].HeaderText = "Merek";
                    if (grid.Columns.Contains("stok")) grid.Columns["stok"].HeaderText = "Stok";
                    if (grid.Columns.Contains("harga"))
                    {
                        grid.Columns["harga"].HeaderText = "Harga";
                        grid.Columns["harga"].DefaultCellStyle.Format = "N0";
                        grid.Columns["harga"].DefaultCellStyle.FormatProvider =
                            new System.Globalization.CultureInfo("id-ID");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat data produk: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =============================================
        // LOAD DASHBOARD
        // =============================================
        private void LoadDashboard()
        {
            isDashboardActive = true;
            content.Controls.Clear();

            Label lblJudul = new Label()
            {
                Text = "Ringkasan Informasi",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = cTextPri,
                BackColor = Color.Transparent,
                Location = new Point(20, 10),
                AutoSize = true
            };
            content.Controls.Add(lblJudul);

            Button btnRefreshDash = new Button()
            {
                Text = "Refresh",
                Location = new Point(650, 5),
                Size = new Size(90, 30),
                BackColor = cAccent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefreshDash.FlatAppearance.BorderSize = 0;
            btnRefreshDash.FlatAppearance.MouseOverBackColor = cAccentHover;
            ApplyRoundRegion(btnRefreshDash, 6);
            btnRefreshDash.Click += (s, e) => LoadDashboard();
            content.Controls.Add(btnRefreshDash);

            Label lblLastRefresh = new Label()
            {
                Text = "Terakhir diperbarui: " + DateTime.Now.ToString("HH:mm:ss"),
                Font = new Font("Segoe UI", 8),
                ForeColor = cTextMut,
                BackColor = Color.Transparent,
                Location = new Point(20, 540),
                AutoSize = true
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    int totalProduk = TryGetCount(conn, "SELECT COUNT(*) FROM PRODUK WHERE is_aktif = 1");
                    int pesananMasuk = TryGetCount(conn, "SELECT COUNT(*) FROM PESANAN WHERE status_pesanan = 'Pending'");
                    int pesananSelesai = TryGetCount(conn, "SELECT COUNT(*) FROM PESANAN WHERE status_pesanan = 'Selesai'");
                    int menungguKonfirm = TryGetCount(conn, "SELECT COUNT(*) FROM PEMBAYARAN WHERE status_bayar = 'Menunggu'");
                    int belumBayar = TryGetCount(conn, "SELECT COUNT(*) FROM PEMBAYARAN WHERE status_bayar = 'Menunggu'");
                    int pesananBatal = TryGetCount(conn, "SELECT COUNT(*) FROM PESANAN WHERE status_pesanan = 'Dibatalkan'");

                    var cards = new[]
                    {
                        ("Total Produk",    totalProduk.ToString(),     Color.FromArgb(120, 170, 255)),
                        ("Pesanan Masuk",   pesananMasuk.ToString(),    Color.FromArgb(255, 180, 110)),
                        ("Tunggu Konfirm",  menungguKonfirm.ToString(), Color.FromArgb(210, 150, 255)),
                        ("Pesanan Selesai", pesananSelesai.ToString(),  Color.FromArgb(120, 220, 150)),
                        ("Belum Bayar",     belumBayar.ToString(),      Color.FromArgb(255, 210, 110)),
                        ("Pembatalan",      pesananBatal.ToString(),    Color.FromArgb(255, 120, 130)),
                    };

                    int x = 20;
                    foreach (var (judul, nilai, warnaTeks) in cards)
                    {
                        Panel card = CreateCardPanel(x, 45, 145, 90, cCard);
                        Label lNilai = new Label()
                        {
                            Text = nilai,
                            Font = new Font("Segoe UI", 22, FontStyle.Bold),
                            ForeColor = warnaTeks,
                            BackColor = Color.Transparent,
                            Location = new Point(10, 8),
                            AutoSize = true
                        };
                        Label lJudul = new Label()
                        {
                            Text = judul,
                            Font = new Font("Segoe UI", 8),
                            ForeColor = cTextMut,
                            BackColor = Color.Transparent,
                            Location = new Point(10, 58),
                            AutoSize = true
                        };
                        card.Controls.AddRange(new Control[] { lNilai, lJudul });
                        content.Controls.Add(card);
                        x += 150;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load stat: " + ex.Message);
            }

            // TABEL PESANAN TERBARU
            Label lblPesanan = new Label()
            {
                Text = "Pesanan Terbaru",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = cTextPri,
                BackColor = Color.Transparent,
                Location = new Point(20, 150),
                AutoSize = true
            };
            content.Controls.Add(lblPesanan);

            DataGridView gridPesanan = CreateDarkGrid(20, 175, 730, 155);

            string sqlPesanan = @"
                SELECT TOP 10
                    ps.id_pesanan                                           AS [ID Pesanan],
                    pl.nama                                                 AS [Nama Pembeli],
                    FORMAT(ps.tanggal_pesan, 'dd/MM/yyyy HH:mm')           AS [Tanggal],
                    ps.status_pesanan                                       AS [Status],
                    'Rp ' + FORMAT(ps.total_harga, 'N0', 'id-ID')          AS [Total]
                FROM PESANAN ps
                JOIN PELANGGAN pl ON ps.id_pelanggan = pl.id_pelanggan
                ORDER BY ps.tanggal_pesan DESC";
            FillGridDashboard(gridPesanan, sqlPesanan);

            gridPesanan.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                string status = gridPesanan.Rows[e.RowIndex].Cells["Status"]?.Value?.ToString();
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
                gridPesanan.Rows[e.RowIndex].DefaultCellStyle.BackColor = warna;
                gridPesanan.Rows[e.RowIndex].DefaultCellStyle.ForeColor = cTextPri;
            };
            content.Controls.Add(gridPesanan);

            // TABEL PEMBAYARAN MENUNGGU
            Label lblBayar = new Label()
            {
                Text = "Pembayaran Menunggu Konfirmasi",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(210, 150, 255),
                BackColor = Color.Transparent,
                Location = new Point(20, 345),
                AutoSize = true
            };
            content.Controls.Add(lblBayar);

            DataGridView gridBayar = CreateDarkGrid(20, 368, 730, 155);

            string sqlBayar = @"
                SELECT TOP 10
                    pb.id_pembayaran                                        AS [ID Bayar],
                    pb.id_pesanan                                           AS [ID Pesanan],
                    pl.nama                                                 AS [Nama Pembeli],
                    pb.metode                                               AS [Metode],
                    'Rp ' + FORMAT(pb.jumlah_bayar, 'N0', 'id-ID')         AS [Jumlah],
                    pb.status_bayar                                         AS [Status Bayar],
                    ps.status_pesanan                                       AS [Status Pesanan],
                    FORMAT(pb.tanggal_bayar, 'dd/MM/yyyy HH:mm')           AS [Tgl Bayar]
                FROM PEMBAYARAN pb
                JOIN PESANAN   ps ON pb.id_pesanan   = ps.id_pesanan
                JOIN PELANGGAN pl ON ps.id_pelanggan = pl.id_pelanggan
                WHERE pb.status_bayar = 'Menunggu'
                ORDER BY pb.id_pembayaran DESC";
            FillGridDashboard(gridBayar, sqlBayar);

            gridBayar.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                string status = gridBayar.Rows[e.RowIndex].Cells["Status Bayar"]?.Value?.ToString();
                Color warna;
                switch (status)
                {
                    case "Menunggu": warna = Color.FromArgb(70, 60, 25); break;
                    case "Lunas": warna = Color.FromArgb(20, 55, 35); break;
                    case "Gagal": warna = Color.FromArgb(70, 25, 30); break;
                    default: warna = cInputBg; break;
                }
                gridBayar.Rows[e.RowIndex].DefaultCellStyle.BackColor = warna;
                gridBayar.Rows[e.RowIndex].DefaultCellStyle.ForeColor = cTextPri;
            };

            content.Controls.Add(gridBayar);
            content.Controls.Add(lblLastRefresh);

            HitungTotalProduk();
        }

        // =============================================
        // Hitung Total Produk (OUTPUT Parameter)
        // =============================================
        private void HitungTotalProduk()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CountProduk", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter outputParam = new SqlParameter("@Total", SqlDbType.Int);
                        outputParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outputParam);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        int total = Convert.ToInt32(outputParam.Value);
                        this.Text = $"Admin Dashboard — Total Produk Aktif: {total}";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menghitung total: " + ex.Message);
            }
        }

        // =============================================
        // Test Koneksi
        // =============================================
        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    MessageBox.Show("Koneksi berhasil!", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi gagal: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =============================================
        // ✅ PERBAIKAN UTAMA — Reset Data dari Backup
        // Sekarang memanggil stored procedure sp_ResetDataProduk
        // (lihat sp_ResetDataProduk_Fixed.sql) yang sudah menangani:
        //   - Semua FK anak (ULASAN, ITEM_KERANJANG, PEMBAYARAN,
        //     DETAIL_PESANAN, PESANAN) dihapus dengan urutan benar
        //   - TRUNCATE TABLE PRODUK (bukan DELETE) supaya tidak
        //     terhalang trigger trg_CegahHapusProdukAktif yang
        //     mengubah DELETE menjadi soft delete (is_aktif = 0)
        // =============================================
        private void BtnResetData_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Reset semua data PRODUK ke kondisi awal?\n" +
                "Semua data transaksi (Pesanan, Pembayaran, Ulasan, Keranjang) " +
                "juga akan ikut dikosongkan.\nTindakan ini tidak bisa dibatalkan.",
                "Konfirmasi Reset",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("sp_ResetDataProduk", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

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
                            MessageBox.Show("Data berhasil direset ke kondisi awal!", "Berhasil",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;

                        case 0:
                            MessageBox.Show(
                                "Reset gagal: tabel PRODUK_Backup tidak ditemukan.\n" +
                                "Pastikan backup awal sudah dibuat sebelum demo.",
                                "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;

                        default: // -1 atau nilai tak terduga lainnya
                            MessageBox.Show("Reset gagal karena terjadi error pada server.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Reset gagal: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (isDashboardActive) LoadDashboard();
            else LoadProduk();
        }

        // =============================================
        // Simulasi SQL Injection
        // =============================================
        private void BtnTestInjection_Click(object sender, EventArgs e)
        {
            string inputBerbahaya = "' OR 1=1 --";

            DialogResult konfirmasi = MessageBox.Show(
                $"SIMULASI SQL INJECTION\n\n" +
                $"Input berbahaya:\n{inputBerbahaya}\n\n" +
                $"Query TIDAK AMAN:\n" +
                $"UPDATE PRODUK SET nama_produk='HACKED' WHERE nama_produk='{inputBerbahaya}'\n\n" +
                $"Lanjutkan simulasi? (Data bisa direset setelahnya)",
                "Simulasi SQL Injection",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (konfirmasi != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // ❌ QUERY TIDAK AMAN — string concatenation (simulasi)
                    string queryTidakAman =
                        "UPDATE PRODUK SET nama_produk='HACKED' WHERE nama_produk='" +
                        inputBerbahaya + "'";

                    using (SqlCommand cmd = new SqlCommand(queryTidakAman, conn))
                    {
                        int result = cmd.ExecuteNonQuery();
                        MessageBox.Show(
                            $"{result} baris terupdate!\n\n" +
                            $"SQL Injection BERHASIL karena query tidak aman!\n\n" +
                            $"Query:\n{queryTidakAman}\n\n" +
                            $"Solusi: Gunakan Parameterized Query atau Stored Procedure.",
                            "Hasil SQL Injection",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (isDashboardActive) LoadDashboard();
                else LoadProduk();

                if (MessageBox.Show(
                    "Reset data kembali ke semula?",
                    "Reset Data",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    BtnResetData_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =============================================
        // REBUILD CONTENT AREA PRODUK
        // =============================================
        private void RebuildContentArea()
        {
            content.Controls.Clear();

            lblNama = CreateFieldLabel("Nama Produk", 20);
            lblHarga = CreateFieldLabel("Harga", 180);
            lblStok = CreateFieldLabel("Stok", 310);
            lblJenis = CreateFieldLabel("Merek", 440);

            txtNamaProduk = CreateDarkTextBox(20, 150);
            txtHarga = CreateDarkTextBox(180, 120);
            txtStok = CreateDarkTextBox(310, 120);
            txtJenisProduk = CreateDarkTextBox(440, 120);

            btnAdd = CreateActionButton("Add", 580, cAccent);
            btnUpdate = CreateActionButton("Update", 660, Color.FromArgb(40, 130, 90));
            btnDelete = CreateActionButton("Delete", 740, Color.FromArgb(150, 45, 55));

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;

            // Search Box
            Label lblSearch = new Label()
            {
                Text = "Search:",
                Location = new Point(20, 58),
                AutoSize = true,
                BackColor = Color.Transparent,
                ForeColor = cTextMut
            };
            TextBox txtSearch = CreateDarkTextBox(70, 200);
            txtSearch.Location = new Point(70, 55);

            Button btnSearch = new Button()
            {
                Text = "Cari",
                Location = new Point(280, 53),
                Width = 70,
                Height = 26,
                BackColor = cAccent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.FlatAppearance.MouseOverBackColor = cAccentHover;
            ApplyRoundRegion(btnSearch, 6);

            // Label Total Produk
            Label lblTotalProduk = new Label()
            {
                Text = "Total Produk: 0",
                Location = new Point(530, 60),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = cTextPri,
                BackColor = Color.Transparent
            };

            // Binding Navigator
            BindingNavigator navigator = new BindingNavigator(true)
            {
                Location = new Point(20, 78),
                Size = new Size(740, 25),
                BackColor = cCard
            };

            // GRID dibuat DULU sebelum subscribe event
            grid = CreateDarkGrid(20, 108, 740, 390);
            grid.CellClick += Grid_CellClick;

            // Subscribe event SETELAH grid dibuat
            grid.DataSourceChanged += (s, e) =>
            {
                if (grid.DataSource is BindingSource bs)
                {
                    navigator.BindingSource = bs;

                    if (bs.DataSource is DataTable dt)
                        lblTotalProduk.Text = "Total Produk: " + dt.Rows.Count;
                }
            };

            btnSearch.Click += (s, e) => LoadProduk(txtSearch.Text.Trim());
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) LoadProduk(txtSearch.Text.Trim()); };

            SetupAccess();

            content.Controls.AddRange(new Control[] {
                lblNama, lblHarga, lblStok, lblJenis,
                txtNamaProduk, txtHarga, txtStok, txtJenisProduk,
                btnAdd, btnUpdate, btnDelete,
                lblSearch, txtSearch, btnSearch,
                lblTotalProduk,
                navigator, grid
            });
        }

        // =============================================
        // REBUILD CONTENT AREA USER
        // =============================================
        private void RebuildContentAreaUser()
        {
            content.Controls.Clear();

            btnAdd = CreateActionButton("Add", 20, cAccent);
            btnAdd.Location = new Point(20, 28);
            btnUpdate = CreateActionButton("Update", 100, Color.FromArgb(40, 130, 90));
            btnUpdate.Location = new Point(100, 28);
            btnDelete = CreateActionButton("Delete", 180, Color.FromArgb(150, 45, 55));
            btnDelete.Location = new Point(180, 28);

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;

            // Label Total User
            Label lblTotalUser = new Label()
            {
                Text = "Total User: 0",
                Location = new Point(530, 35),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = cTextPri,
                BackColor = Color.Transparent
            };

            // GRID dibuat DULU sebelum subscribe event
            grid = CreateDarkGrid(20, 80, 740, 420);
            grid.CellClick += Grid_CellClick;

            // Subscribe event SETELAH grid dibuat
            grid.DataSourceChanged += (s, e) =>
            {
                if (grid.DataSource is DataTable dt)
                    lblTotalUser.Text = "Total User: " + dt.Rows.Count;
                else if (grid.DataSource is BindingSource bs && bs.DataSource is DataTable dtBs)
                    lblTotalUser.Text = "Total User: " + dtBs.Rows.Count;
            };

            SetupAccess();

            content.Controls.AddRange(new Control[] {
                btnAdd, btnUpdate, btnDelete,
                lblTotalUser,
                grid
            });
        }

        private void FillGridDashboard(DataGridView g, string sql)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    var da = new SqlDataAdapter(sql, conn);
                    var dt = new DataTable();
                    da.Fill(dt);
                    g.DataSource = dt;
                }
            }
            catch { g.DataSource = null; }
        }

        private int TryGetCount(SqlConnection conn, string sql)
        {
            try { return (int)new SqlCommand(sql, conn).ExecuteScalar(); }
            catch { return 0; }
        }

        // =============================================
        // ADD — Pakai SP
        // =============================================
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidasiInputProduk())
                return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmdAdmin = new SqlCommand(
                        "SELECT id_admin FROM ADMIN WHERE [username] = @u", conn);

                    cmdAdmin.Parameters.AddWithValue("@u", userRole);

                    object res = cmdAdmin.ExecuteScalar();

                    int idAdmin = res != null
                        ? Convert.ToInt32(res)
                        : 1;

                    SqlCommand cmd = new SqlCommand("sp_InsertProduk", conn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_admin", idAdmin);

                    cmd.Parameters.AddWithValue(
                        "@nama_produk",
                        txtNamaProduk.Text.Trim());

                    cmd.Parameters.AddWithValue(
                        "@merek",
                        txtJenisProduk.Text.Trim());

                    cmd.Parameters.AddWithValue(
                        "@harga",
                        decimal.Parse(
                            txtHarga.Text.Replace(".", "").Replace(",", "")));

                    cmd.Parameters.AddWithValue(
                        "@stok",
                        int.Parse(txtStok.Text.Trim()));

                    SqlParameter pResult =
                        new SqlParameter("@hasil", SqlDbType.Int);

                    pResult.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(pResult);

                    cmd.ExecuteNonQuery();

                    if (Convert.ToInt32(pResult.Value) == 1)
                    {
                        MessageBox.Show(
                            "Produk berhasil ditambahkan!",
                            "Berhasil",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        ClearForm();
                        LoadProduk();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Gagal menambahkan produk!",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Gagal tambah produk: " + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // =============================================
        // UPDATE — Pakai SP
        // =============================================
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih baris di tabel terlebih dahulu!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ Panggil validasi dulu
            if (!ValidasiInputProduk()) return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    int idProduk = 0;
                    if (grid.SelectedRows[0].DataBoundItem is DataRowView drv)
                        idProduk = Convert.ToInt32(drv.Row["id_produk"]);
                    else { MessageBox.Show("Gagal membaca ID produk!"); return; }


                    SqlCommand cmd = new SqlCommand("sp_UpdateProduk", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_produk", idProduk);
                    cmd.Parameters.AddWithValue("@nama_produk", txtNamaProduk.Text.Trim());
                    cmd.Parameters.AddWithValue("@merek", txtJenisProduk.Text.Trim());
                    cmd.Parameters.AddWithValue("@harga", decimal.Parse(txtHarga.Text.Replace(".", "").Replace(",", "")));
                    cmd.Parameters.AddWithValue("@stok", int.Parse(txtStok.Text.Trim()));

                    SqlParameter pResult = new SqlParameter("@hasil", SqlDbType.Int)
                    { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(pResult);
                    cmd.ExecuteNonQuery();

                    if (Convert.ToInt32(pResult.Value) == 1)
                    {
                        MessageBox.Show("Produk berhasil diupdate!", "Berhasil",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        LoadProduk();
                    }
                    else
                    {
                        MessageBox.Show("Gagal update produk!", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Update Error: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private int GetIdAdmin()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT id_admin FROM ADMIN WHERE [username] = @u", conn);
                    cmd.Parameters.AddWithValue("@u", userRole);
                    object res = cmd.ExecuteScalar();
                    return res != null ? Convert.ToInt32(res) : 1;
                }
                catch { return 1; }
            }
        }

        // =============================================
        // DELETE — Pakai SP
        // =============================================
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih baris di tabel terlebih dahulu!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idProduk = 0;
            string namaProduk = "";

            if (grid.SelectedRows[0].DataBoundItem is DataRowView drv)
            {
                idProduk = Convert.ToInt32(drv.Row["id_produk"]);
                namaProduk = drv.Row["nama_produk"].ToString();
            }
            else { MessageBox.Show("Gagal membaca data!"); return; }

            if (MessageBox.Show($"Hapus produk '{namaProduk}'?", "Konfirmasi",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("sp_DeleteProduk", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_produk", idProduk);

                        SqlParameter pResult = new SqlParameter("@hasil", SqlDbType.Int)
                        { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(pResult);
                        cmd.ExecuteNonQuery();

                        if (Convert.ToInt32(pResult.Value) == 1)
                        {
                            MessageBox.Show($"Produk '{namaProduk}' berhasil dihapus!", "Berhasil",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearForm();
                            LoadProduk();
                        }
                        else
                        {
                            MessageBox.Show("Gagal menghapus produk!", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        // =============================================
        // METHOD VALIDASI PRODUK
        // =============================================
        private bool ValidasiInputProduk()
        {
            // ✅ 1. Validasi semua field wajib diisi
            if (string.IsNullOrWhiteSpace(txtNamaProduk?.Text) ||
                string.IsNullOrWhiteSpace(txtHarga?.Text) ||
                string.IsNullOrWhiteSpace(txtStok?.Text) ||
                string.IsNullOrWhiteSpace(txtJenisProduk?.Text))
            {
                MessageBox.Show("Semua field wajib diisi!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // ✅ 2. Validasi Nama Produk harus mengandung huruf abjad
            bool namaMengandungHuruf = false;
            foreach (char c in txtNamaProduk.Text)
            {
                if (char.IsLetter(c)) { namaMengandungHuruf = true; break; }
            }
            if (!namaMengandungHuruf)
            {
                MessageBox.Show("Nama Produk harus mengandung huruf abjad!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNamaProduk.Focus();
                return false;
            }

            // ✅ 3. Validasi Merek harus mengandung huruf abjad
            bool merekMengandungHuruf = false;
            foreach (char c in txtJenisProduk.Text)
            {
                if (char.IsLetter(c)) { merekMengandungHuruf = true; break; }
            }
            if (!merekMengandungHuruf)
            {
                MessageBox.Show("Merek harus mengandung huruf abjad!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtJenisProduk.Focus();
                return false;
            }

            // ✅ 4. Validasi Harga harus angka dan tidak boleh 0 atau negatif
            if (!decimal.TryParse(
                    txtHarga.Text.Replace(".", "").Replace(",", "").Trim(),
                    out decimal harga))
            {
                MessageBox.Show("Harga harus berupa angka!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHarga.Focus();
                return false;
            }
            if (harga <= 0)
            {
                MessageBox.Show("Harga tidak boleh 0 atau negatif!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHarga.Focus();
                return false;
            }

            // ✅ 5. Validasi Stok harus angka dan tidak boleh 0 atau negatif
            if (!int.TryParse(txtStok.Text.Trim(), out int stok))
            {
                MessageBox.Show("Stok harus berupa angka bulat!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStok.Focus();
                return false;
            }
            if (stok <= 0)
            {
                MessageBox.Show("Stok tidak boleh 0 atau negatif!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStok.Focus();
                return false;
            }

            return true; // ✅ Semua validasi lolos
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = grid.Rows[e.RowIndex];

            if (grid.Columns.Contains("nama_produk"))
            {
                txtNamaProduk.Text = row.Cells["nama_produk"].Value?.ToString();
                txtJenisProduk.Text = row.Cells["merek"].Value?.ToString();
                txtStok.Text = row.Cells["stok"].Value?.ToString();
                string hargaRaw = row.Cells["harga"].Value?.ToString() ?? "0";
                txtHarga.Text = hargaRaw.Replace(".", "").Replace(",", "");
            }
            else
            {
                ClearForm();
            }
        }

        private void ClearForm()
        {
            txtNamaProduk?.Clear();
            txtHarga?.Clear();
            txtStok?.Clear();
            txtJenisProduk?.Clear();
        }

        private void BtnUser_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(
                        "SELECT [username], email, alamat, no_telepon AS [No HP] FROM PELANGGAN", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    grid.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal load user: " + ex.Message);
                }
            }
        }

        private void SetupAccess()
        {
            if (userRole.ToLower() != "wildhan" && userRole.ToLower() != "admin")
            {
                if (btnUser != null) btnUser.Enabled = false;
                if (btnAdd != null) btnAdd.Enabled = false;
                if (btnUpdate != null) btnUpdate.Enabled = false;
                if (btnDelete != null) btnDelete.Enabled = false;
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            Form loginForm = Application.OpenForms["Form1"];
            if (loginForm != null) loginForm.Show();
            else { Form1 f = new Form1(); f.Show(); }
            this.Close();
        }
    }
}