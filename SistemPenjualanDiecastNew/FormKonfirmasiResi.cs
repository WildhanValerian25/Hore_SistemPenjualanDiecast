using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public class FormKonfirmasiResi : Form
    {
        Label lblTitle, lblInfo, lblStatusBaru, lblResi;
        ComboBox cmbStatusBaru;
        TextBox txtResi;
        Button btnSimpan, btnBatal;

        private int _idPesanan;
        private string _statusSaat;
        private const string placeholder = "Isi jika status Dikirim";

        string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

        public FormKonfirmasiResi(int idPesanan, string statusSaat, string namaPembeli, string namaProduk)
        {
            _idPesanan = idPesanan;
            _statusSaat = statusSaat;

            this.Text = "Konfirmasi Pesanan";
            this.Size = new Size(420, 340);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // TITLE
            lblTitle = new Label();
            lblTitle.Text = "Konfirmasi Pesanan #" + idPesanan;
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitle.Location = new Point(30, 20);
            lblTitle.AutoSize = true;

            // INFO PESANAN
            lblInfo = new Label();
            lblInfo.Text = $"Pembeli  : {namaPembeli}\nProduk    : {namaProduk}\nStatus saat ini : {statusSaat}";
            lblInfo.Font = new Font("Segoe UI", 9);
            lblInfo.Location = new Point(30, 60);
            lblInfo.AutoSize = true;
            lblInfo.ForeColor = Color.DimGray;

            // STATUS BARU
            lblStatusBaru = new Label();
            lblStatusBaru.Text = "Ubah Status :";
            lblStatusBaru.Location = new Point(30, 140);
            lblStatusBaru.AutoSize = true;
            lblStatusBaru.Font = new Font("Segoe UI", 9);

            cmbStatusBaru = new ComboBox();
            cmbStatusBaru.Location = new Point(140, 137);
            cmbStatusBaru.Size = new Size(200, 22);
            cmbStatusBaru.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatusBaru.Items.AddRange(new string[] {
                "Dikonfirmasi", "Diproses", "Dikirim", "Selesai", "Dibatalkan"
            });
            cmbStatusBaru.SelectedIndex = 0;
            cmbStatusBaru.SelectedIndexChanged += CmbStatus_Changed;

            // NOMOR RESI
            lblResi = new Label();
            lblResi.Text = "Nomor Resi  :";
            lblResi.Location = new Point(30, 180);
            lblResi.AutoSize = true;
            lblResi.Font = new Font("Segoe UI", 9);

            txtResi = new TextBox();
            txtResi.Location = new Point(140, 177);
            txtResi.Size = new Size(200, 22);
            txtResi.Text = placeholder;
            txtResi.ForeColor = Color.Gray;
            txtResi.Enabled = false;

            txtResi.Enter += (s, e) => {
                if (txtResi.Text == placeholder)
                {
                    txtResi.Text = "";
                    txtResi.ForeColor = Color.Black;
                }
            };
            txtResi.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtResi.Text))
                {
                    txtResi.Text = placeholder;
                    txtResi.ForeColor = Color.Gray;
                }
            };

            // BUTTON SIMPAN
            btnSimpan = new Button();
            btnSimpan.Text = "SIMPAN";
            btnSimpan.Location = new Point(80, 240);
            btnSimpan.Size = new Size(110, 40);
            btnSimpan.BackColor = Color.Black;
            btnSimpan.ForeColor = Color.White;
            btnSimpan.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnSimpan.FlatStyle = FlatStyle.Flat;
            btnSimpan.Click += BtnSimpan_Click;

            // BUTTON BATAL
            btnBatal = new Button();
            btnBatal.Text = "BATAL";
            btnBatal.Location = new Point(210, 240);
            btnBatal.Size = new Size(110, 40);
            btnBatal.BackColor = Color.LightCoral;
            btnBatal.ForeColor = Color.White;
            btnBatal.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnBatal.FlatStyle = FlatStyle.Flat;
            btnBatal.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblTitle, lblInfo, lblStatusBaru, cmbStatusBaru,
                lblResi, txtResi, btnSimpan, btnBatal
            });
        }

        private void CmbStatus_Changed(object sender, EventArgs e)
        {
            bool isDikirim = cmbStatusBaru.SelectedItem.ToString() == "Dikirim";
            txtResi.Enabled = isDikirim;

            if (!isDikirim)
            {
                txtResi.Text = placeholder;
                txtResi.ForeColor = Color.Gray;
            }
            else
            {
                txtResi.Text = "";
                txtResi.ForeColor = Color.Black;
            }
        }

        private void BtnSimpan_Click(object sender, EventArgs e)
        {
            string statusBaru = cmbStatusBaru.SelectedItem.ToString();

            // Validasi resi wajib diisi jika status Dikirim
            if (statusBaru == "Dikirim" && (txtResi.Text.Trim() == "" || txtResi.Text == placeholder))
            {
                MessageBox.Show("Nomor resi wajib diisi jika status Dikirim!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ Pesan konfirmasi sesuai status yang dipilih
            string pesanKonfirmasi;
            if (statusBaru == "Dibatalkan")
                pesanKonfirmasi = $"Yakin ingin MEMBATALKAN pesanan #{_idPesanan}?\nStatus pesanan akan menjadi 'Dibatalkan' dan stok dikembalikan.";
            else if (statusBaru == "Dikirim")
                pesanKonfirmasi = $"Yakin mengubah status pesanan #{_idPesanan} menjadi 'Dikirim'?\nPastikan nomor resi sudah benar.";
            else
                pesanKonfirmasi = $"Yakin mengubah status pesanan #{_idPesanan} menjadi '{statusBaru}'?";

            DialogResult konfirmasi = MessageBox.Show(pesanKonfirmasi, "Konfirmasi",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (konfirmasi != DialogResult.Yes) return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlTransaction trans = null;
                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction();

                    // Update status pesanan + nomor resi
                    string qPesanan = @"UPDATE PESANAN 
                                SET status_pesanan = @status,
                                    nomor_resi     = @resi
                                WHERE id_pesanan   = @id";
                    SqlCommand cmdPesanan = new SqlCommand(qPesanan, conn, trans);
                    cmdPesanan.Parameters.AddWithValue("@status", statusBaru);
                    cmdPesanan.Parameters.AddWithValue("@resi",
                        txtResi.Text.Trim() == "" || txtResi.Text == placeholder
                            ? (object)DBNull.Value
                            : txtResi.Text.Trim());
                    cmdPesanan.Parameters.AddWithValue("@id", _idPesanan);
                    cmdPesanan.ExecuteNonQuery();

                    // Update status pembayaran jika Selesai
                    if (statusBaru == "Selesai")
                    {
                        string qBayar = @"UPDATE PEMBAYARAN 
                                  SET status_bayar  = 'Lunas',
                                      tanggal_bayar = GETDATE()
                                  WHERE id_pesanan  = @id";
                        SqlCommand cmdBayar = new SqlCommand(qBayar, conn, trans);
                        cmdBayar.Parameters.AddWithValue("@id", _idPesanan);
                        cmdBayar.ExecuteNonQuery();
                    }

                    // Kembalikan stok jika Dibatalkan
                    if (statusBaru == "Dibatalkan")
                    {
                        string qStok = @"UPDATE p
                                 SET p.stok = p.stok + dp.jumlah
                                 FROM PRODUK p
                                 INNER JOIN DETAIL_PESANAN dp ON p.id_produk = dp.id_produk
                                 WHERE dp.id_pesanan = @id";
                        SqlCommand cmdStok = new SqlCommand(qStok, conn, trans);
                        cmdStok.Parameters.AddWithValue("@id", _idPesanan);
                        cmdStok.ExecuteNonQuery();

                        // ✅ Update status pembayaran → Gagal jika dibatalkan
                        string qBayarBatal = @"UPDATE PEMBAYARAN 
                                       SET status_bayar  = 'Gagal',
                                           tanggal_bayar = GETDATE()
                                       WHERE id_pesanan  = @id";
                        SqlCommand cmdBayarBatal = new SqlCommand(qBayarBatal, conn, trans);
                        cmdBayarBatal.Parameters.AddWithValue("@id", _idPesanan);
                        cmdBayarBatal.ExecuteNonQuery();
                    }

                    trans.Commit();

                    MessageBox.Show(
                        $"Status pesanan #{_idPesanan} berhasil diubah ke '{statusBaru}'!",
                        "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    trans?.Rollback();
                    MessageBox.Show("Gagal update pesanan: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}