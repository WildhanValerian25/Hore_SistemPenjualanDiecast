using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public class FormAdminStok : Form
    {
        DataGridView dgvStok;
        Button btnTambahStok, btnRefresh, btnKurangiStok;
        Label lblTitle, lblInfoData;
        TextBox txtCari;
        Button btnCari;
        Label lblCari;

        // ✅ BindingSource & BindingNavigator
        private BindingSource _bindingSource = new BindingSource();
        private BindingNavigator _navigator;

        string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

        public FormAdminStok()
        {
            this.Text = "Manajemen Stok Produk";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            BuildUI();
            LoadStok();
        }

        private void BuildUI()
        {
            lblTitle = new Label()
            {
                Text = "DATA STOK PRODUK",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true,
                ForeColor = Color.DarkBlue
            };

            // Cari produk
            lblCari = new Label() { Text = "Cari:", Location = new Point(20, 58), AutoSize = true };
            txtCari = new TextBox() { Location = new Point(58, 55), Width = 200 };
            btnCari = new Button()
            {
                Text = "Cari",
                Location = new Point(268, 53),
                Width = 70,
                BackColor = Color.DarkBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCari.Click += BtnCari_Click;
            txtCari.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnCari_Click(s, e); };

            // ✅ BindingNavigator
            _navigator = new BindingNavigator(true)
            {
                BindingSource = _bindingSource,
                Location = new Point(20, 83),
                Size = new Size(740, 25),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White
            };
            foreach (ToolStripItem item in _navigator.Items)
                item.ForeColor = Color.White;

            // ✅ DataGridView dihubungkan ke BindingSource
            dgvStok = new DataGridView()
            {
                Location = new Point(20, 113),
                Size = new Size(740, 350),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = _bindingSource   // ✅ Binding
            };
            dgvStok.CellFormatting += DgvStok_CellFormatting;

            lblInfoData = new Label()
            {
                Text = "",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                Location = new Point(20, 470),
                AutoSize = true
            };

            btnTambahStok = new Button()
            {
                Text = "TAMBAH STOK",
                Location = new Point(20, 495),
                Size = new Size(140, 40),
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnTambahStok.Click += BtnTambahStok_Click;

            btnKurangiStok = new Button()
            {
                Text = "KURANGI STOK",
                Location = new Point(170, 495),
                Size = new Size(140, 40),
                BackColor = Color.DarkOrange,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnKurangiStok.Click += BtnKurangiStok_Click;

            btnRefresh = new Button()
            {
                Text = "REFRESH",
                Location = new Point(320, 495),
                Size = new Size(100, 40),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnRefresh.Click += (s, e) => { txtCari.Clear(); LoadStok(); };

            this.Controls.AddRange(new Control[] {
                lblTitle, lblCari, txtCari, btnCari,
                _navigator, dgvStok, lblInfoData,
                btnTambahStok, btnKurangiStok, btnRefresh
            });

            // ✅ Update info jumlah data
            _bindingSource.ListChanged += (s, e) =>
            {
                lblInfoData.Text = $"Menampilkan {_bindingSource.Count} produk";
            };
        }

        private void LoadStok(string keyword = "")
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                    id_produk                               AS [ID],
                    nama_produk                             AS [Nama Produk],
                    merek                                   AS [Merek],
                    'Rp ' + FORMAT(harga, 'N0', 'id-ID')   AS [Harga],
                    stok                                    AS [Stok],
                    status_stok                             AS [Status Stok]
                    FROM vw_StokProduk
                    ORDER BY stok ASC";

                    if (!string.IsNullOrEmpty(keyword))
                        query += " AND (nama_produk LIKE @kw OR merek LIKE @kw)";

                    query += " ORDER BY stok ASC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (!string.IsNullOrEmpty(keyword))
                        cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // ✅ Set ke BindingSource
                    _bindingSource.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat stok: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DgvStok_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string status = dgvStok.Rows[e.RowIndex].Cells["Status Stok"]?.Value?.ToString();
            if (status == "Habis") dgvStok.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
            else if (status == "Hampir Habis") dgvStok.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 205);
            else dgvStok.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(212, 237, 218);
        }

        private void BtnCari_Click(object sender, EventArgs e)
        {
            LoadStok(txtCari.Text.Trim());
        }

        private void BtnTambahStok_Click(object sender, EventArgs e)
        {
            UbahStok(true);
        }

        private void BtnKurangiStok_Click(object sender, EventArgs e)
        {
            UbahStok(false);
        }

        private void UbahStok(bool tambah)
        {
            if (dgvStok.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih produk terlebih dahulu!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idProduk = Convert.ToInt32(dgvStok.SelectedRows[0].Cells["ID"].Value);
            string namaProduk = dgvStok.SelectedRows[0].Cells["Nama Produk"].Value.ToString();
            int stokSaatIni = Convert.ToInt32(dgvStok.SelectedRows[0].Cells["Stok"].Value);
            string aksi = tambah ? "tambahkan" : "kurangi";

            // Dialog input jumlah
            Form dialog = new Form()
            {
                Text = tambah ? "Tambah Stok" : "Kurangi Stok",
                Size = new Size(380, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            Label lbl = new Label() { Text = $"Jumlah stok yang ingin di{aksi}:\n{namaProduk}\n(Stok saat ini: {stokSaatIni})", Font = new Font("Segoe UI", 9), Location = new Point(20, 15), AutoSize = true };
            TextBox txtJ = new TextBox() { Location = new Point(20, 85), Width = 200, Font = new Font("Segoe UI", 11) };
            Button btnOk = new Button() { Text = "OK", Location = new Point(20, 120), Size = new Size(100, 35), BackColor = tambah ? Color.DarkGreen : Color.DarkOrange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.OK };
            Button btnBtl = new Button() { Text = "Batal", Location = new Point(130, 120), Size = new Size(80, 35), FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.Cancel };

            dialog.Controls.AddRange(new Control[] { lbl, txtJ, btnOk, btnBtl });
            dialog.AcceptButton = btnOk;
            dialog.CancelButton = btnBtl;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (!int.TryParse(txtJ.Text, out int jumlah) || jumlah <= 0)
                {
                    MessageBox.Show("Masukkan angka yang valid!", "Peringatan",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!tambah && jumlah > stokSaatIni)
                {
                    MessageBox.Show($"Stok tidak cukup! Stok saat ini: {stokSaatIni}", "Peringatan",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        string sql = tambah
                            ? "UPDATE PRODUK SET stok = stok + @jml WHERE id_produk = @id"
                            : "UPDATE PRODUK SET stok = stok - @jml WHERE id_produk = @id";

                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@jml", jumlah);
                        cmd.Parameters.AddWithValue("@id", idProduk);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show(
                            $"Stok '{namaProduk}' berhasil {(tambah ? "ditambah" : "dikurangi")} {jumlah}!",
                            "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadStok(txtCari.Text.Trim());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal ubah stok: " + ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}