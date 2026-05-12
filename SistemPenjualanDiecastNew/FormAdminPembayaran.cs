using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public class FormAdminPembayaran : Form
    {
        DataGridView dgvPembayaran;
        Button btnKonfirmasiLunas, btnTolak, btnRefresh, btnLihatBukti, btnInputResi;
        Label lblTitle;
        ComboBox cmbFilter;
        Label lblFilter;
        PictureBox picBukti;
        Panel panelBukti;
        Label lblBuktiTitle;

        // ✅ BindingSource & BindingNavigator
        private BindingSource _bindingSource = new BindingSource();
        private BindingNavigator _navigator;

        string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

        public FormAdminPembayaran()
        {
            this.Text = "Konfirmasi Pembayaran";
            this.Size = new Size(1100, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9);

            BuildUI();
            LoadPembayaran();
        }

        private void BuildUI()
        {
            lblTitle = new Label()
            {
                Text = "MANAJEMEN PEMBAYARAN",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true,
                ForeColor = Color.DarkBlue
            };

            lblFilter = new Label()
            {
                Text = "Filter:",
                Location = new Point(20, 58),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            cmbFilter = new ComboBox()
            {
                Location = new Point(65, 55),
                Size = new Size(160, 22),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilter.Items.AddRange(new string[] {
                "Semua", "Menunggu", "Lunas", "Gagal", "Dikembalikan"
            });
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += (s, e) => LoadPembayaran();

            // ✅ BindingNavigator
            _navigator = new BindingNavigator(true)
            {
                BindingSource = _bindingSource,
                Location = new Point(20, 82),
                Size = new Size(650, 25),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White
            };
            // Warnai tombol navigator
            foreach (ToolStripItem item in _navigator.Items)
                item.ForeColor = Color.White;

            // ✅ DataGridView dihubungkan ke BindingSource
            dgvPembayaran = new DataGridView()
            {
                Location = new Point(20, 112),
                Size = new Size(650, 400),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9),
                DataSource = _bindingSource   // ✅ Binding
            };
            dgvPembayaran.SelectionChanged += DgvPembayaran_SelectionChanged;
            dgvPembayaran.CellFormatting += DgvPembayaran_CellFormatting;

            // Panel preview bukti
            panelBukti = new Panel()
            {
                Location = new Point(690, 82),
                Size = new Size(385, 430),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            lblBuktiTitle = new Label()
            {
                Text = "Bukti Pembayaran",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true,
                ForeColor = Color.DarkBlue
            };

            picBukti = new PictureBox()
            {
                Location = new Point(10, 40),
                Size = new Size(363, 340),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblNoBukti = new Label()
            {
                Text = "Pilih baris untuk\nmelihat bukti pembayaran",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(90, 140),
                AutoSize = true
            };
            picBukti.Controls.Add(lblNoBukti);

            btnLihatBukti = new Button()
            {
                Text = "Lihat Bukti Full",
                Location = new Point(10, 390),
                Size = new Size(363, 35),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnLihatBukti.Click += BtnLihatBukti_Click;

            panelBukti.Controls.AddRange(new Control[] { lblBuktiTitle, picBukti, btnLihatBukti });

            // TOMBOL AKSI
            btnKonfirmasiLunas = new Button()
            {
                Text = "KONFIRMASI LUNAS",
                Location = new Point(20, 530),
                Size = new Size(155, 42),
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnKonfirmasiLunas.Click += BtnKonfirmasiLunas_Click;

            btnTolak = new Button()
            {
                Text = "TOLAK PEMBAYARAN",
                Location = new Point(185, 530),
                Size = new Size(155, 42),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnTolak.Click += BtnTolak_Click;

            btnInputResi = new Button()
            {
                Text = "INPUT NOMOR RESI",
                Location = new Point(350, 530),
                Size = new Size(155, 42),
                BackColor = Color.DarkOrange,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnInputResi.Click += BtnInputResi_Click;

            btnRefresh = new Button()
            {
                Text = "REFRESH",
                Location = new Point(515, 530),
                Size = new Size(110, 42),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnRefresh.Click += (s, e) => LoadPembayaran();

            // Label info jumlah data
            Label lblInfo = new Label()
            {
                Name = "lblInfoData",
                Text = "",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                Location = new Point(20, 588),
                AutoSize = true
            };

            this.Controls.AddRange(new Control[] {
                lblTitle, lblFilter, cmbFilter,
                _navigator, dgvPembayaran, panelBukti,
                btnKonfirmasiLunas, btnTolak, btnInputResi, btnRefresh,
                lblInfo
            });

            // ✅ Update label info saat data berubah
            _bindingSource.ListChanged += (s, e) =>
            {
                lblInfo.Text = $"Total data: {_bindingSource.Count} baris";
            };
        }

        private void LoadPembayaran()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string filter = cmbFilter.SelectedItem.ToString();
                    string query;
                    SqlCommand cmd;

                    if (filter == "Semua")
                    {
                        query = @"SELECT 
                                    pb.id_pembayaran                                AS [ID Bayar],
                                    pb.id_pesanan                                   AS [ID Pesanan],
                                    pl.nama                                         AS [Nama Pembeli],
                                    pb.metode                                       AS [Metode],
                                    'Rp ' + FORMAT(pb.jumlah_bayar,'N0','id-ID')   AS [Jumlah],
                                    pb.status_bayar                                 AS [Status Bayar],
                                    ps.status_pesanan                               AS [Status Pesanan],
                                    ISNULL(ps.nomor_resi,'-')                       AS [No Resi],
                                    FORMAT(pb.tanggal_bayar,'dd/MM/yyyy HH:mm')     AS [Tgl Bayar],
                                    CASE WHEN pb.bukti_bayar IS NOT NULL 
                                         THEN 'Ada' ELSE 'Tidak Ada' END            AS [Bukti]
                                  FROM PEMBAYARAN pb
                                  JOIN PESANAN   ps ON pb.id_pesanan   = ps.id_pesanan
                                  JOIN PELANGGAN pl ON ps.id_pelanggan = pl.id_pelanggan
                                  ORDER BY pb.id_pembayaran DESC";
                        cmd = new SqlCommand(query, conn);
                    }
                    else
                    {
                        query = @"SELECT 
                                    pb.id_pembayaran                                AS [ID Bayar],
                                    pb.id_pesanan                                   AS [ID Pesanan],
                                    pl.nama                                         AS [Nama Pembeli],
                                    pb.metode                                       AS [Metode],
                                    'Rp ' + FORMAT(pb.jumlah_bayar,'N0','id-ID')   AS [Jumlah],
                                    pb.status_bayar                                 AS [Status Bayar],
                                    ps.status_pesanan                               AS [Status Pesanan],
                                    ISNULL(ps.nomor_resi,'-')                       AS [No Resi],
                                    FORMAT(pb.tanggal_bayar,'dd/MM/yyyy HH:mm')     AS [Tgl Bayar],
                                    CASE WHEN pb.bukti_bayar IS NOT NULL 
                                         THEN 'Ada' ELSE 'Tidak Ada' END            AS [Bukti]
                                  FROM PEMBAYARAN pb
                                  JOIN PESANAN   ps ON pb.id_pesanan   = ps.id_pesanan
                                  JOIN PELANGGAN pl ON ps.id_pelanggan = pl.id_pelanggan
                                  WHERE pb.status_bayar = @status
                                  ORDER BY pb.id_pembayaran DESC";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@status", filter);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // ✅ Set data ke BindingSource
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
                case "Menunggu": warna = Color.FromArgb(255, 243, 205); break;
                case "Lunas": warna = Color.FromArgb(212, 237, 218); break;
                case "Gagal": warna = Color.FromArgb(255, 220, 220); break;
                case "Dikembalikan": warna = Color.FromArgb(207, 226, 255); break;
                default: warna = Color.White; break;
            }
            dgvPembayaran.Rows[e.RowIndex].DefaultCellStyle.BackColor = warna;
        }

        private void DgvPembayaran_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPembayaran.SelectedRows.Count == 0) return;
            if (dgvPembayaran.SelectedRows[0].Cells["ID Bayar"].Value == null) return;
            int idPembayaran = Convert.ToInt32(dgvPembayaran.SelectedRows[0].Cells["ID Bayar"].Value);
            TampilkanBukti(idPembayaran);
        }

        private void TampilkanBukti(int idPembayaran)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string q = "SELECT bukti_bayar FROM PEMBAYARAN WHERE id_pembayaran = @id";
                    SqlCommand cmd = new SqlCommand(q, conn);
                    cmd.Parameters.AddWithValue("@id", idPembayaran);
                    object result = cmd.ExecuteScalar();

                    picBukti.Controls.Clear();

                    if (result != null && result != DBNull.Value)
                    {
                        byte[] imgBytes = (byte[])result;
                        using (var ms = new System.IO.MemoryStream(imgBytes))
                            picBukti.Image = System.Drawing.Image.FromStream(ms);

                        lblBuktiTitle.Text = "Bukti Pembayaran — Ada";
                        lblBuktiTitle.ForeColor = Color.DarkGreen;
                    }
                    else
                    {
                        picBukti.Image = null;
                        lblBuktiTitle.Text = "Bukti Pembayaran — Tidak Ada";
                        lblBuktiTitle.ForeColor = Color.DarkRed;

                        picBukti.Controls.Add(new Label()
                        {
                            Text = "Tidak ada bukti pembayaran",
                            ForeColor = Color.Gray,
                            Font = new Font("Segoe UI", 9),
                            TextAlign = ContentAlignment.MiddleCenter,
                            Location = new Point(80, 140),
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
            Form f = new Form() { Text = "Bukti Pembayaran", Size = new Size(700, 600), StartPosition = FormStartPosition.CenterScreen };
            f.Controls.Add(new PictureBox() { Dock = DockStyle.Fill, Image = picBukti.Image, SizeMode = PictureBoxSizeMode.Zoom });
            f.Show();
        }

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
                "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlTransaction trans = null;
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction();

                        new SqlCommand(
                            "UPDATE PEMBAYARAN SET status_bayar='Lunas', tanggal_bayar=GETDATE() WHERE id_pesanan=@id",
                            conn, trans)
                        { Parameters = { new SqlParameter("@id", idPesanan) } }.ExecuteNonQuery();

                        new SqlCommand(
                            "UPDATE PESANAN SET status_pesanan='Diproses' WHERE id_pesanan=@id",
                            conn, trans)
                        { Parameters = { new SqlParameter("@id", idPesanan) } }.ExecuteNonQuery();

                        trans.Commit();
                        MessageBox.Show($"Pesanan #{idPesanan} dikonfirmasi Lunas!\nStatus otomatis 'Diproses'.",
                            "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPembayaran();
                    }
                    catch (Exception ex)
                    {
                        trans?.Rollback();
                        MessageBox.Show("Gagal konfirmasi: " + ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

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

            if (MessageBox.Show($"Tolak pembayaran pesanan #{idPesanan}?\nStatus pesanan kembali ke 'Dibatalkan'.",
                "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlTransaction trans = null;
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction();

                        new SqlCommand(
                            "UPDATE PEMBAYARAN SET status_bayar='Gagal', bukti_bayar=NULL WHERE id_pesanan=@id",
                            conn, trans)
                        { Parameters = { new SqlParameter("@id", idPesanan) } }.ExecuteNonQuery();

                        new SqlCommand(
                            "UPDATE PESANAN SET status_pesanan='Dibatalkan' WHERE id_pesanan=@id",
                            conn, trans)
                        { Parameters = { new SqlParameter("@id", idPesanan) } }.ExecuteNonQuery();
                        // ✅ Tambahkan ini — restore stok berdasarkan detail pesanan
                        new SqlCommand(@"
                             UPDATE p SET p.stok = p.stok + dp.jumlah
                             FROM PRODUK p
                             JOIN DETAIL_PESANAN dp ON p.id_produk = dp.id_produk
                               WHERE dp.id_pesanan = @id",
                            conn, trans)
                        { Parameters = { new SqlParameter("@id", idPesanan) } }
                        .ExecuteNonQuery();

                        trans.Commit();
                        MessageBox.Show($"Pembayaran pesanan #{idPesanan} ditolak.\nPesanan kembali ke 'Dibatalkan'.",
                            "Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPembayaran();
                    }
                    catch (Exception ex)
                    {
                        trans?.Rollback();
                        MessageBox.Show("Gagal tolak: " + ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnInputResi_Click(object sender, EventArgs e)
        {
            if (dgvPembayaran.SelectedRows.Count == 0)
            { MessageBox.Show("Pilih data pesanan terlebih dahulu!"); return; }

            string statusBayar = dgvPembayaran.SelectedRows[0].Cells["Status Bayar"].Value.ToString();
            if (statusBayar != "Lunas")
            { MessageBox.Show("Hanya pesanan LUNAS yang bisa diinput resi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            int idPesanan = Convert.ToInt32(dgvPembayaran.SelectedRows[0].Cells["ID Pesanan"].Value);
            string resiLama = dgvPembayaran.SelectedRows[0].Cells["No Resi"].Value?.ToString();
            if (resiLama == "-") resiLama = "";

            Form dialog = new Form()
            {
                Text = "Input Nomor Resi",
                Size = new Size(420, 210),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            Label lbl = new Label() { Text = $"Nomor Resi Pesanan #{idPesanan}:", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true, ForeColor = Color.DarkBlue };
            TextBox txtR = new TextBox() { Location = new Point(20, 55), Size = new Size(360, 28), Font = new Font("Segoe UI", 11), Text = resiLama };
            Button btnOk = new Button() { Text = "SIMPAN", Location = new Point(20, 110), Size = new Size(120, 38), BackColor = Color.DarkOrange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), DialogResult = DialogResult.OK };
            Button btnBtl = new Button() { Text = "Batal", Location = new Point(155, 110), Size = new Size(100, 38), FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.Cancel };

            dialog.Controls.AddRange(new Control[] { lbl, txtR, btnOk, btnBtl });
            dialog.AcceptButton = btnOk;
            dialog.CancelButton = btnBtl;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string resi = txtR.Text.Trim();
                if (string.IsNullOrEmpty(resi))
                { MessageBox.Show("Nomor resi tidak boleh kosong!"); return; }

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        string sql = "UPDATE PESANAN SET nomor_resi=@resi, status_pesanan='Dikirim' WHERE id_pesanan=@id";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@resi", resi);
                        cmd.Parameters.AddWithValue("@id", idPesanan);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show($"Nomor resi disimpan!\nNo Resi: {resi}\nStatus: Dikirim",
                            "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPembayaran();
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
}