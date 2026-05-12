using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SistemPenjualanDiecastNew
{
    public class FormBeli : Form
    {
        Label lblTitle, lblNamaProduk, lblHarga, lblJumlah, lblAlamat, lblTotal;
        TextBox txtJumlah, txtAlamat;
        Button btnKonfirmasi, btnBatal;
        ComboBox cmbMetode;
        Label lblMetode;

        private int _idProduk;
        private string _namaProduk;
        private decimal _harga;
        private int _stokTersedia;
        private string _username;

        string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

        public FormBeli(string username, int idProduk, string namaProduk, decimal harga, int stok)
        {
            _username = username;
            _idProduk = idProduk;
            _namaProduk = namaProduk;
            _harga = harga;
            _stokTersedia = stok;

            InitializeForm();
            BuildUI();
            LoadAlamatUser();
        }

        private void InitializeForm()
        {
            this.Text = "Konfirmasi Pembelian";
            this.Size = new Size(480, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void BuildUI()
        {
            // TITLE
            lblTitle = new Label();
            lblTitle.Text = "Konfirmasi Pembelian";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.Location = new Point(120, 20);
            lblTitle.AutoSize = true;

            // NAMA PRODUK
            lblNamaProduk = new Label();
            lblNamaProduk.Text = "Produk       : " + _namaProduk;
            lblNamaProduk.Location = new Point(30, 70);
            lblNamaProduk.AutoSize = true;
            lblNamaProduk.Font = new Font("Segoe UI", 10);

            // HARGA
            lblHarga = new Label();
            lblHarga.Text = "Harga          : Rp " + _harga.ToString("N0");
            lblHarga.Location = new Point(30, 100);
            lblHarga.AutoSize = true;
            lblHarga.Font = new Font("Segoe UI", 10);

            // JUMLAH
            lblJumlah = new Label();
            lblJumlah.Text = "Jumlah        :";
            lblJumlah.Location = new Point(30, 135);
            lblJumlah.AutoSize = true;
            lblJumlah.Font = new Font("Segoe UI", 10);

            txtJumlah = new TextBox();
            txtJumlah.Location = new Point(160, 132);
            txtJumlah.Size = new Size(80, 22);
            txtJumlah.Text = "1";
            txtJumlah.TextChanged += TxtJumlah_TextChanged;

            // TOTAL
            lblTotal = new Label();
            lblTotal.Text = "Total            : Rp " + _harga.ToString("N0");
            lblTotal.Location = new Point(30, 165);
            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTotal.ForeColor = Color.DarkGreen;

            // ALAMAT
            lblAlamat = new Label();
            lblAlamat.Text = "Alamat Kirim :";
            lblAlamat.Location = new Point(30, 200);
            lblAlamat.AutoSize = true;
            lblAlamat.Font = new Font("Segoe UI", 10);

            txtAlamat = new TextBox();
            txtAlamat.Location = new Point(160, 197);
            txtAlamat.Size = new Size(270, 22);

            // METODE PEMBAYARAN
            lblMetode = new Label();
            lblMetode.Text = "Pembayaran :";
            lblMetode.Location = new Point(30, 235);
            lblMetode.AutoSize = true;
            lblMetode.Font = new Font("Segoe UI", 10);

            cmbMetode = new ComboBox();
            cmbMetode.Location = new Point(160, 232);
            cmbMetode.Size = new Size(200, 22);
            cmbMetode.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMetode.Items.AddRange(new string[] {
                "Transfer Bank", "QRIS", "COD", "Dompet Digital"
            });
            cmbMetode.SelectedIndex = 0;

            // TOMBOL KONFIRMASI
            btnKonfirmasi = new Button();
            btnKonfirmasi.Text = "KONFIRMASI BELI";
            btnKonfirmasi.Location = new Point(60, 310);
            btnKonfirmasi.Size = new Size(160, 40);
            btnKonfirmasi.BackColor = Color.Black;
            btnKonfirmasi.ForeColor = Color.White;
            btnKonfirmasi.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnKonfirmasi.FlatStyle = FlatStyle.Flat;
            btnKonfirmasi.Click += BtnKonfirmasi_Click;

            // TOMBOL BATAL
            btnBatal = new Button();
            btnBatal.Text = "BATAL";
            btnBatal.Location = new Point(240, 310);
            btnBatal.Size = new Size(160, 40);
            btnBatal.BackColor = Color.LightCoral;
            btnBatal.ForeColor = Color.White;
            btnBatal.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnBatal.FlatStyle = FlatStyle.Flat;
            btnBatal.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblTitle, lblNamaProduk, lblHarga,
                lblJumlah, txtJumlah, lblTotal,
                lblAlamat, txtAlamat,
                lblMetode, cmbMetode,
                btnKonfirmasi, btnBatal
            });
        }

        // ✅ Auto-load alamat dari database
        private void LoadAlamatUser()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT alamat FROM PELANGGAN WHERE [username] = @u";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@u", _username);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        txtAlamat.Text = result.ToString();
                }
                catch { }
            }
        }

        // ✅ Update total otomatis saat jumlah berubah
        private void TxtJumlah_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtJumlah.Text, out int jumlah) && jumlah > 0)
            {
                decimal total = jumlah * _harga;
                lblTotal.Text = "Total            : Rp " + total.ToString("N0");
            }
            else
            {
                lblTotal.Text = "Total            : Rp 0";
            }
        }

        private void BtnKonfirmasi_Click(object sender, EventArgs e)
        {
            // Validasi jumlah
            if (!int.TryParse(txtJumlah.Text, out int jumlah) || jumlah <= 0)
            {
                MessageBox.Show("Jumlah harus berupa angka dan lebih dari 0!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (jumlah > _stokTersedia)
            {
                MessageBox.Show($"Stok tidak mencukupi! Stok tersedia: {_stokTersedia}", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi alamat
            if (txtAlamat.Text.Trim() == "")
            {
                MessageBox.Show("Alamat pengiriman wajib diisi!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal totalHarga = jumlah * _harga;
            string metode = cmbMetode.SelectedItem.ToString();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlTransaction trans = null;
                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction(); // ✅ Pakai transaksi agar aman

                    // 1. Ambil id_pelanggan dari username
                    string qId = "SELECT id_pelanggan FROM PELANGGAN WHERE [username] = @u";
                    SqlCommand cmdId = new SqlCommand(qId, conn, trans);
                    cmdId.Parameters.AddWithValue("@u", _username);
                    int idPelanggan = (int)cmdId.ExecuteScalar();

                    // 2. Insert ke tabel PESANAN
                    string qPesanan = @"INSERT INTO PESANAN 
                                        (id_pelanggan, total_harga, status_pesanan, alamat_kirim)
                                        OUTPUT INSERTED.id_pesanan
                                        VALUES (@idp, @total, 'Pending', @alamat)";
                    SqlCommand cmdPesanan = new SqlCommand(qPesanan, conn, trans);
                    cmdPesanan.Parameters.AddWithValue("@idp", idPelanggan);
                    cmdPesanan.Parameters.AddWithValue("@total", totalHarga);
                    cmdPesanan.Parameters.AddWithValue("@alamat", txtAlamat.Text.Trim());
                    int idPesanan = (int)cmdPesanan.ExecuteScalar();

                    // 3. Insert ke tabel DETAIL_PESANAN
                    string qDetail = @"INSERT INTO DETAIL_PESANAN 
                                       (id_pesanan, id_produk, jumlah, harga_satuan)
                                       VALUES (@idpes, @idprod, @jml, @harga)";
                    SqlCommand cmdDetail = new SqlCommand(qDetail, conn, trans);
                    cmdDetail.Parameters.AddWithValue("@idpes", idPesanan);
                    cmdDetail.Parameters.AddWithValue("@idprod", _idProduk);
                    cmdDetail.Parameters.AddWithValue("@jml", jumlah);
                    cmdDetail.Parameters.AddWithValue("@harga", _harga);
                    cmdDetail.ExecuteNonQuery();

                    // 4. Insert ke tabel PEMBAYARAN
                    string qBayar = @"INSERT INTO PEMBAYARAN 
                                      (id_pesanan, metode, jumlah_bayar, status_bayar)
                                      VALUES (@idpes, @metode, @total, 'Menunggu')";
                    SqlCommand cmdBayar = new SqlCommand(qBayar, conn, trans);
                    cmdBayar.Parameters.AddWithValue("@idpes", idPesanan);
                    cmdBayar.Parameters.AddWithValue("@metode", metode);
                    cmdBayar.Parameters.AddWithValue("@total", totalHarga);
                    cmdBayar.ExecuteNonQuery();

                    // 5. ✅ Kurangi stok di tabel PRODUK
                    string qStok = "UPDATE PRODUK SET stok = stok - @jml WHERE id_produk = @idprod";
                    SqlCommand cmdStok = new SqlCommand(qStok, conn, trans);
                    cmdStok.Parameters.AddWithValue("@jml", jumlah);
                    cmdStok.Parameters.AddWithValue("@idprod", _idProduk);
                    cmdStok.ExecuteNonQuery();

                    trans.Commit(); // ✅ Semua berhasil, simpan ke database

                    MessageBox.Show(
                        $"Pesanan berhasil dibuat!\n\n" +
                        $"Produk  : {_namaProduk}\n" +
                        $"Jumlah  : {jumlah}\n" +
                        $"Total    : Rp {totalHarga:N0}\n" +
                        $"Metode  : {metode}\n" +
                        $"Status   : Menunggu Pembayaran",
                        "Pesanan Berhasil",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    trans?.Rollback(); // ✅ Gagal? Batalkan semua perubahan
                    MessageBox.Show("Gagal membuat pesanan: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}