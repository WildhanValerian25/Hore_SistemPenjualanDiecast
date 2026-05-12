using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public class FormRiwayatPesanan : Form
    {
        Label lblTitle, lblInfo;
        DataGridView dgvPesanan;
        Button btnBayar, btnRefresh, btnLihatResi, btnPesananDiterima;
        ComboBox cmbFilter;
        Label lblFilter, lblInfoData;

        // ✅ BindingSource
        private BindingSource _bindingSource = new BindingSource();

        private string _username;
        string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

        public FormRiwayatPesanan(string username)
        {
            _username = username;

            this.Text = "Riwayat Pesanan Saya";
            this.Size = new Size(900, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            BuildUI();
            LoadPesanan();
        }

        private void BuildUI()
        {
            lblTitle = new Label()
            {
                Text = "Riwayat Pesanan Saya",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Location = new Point(20, 15),
                AutoSize = true
            };

            lblFilter = new Label()
            {
                Text = "Filter Status:",
                Location = new Point(20, 58),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            cmbFilter = new ComboBox()
            {
                Location = new Point(110, 55),
                Size = new Size(180, 22),
                DropDownStyle = ComboBoxStyle.DropDownList
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
                ForeColor = Color.DimGray,
                Location = new Point(20, 85),
                AutoSize = true
            };

            // ✅ BindingNavigator
            BindingNavigator navigator = new BindingNavigator(true)
            {
                BindingSource = _bindingSource,
                Location = new Point(20, 108),
                Size = new Size(840, 25),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White
            };
            foreach (ToolStripItem item in navigator.Items)
                item.ForeColor = Color.White;

            // ✅ DataGridView dihubungkan ke BindingSource
            dgvPesanan = new DataGridView()
            {
                Location = new Point(20, 138),
                Size = new Size(840, 330),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = _bindingSource   // ✅ Binding
            };
            dgvPesanan.CellFormatting += DgvPesanan_CellFormatting;
            dgvPesanan.SelectionChanged += DgvPesanan_SelectionChanged;

            lblInfoData = new Label()
            {
                Text = "",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                Location = new Point(20, 475),
                AutoSize = true
            };

            btnBayar = new Button()
            {
                Text = "BAYAR SEKARANG",
                Location = new Point(20, 498),
                Size = new Size(160, 42),
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnBayar.Click += BtnBayar_Click;

            btnLihatResi = new Button()
            {
                Text = "LIHAT NOMOR RESI",
                Location = new Point(190, 498),
                Size = new Size(160, 42),
                BackColor = Color.DarkOrange,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnLihatResi.Click += BtnLihatResi_Click;

            btnPesananDiterima = new Button()
            {
                Text = "PESANAN DITERIMA",
                Location = new Point(360, 498),
                Size = new Size(160, 42),
                BackColor = Color.FromArgb(0, 153, 76),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnPesananDiterima.Click += BtnPesananDiterima_Click;

            btnRefresh = new Button()
            {
                Text = "REFRESH",
                Location = new Point(530, 498),
                Size = new Size(110, 42),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnRefresh.Click += (s, e) => LoadPesanan();

            this.Controls.AddRange(new Control[] {
                lblTitle, lblFilter, cmbFilter, lblInfo,
                navigator, dgvPesanan, lblInfoData,
                btnBayar, btnLihatResi, btnPesananDiterima, btnRefresh
            });

            // ✅ Update info jumlah data
            _bindingSource.ListChanged += (s, e) =>
            {
                lblInfoData.Text = $"Total {_bindingSource.Count} pesanan ditemukan";
            };
        }

        private void LoadPesanan()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string filter = cmbFilter.SelectedItem.ToString();
                    string where = filter == "Semua" ? "" : "AND ps.status_pesanan = @status";

                    string query = $@"
                        SELECT
                            ps.id_pesanan                                        AS [ID Pesanan],
                            pr.nama_produk                                       AS [Produk],
                            dp.jumlah                                            AS [Jumlah],
                            'Rp ' + FORMAT(ps.total_harga,'N0','id-ID')          AS [Total],
                            pb.metode                                            AS [Metode Bayar],
                            pb.status_bayar                                      AS [Status Bayar],
                            ps.status_pesanan                                    AS [Status Pesanan],
                            ISNULL(ps.nomor_resi,'-')                            AS [No Resi],
                            FORMAT(ps.tanggal_pesan,'dd/MM/yyyy HH:mm')          AS [Tanggal Pesan]
                        FROM PESANAN ps
                        JOIN PELANGGAN      pl ON ps.id_pelanggan = pl.id_pelanggan
                        JOIN DETAIL_PESANAN dp ON ps.id_pesanan   = dp.id_pesanan
                        JOIN PRODUK         pr ON dp.id_produk    = pr.id_produk
                        LEFT JOIN PEMBAYARAN pb ON ps.id_pesanan  = pb.id_pesanan
                        WHERE pl.[username] = @username {where}
                        ORDER BY ps.tanggal_pesan DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", _username);
                    if (filter != "Semua")
                        cmd.Parameters.AddWithValue("@status", filter);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // ✅ Set ke BindingSource
                    _bindingSource.DataSource = dt;

                    btnBayar.Enabled = false;
                    btnLihatResi.Enabled = false;
                    btnPesananDiterima.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat pesanan: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DgvPesanan_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string status = dgvPesanan.Rows[e.RowIndex].Cells["Status Pesanan"]?.Value?.ToString();
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
            dgvPesanan.Rows[e.RowIndex].DefaultCellStyle.BackColor = warna;
        }

        