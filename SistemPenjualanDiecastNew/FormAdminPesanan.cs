using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public class FormAdminPesanan : Form
    {
        DataGridView dgvPesanan;
        Button btnKonfirmasi, btnTolak, btnRefresh;
        Label lblTitle;
        ComboBox cmbFilterStatus;
        Label lblFilter;
        BindingNavigator navigator;

        private BindingSource _bindingSource = new BindingSource();

        string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

        public FormAdminPesanan()
        {
            InitializeForm();
            BuildUI();
            LoadPesanan();
        }

        private void InitializeForm()
        {
            this.Text = "Manajemen Pesanan";
            this.Size = new Size(950, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
        }

        private void BuildUI()
        {
            lblTitle = new Label()
            {
                Text = "DATA PESANAN MASUK",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(30, 20),
                AutoSize = true
            };

            lblFilter = new Label()
            {
                Text = "Filter Status:",
                Location = new Point(30, 60),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            cmbFilterStatus = new ComboBox()
            {
                Location = new Point(120, 57),
                Size = new Size(160, 22),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilterStatus.Items.AddRange(new string[] {
                "Semua", "Pending", "Dikonfirmasi", "Diproses",
                "Dikirim", "Selesai", "Dibatalkan"
            });
            cmbFilterStatus.SelectedIndex = 0;
            cmbFilterStatus.SelectedIndexChanged += (s, e) => LoadPesanan();

            navigator = new BindingNavigator(true)
            {
                Location = new Point(30, 85),
                Size = new Size(880, 25),
                BindingSource = _bindingSource
            };

            dgvPesanan = new DataGridView()
            {
                Location = new Point(30, 115),
                Size = new Size(880, 350),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = _bindingSource
            };
            dgvPesanan.DataBindingComplete += DgvPesanan_DataBindingComplete;

            btnKonfirmasi = new Button()
            {
                Text = "KONFIRMASI & ISI RESI",
                Location = new Point(30, 485),
                Size = new Size(190, 40),
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnKonfirmasi.Click += BtnKonfirmasi_Click;

            btnTolak = new Button()
            {
                Text = "TOLAK PEMBAYARAN",
                Location = new Point(235, 485),
                Size = new Size(180, 40),
                BackColor = Color.Crimson,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnTolak.Click += BtnTolak_Click;

            btnRefresh = new Button()
            {
                Text = "REFRESH",
                Location = new Point(430, 485),
                Size = new Size(100, 40),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnRefresh.Click += (s, e) => LoadPesanan();

            this.Controls.AddRange(new Control[] {
                lblTitle, lblFilter, cmbFilterStatus,
                navigator, dgvPesanan,
                btnKonfirmasi, btnTolak, btnRefresh
            });
        }

        private void DgvPesanan_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvPesanan.Rows)
            {
                string status = row.Cells["Status"]?.Value?.ToString();
                switch (status)
                {
                    case "Pending":
                        row.DefaultCellStyle.BackColor = Color.LightYellow; break;
                    case "Dikonfirmasi":
                    case "Diproses":
                        row.DefaultCellStyle.BackColor = Color.LightBlue; break;
                    case "Dikirim":
                        row.DefaultCellStyle.BackColor = Color.LightGreen; break;
                    case "Selesai":
                        row.DefaultCellStyle.BackColor = Color.PaleGreen; break;
                    case "Dibatalkan":
                        row.DefaultCellStyle.BackColor = Color.LightCoral; break;
                }
            }
        }

        // ✅ LoadPesanan — pakai sp_GetPesananAdmin
        private void LoadPesanan()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string filterStatus = cmbFilterStatus.SelectedItem.ToString();

                    SqlCommand cmd = new SqlCommand("sp_GetPesananAdmin", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@status",
                        filterStatus == "Semua" ? (object)DBNull.Value : filterStatus);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    _bindingSource.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat pesanan: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnKonfirmasi_Click(object sender, EventArgs e)
        {
            if (dgvPesanan.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih pesanan terlebih dahulu!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idPesanan = Convert.ToInt32(dgvPesanan.SelectedRows[0].Cells["ID Pesanan"].Value);
            string statusSaat = dgvPesanan.SelectedRows[0].Cells["Status"].Value.ToString();
            string namaPembeli = dgvPesanan.SelectedRows[0].Cells["Nama Pembeli"].Value.ToString();
            string namaProduk = dgvPesanan.SelectedRows[0].Cells["Produk"].Value.ToString();

            if (statusSaat == "Selesai" || statusSaat == "Dibatalkan")
            {
                MessageBox.Show($"Pesanan sudah berstatus '{statusSaat}', tidak bisa diubah lagi.",
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            FormKonfirmasiResi formResi = new FormKonfirmasiResi(
                idPesanan, statusSaat, namaPembeli, namaProduk);

            if (formResi.ShowDialog() == DialogResult.OK)
                LoadPesanan();
        }

        // ✅ BtnTolak_Click — pakai sp_TolakPesanan
        private void BtnTolak_Click(object sender, EventArgs e)
        {
            if (dgvPesanan.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih pesanan terlebih dahulu!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idPesanan = Convert.ToInt32(dgvPesanan.SelectedRows[0].Cells["ID Pesanan"].Value);
            string statusSaat = dgvPesanan.SelectedRows[0].Cells["Status"].Value.ToString();
            string namaPembeli = dgvPesanan.SelectedRows[0].Cells["Nama Pembeli"].Value.ToString();
            string namaProduk = dgvPesanan.SelectedRows[0].Cells["Produk"].Value.ToString();

            // Guard: status yang tidak bisa ditolak
            if (statusSaat == "Selesai" || statusSaat == "Dibatalkan")
            {
                MessageBox.Show($"Pesanan sudah berstatus '{statusSaat}', tidak bisa ditolak.",
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (statusSaat == "Dikirim" || statusSaat == "Diproses")
            {
                MessageBox.Show($"Pesanan sudah '{statusSaat}', tidak bisa ditolak!",
                    "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string pesanKonfirmasi =
                $"Yakin ingin MENOLAK pesanan ini?\n\n" +
                $"Pembeli : {namaPembeli}\n" +
                $"Produk  : {namaProduk}\n" +
                $"Status  : {statusSaat}\n\n" +
                $"Pesanan akan dibatalkan dan stok dikembalikan.";

            if (MessageBox.Show(pesanKonfirmasi, "Konfirmasi Penolakan",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("sp_TolakPesanan", conn);
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
                            $"Pesanan #{idPesanan} berhasil DITOLAK.\n\n" +
                            $"Status sebelum    : {statusSaat}\n" +
                            $"Status sekarang   : Dibatalkan\n" +
                            $"Status pembayaran : Gagal\n" +
                            $"Stok produk sudah dikembalikan.",
                            "Penolakan Berhasil",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadPesanan();
                    }
                    else
                    {
                        MessageBox.Show("Gagal menolak pesanan. Coba lagi.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menolak pembayaran: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}