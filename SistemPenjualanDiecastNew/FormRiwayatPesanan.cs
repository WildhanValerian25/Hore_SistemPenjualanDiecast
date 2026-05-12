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

        private void DgvPesanan_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPesanan.SelectedRows.Count == 0)
            {
                btnBayar.Enabled = btnLihatResi.Enabled = btnPesananDiterima.Enabled = false;
                return;
            }

            string statusPesanan = dgvPesanan.SelectedRows[0].Cells["Status Pesanan"].Value?.ToString();
            string statusBayar = dgvPesanan.SelectedRows[0].Cells["Status Bayar"].Value?.ToString();
            string noResi = dgvPesanan.SelectedRows[0].Cells["No Resi"].Value?.ToString();

            btnBayar.Enabled = statusPesanan == "Pending" && statusBayar == "Menunggu";
            btnLihatResi.Enabled = (statusPesanan == "Dikirim" || statusPesanan == "Selesai") && noResi != "-";
            btnPesananDiterima.Enabled = statusPesanan == "Dikirim";
        }

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
                    SqlTransaction trans = null;
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction();

                        new SqlCommand("UPDATE PESANAN SET status_pesanan='Selesai' WHERE id_pesanan=@id",
                            conn, trans)
                        { Parameters = { new SqlParameter("@id", idPesanan) } }.ExecuteNonQuery();

                        new SqlCommand("UPDATE PEMBAYARAN SET status_bayar='Lunas', tanggal_bayar=GETDATE() WHERE id_pesanan=@id",
                            conn, trans)
                        { Parameters = { new SqlParameter("@id", idPesanan) } }.ExecuteNonQuery();

                        trans.Commit();
                        MessageBox.Show("Pesanan dikonfirmasi selesai! Terima kasih.",
                            "Selesai", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPesanan();
                    }
                    catch (Exception ex)
                    {
                        trans?.Rollback();
                        MessageBox.Show("Gagal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnLihatResi_Click(object sender, EventArgs e)
        {
            if (dgvPesanan.SelectedRows.Count == 0) return;

            string noResi = dgvPesanan.SelectedRows[0].Cells["No Resi"].Value?.ToString();
            int idPesanan = Convert.ToInt32(dgvPesanan.SelectedRows[0].Cells["ID Pesanan"].Value);
            string produk = dgvPesanan.SelectedRows[0].Cells["Produk"].Value?.ToString();

            if (string.IsNullOrEmpty(noResi) || noResi == "-")
            {
                MessageBox.Show("Nomor resi belum tersedia.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Form popup = new Form()
            {
                Text = "Informasi Pengiriman",
                Size = new Size(400, 270),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                BackColor = Color.White
            };

            Panel hdr = new Panel() { Dock = DockStyle.Top, Height = 45, BackColor = Color.DarkOrange };
            hdr.Controls.Add(new Label() { Text = "Info Pengiriman", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.White, Location = new Point(15, 10), AutoSize = true });

            popup.Controls.AddRange(new Control[] {
                hdr,
                new Label() { Text = $"ID Pesanan : #{idPesanan}", Font = new Font("Segoe UI",10), Location = new Point(20,55), AutoSize=true },
                new Label() { Text = $"Produk     : {produk}",     Font = new Font("Segoe UI",10), Location = new Point(20,80), AutoSize=true },
                new Label() { Text = "Nomor Resi :",               Font = new Font("Segoe UI",9), ForeColor=Color.Gray, Location = new Point(20,110), AutoSize=true },
                new Label() { Text = noResi, Font = new Font("Segoe UI",15,FontStyle.Bold), ForeColor=Color.DarkOrange, Location = new Point(20,130), AutoSize=true },
            });

            Button btnSalin = new Button() { Text = "Salin Resi", Location = new Point(20, 185), Size = new Size(130, 35), BackColor = Color.DarkOrange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnSalin.Click += (s, ev) => { Clipboard.SetText(noResi); MessageBox.Show("Nomor resi disalin!"); };
            Button btnTutup = new Button() { Text = "Tutup", Location = new Point(160, 185), Size = new Size(90, 35), FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.OK };

            popup.Controls.AddRange(new Control[] { btnSalin, btnTutup });
            popup.ShowDialog();
        }

        private decimal GetTotalHarga(int idPesanan)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT total_harga FROM PESANAN WHERE id_pesanan=@id", conn);
                    cmd.Parameters.AddWithValue("@id", idPesanan);
                    object r = cmd.ExecuteScalar();
                    return r != null ? Convert.ToDecimal(r) : 0;
                }
                catch { return 0; }
            }
        }
    }
}