using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
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
            this.BackColor = Color.White;

            // HEADER
            header = new Panel() { Size = new Size(1000, 60), Dock = DockStyle.Top, BackColor = Color.DarkBlue };
            lblTitle = new Label() { Text = "ADMIN DASHBOARD", ForeColor = Color.White, Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
            lblRole = new Label() { ForeColor = Color.White, Location = new Point(800, 20), AutoSize = true };
            header.Controls.AddRange(new Control[] { lblTitle, lblRole });

            // SIDEBAR
            sidebar = new Panel() { Size = new Size(200, 600), Dock = DockStyle.Left, BackColor = Color.LightGray };

            btnDashboard = CreateButton("Dashboard", 30);
            btnProduk = CreateButton("Produk", 80);

            Button btnPembayaran = CreateButton("Pembayaran", 130);
            btnPembayaran.Click += (s, e) =>
            {
                FormAdminPembayaran f = new FormAdminPembayaran();
                f.ShowDialog();
                if (isDashboardActive) LoadDashboard();
            };

            btnUser = CreateButton("User", 180);
            btnLogout = CreateButton("Logout", 530);

            // Tombol Test Koneksi
            Button btnConnect = CreateButton("Test Koneksi", 230);
            btnConnect.BackColor = Color.LightGreen;
            btnConnect.Click += BtnConnect_Click;

            // Tombol Reset Data
            Button btnResetData = CreateButton("Reset Data", 280);
            btnResetData.BackColor = Color.LightYellow;
            btnResetData.Click += BtnResetData_Click;

            // Tombol Test SQL Injection
            Button btnTestInjection = CreateButton("Test Injection", 330);
            btnTestInjection.BackColor = Color.LightCoral;
            btnTestInjection.Click += BtnTestInjection_Click;

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
            content = new Panel() { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke };
            lblNama = new Label() { Text = "Nama Produk", Location = new Point(20, 10), AutoSize = true };
            lblHarga = new Label() { Text = "Harga", Location = new Point(180, 10), AutoSize = true };
            lblStok = new Label() { Text = "Stok", Location = new Point(310, 10), AutoSize = true };
            lblJenis = new Label() { Text = "Merek", Location = new Point(440, 10), AutoSize = true };

            txtNamaProduk = new TextBox() { Location = new Point(20, 30), Width = 150 };
            txtHarga = new TextBox() { Location = new Point(180, 30), Width = 120 };
            txtStok = new TextBox() { Location = new Point(310, 30), Width = 120 };
            txtJenisProduk = new TextBox() { Location = new Point(440, 30), Width = 120 };

            btnAdd = new Button() { Text = "Add", Location = new Point(580, 28), Width = 70 };
            btnUpdate = new Button() { Text = "Update", Location = new Point(660, 28), Width = 70 };
            btnDelete = new Button() { Text = "Delete", Location = new Point(740, 28), Width = 70 };

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;

            grid = new DataGridView()
            {
                Location = new Point(20, 80),
                Size = new Size(740, 420),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false
            };
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
                ForeColor = Color.DarkBlue,
                Location = new Point(20, 10),
                AutoSize = true
            };
            content.Controls.Add(lblJudul);

            Button btnRefreshDash = new Button()
            {
                Text = "Refresh",
                Location = new Point(650, 5),
                Size = new Size(90, 30),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnRefreshDash.Click += (s, e) => LoadDashboard();
            content.Controls.Add(btnRefreshDash);

            Label lblLastRefresh = new Label()
            {
                Text = "Terakhir diperbarui: " + DateTime.Now.ToString("HH:mm:ss"),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
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
                        ("Total Produk",    totalProduk.ToString(),     Color.DarkBlue,            Color.AliceBlue),
                        ("Pesanan Masuk",   pesananMasuk.ToString(),    Color.DarkOrange,          Color.FromArgb(255,245,230)),
                        ("Tunggu Konfirm",  menungguKonfirm.ToString(), Color.Purple,              Color.FromArgb(245,230,255)),
                        ("Pesanan Selesai", pesananSelesai.ToString(),  Color.DarkGreen,           Color.FromArgb(230,255,230)),
                        ("Belum Bayar",     belumBayar.ToString(),      Color.FromArgb(180,100,0), Color.FromArgb(255,248,220)),
                        ("Pembatalan",      pesananBatal.ToString(),    Color.DarkRed,             Color.FromArgb(255,230,230)),
                    };

                    int x = 20;
                    foreach (var (judul, nilai, warnaTeks, warnaBg) in cards)
                    {
                        Panel card = new Panel() { Size = new Size(145, 90), Location = new Point(x, 45), BackColor = warnaBg, BorderStyle = BorderStyle.FixedSingle };
                        Label lNilai = new Label() { Text = nilai, Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = warnaTeks, Location = new Point(10, 8), AutoSize = true };
                        Label lJudul = new Label() { Text = judul, Font = new Font("Segoe UI", 8), ForeColor = Color.Gray, Location = new Point(10, 58), AutoSize = true };
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
                ForeColor = Color.DarkBlue,
                Location = new Point(20, 150),
                AutoSize = true
            };
            content.Controls.Add(lblPesanan);

            DataGridView gridPesanan = new DataGridView()
            {
                Location = new Point(20, 175),
                Size = new Size(730, 155),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.FixedSingle
            };

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
                    case "Pending": warna = Color.FromArgb(255, 243, 205); break;
                    case "Dikonfirmasi":
                    case "Diproses": warna = Color.FromArgb(207, 226, 255); break;
                    case "Dikirim": warna = Color.FromArgb(255, 235, 180); break;
                    case "Selesai": warna = Color.FromArgb(212, 237, 218); break;
                    case "Dibatalkan": warna = Color.FromArgb(255, 220, 220); break;
                    default: warna = Color.White; break;
                }
                gridPesanan.Rows[e.RowIndex].DefaultCellStyle.BackColor = warna;
            };
            content.Controls.Add(gridPesanan);

            // TABEL PEMBAYARAN MENUNGGU
            Label lblBayar = new Label()
            {
                Text = "Pembayaran Menunggu Konfirmasi",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.Purple,
                Location = new Point(20, 345),
                AutoSize = true
            };
            content.Controls.Add(lblBayar);

            DataGridView gridBayar = new DataGridView()
            {
                Location = new Point(20, 368),
                Size = new Size(730, 155),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.FixedSingle
            };

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
                    case "Menunggu": warna = Color.FromArgb(255, 243, 205); break;
                    case "Lunas": warna = Color.FromArgb(212, 237, 218); break;
                    case "Gagal": warna = Color.FromArgb(255, 220, 220); break;
                    default: warna = Color.White; break;
                }
                gridBayar.Rows[e.RowIndex].DefaultCellStyle.BackColor = warna;
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
        // =============================================
        private void BtnResetData_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Reset semua data PRODUK ke kondisi awal?\nData yang ditambahkan akan hilang.",
                "Konfirmasi Reset",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string query = @"
                IF OBJECT_ID('dbo.PRODUK_Backup') IS NOT NULL
                BEGIN
                    -- ✅ STEP 1: Hapus child table dulu (yang punya FK ke PRODUK)
                    -- Simpan sementara jika ada backup detail pesanan
                    -- Hapus DETAIL_PESANAN yang id_produk-nya ada di PRODUK
                    DELETE FROM dbo.DETAIL_PESANAN
                    WHERE id_produk IN (SELECT id_produk FROM dbo.PRODUK);

                    -- ✅ STEP 2: Baru hapus PRODUK (sudah tidak ada FK yang menggantung)
                    DELETE FROM dbo.PRODUK;

                    -- ✅ STEP 3: Aktifkan IDENTITY_INSERT lalu restore dari backup
                    SET IDENTITY_INSERT dbo.PRODUK ON;

                    INSERT INTO dbo.PRODUK
                        (id_produk, id_admin, nama_produk, merek, harga, stok, is_aktif, created_at)
                    SELECT
                        id_produk, id_admin, nama_produk, merek, harga, stok, is_aktif, created_at
                    FROM dbo.PRODUK_Backup;

                    SET IDENTITY_INSERT dbo.PRODUK OFF;

                    -- ✅ STEP 4: Reseed identity counter
                    DECLARE @maxId INT = (SELECT ISNULL(MAX(id_produk), 0) FROM dbo.PRODUK);
                    DBCC CHECKIDENT ('dbo.PRODUK', RESEED, @maxId);
                END
                ELSE
                BEGIN
                    RAISERROR('Tabel PRODUK_Backup tidak ditemukan!', 16, 1);
                END";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil direset!", "Berhasil",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (isDashboardActive) LoadDashboard();
                else LoadProduk();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reset gagal: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

            lblNama = new Label() { Text = "Nama Produk", Location = new Point(20, 10), AutoSize = true };
            lblHarga = new Label() { Text = "Harga", Location = new Point(180, 10), AutoSize = true };
            lblStok = new Label() { Text = "Stok", Location = new Point(310, 10), AutoSize = true };
            lblJenis = new Label() { Text = "Merek", Location = new Point(440, 10), AutoSize = true };

            txtNamaProduk = new TextBox() { Location = new Point(20, 30), Width = 150 };
            txtHarga = new TextBox() { Location = new Point(180, 30), Width = 120 };
            txtStok = new TextBox() { Location = new Point(310, 30), Width = 120 };
            txtJenisProduk = new TextBox() { Location = new Point(440, 30), Width = 120 };

            btnAdd = new Button() { Text = "Add", Location = new Point(580, 28), Width = 70 };
            btnUpdate = new Button() { Text = "Update", Location = new Point(660, 28), Width = 70 };
            btnDelete = new Button() { Text = "Delete", Location = new Point(740, 28), Width = 70 };

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;

            // Search Box
            Label lblSearch = new Label() { Text = "Search:", Location = new Point(20, 58), AutoSize = true };
            TextBox txtSearch = new TextBox() { Location = new Point(70, 55), Width = 200 };
            Button btnSearch = new Button()
            {
                Text = "Cari",
                Location = new Point(280, 53),
                Width = 70,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            // Label Total Produk
            Label lblTotalProduk = new Label()
            {
                Text = "Total Produk: 0",
                Location = new Point(530, 60),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            // Binding Navigator
            BindingNavigator navigator = new BindingNavigator(true)
            {
                Location = new Point(20, 78),
                Size = new Size(740, 25)
            };

            // GRID dibuat DULU sebelum subscribe event
            grid = new DataGridView()
            {
                Location = new Point(20, 108),
                Size = new Size(740, 390),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                RowHeadersVisible = false
            };
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

            btnAdd = new Button() { Text = "Add", Location = new Point(20, 28), Width = 70 };
            btnUpdate = new Button() { Text = "Update", Location = new Point(100, 28), Width = 70 };
            btnDelete = new Button() { Text = "Delete", Location = new Point(180, 28), Width = 70 };

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
                ForeColor = Color.DarkBlue
            };

            // GRID dibuat DULU sebelum subscribe event
            grid = new DataGridView()
            {
                Location = new Point(20, 80),
                Size = new Size(740, 420),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false
            };
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

        private Button CreateButton(string text, int top)
        {
            return new Button()
            {
                Text = text,
                Width = 180,
                Height = 40,
                Location = new Point(10, top),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };
        }
    }
}