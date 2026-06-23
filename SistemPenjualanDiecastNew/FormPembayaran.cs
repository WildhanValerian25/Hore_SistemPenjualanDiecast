using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public class FormPembayaran : Form
    {
        Label lblTitle, lblIdPesanan, lblTotal, lblMetode, lblBankInfo, lblBukti, lblStatus;
        Button btnUploadBukti, btnKirimBukti, btnBatal;
        PictureBox picBukti;
        Panel panelInfo;

        private string _buktiPath = "";
        private int _idPesanan;
        private decimal _totalHarga;
        private string _metode;

        string connStr = Koneksi.GetConnectionString();

        public FormPembayaran(int idPesanan, decimal totalHarga, string metode)
        {
            _idPesanan = idPesanan;
            _totalHarga = totalHarga;
            _metode = metode;

            this.Text = "Pembayaran Pesanan #" + idPesanan;
            this.Size = new Size(500, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            BuildUI();
        }

        private void BuildUI()
        {
            lblTitle = new Label()
            {
                Text = "Pembayaran Pesanan #" + _idPesanan,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true,
                ForeColor = Color.DarkBlue
            };

            panelInfo = new Panel()
            {
                Location = new Point(20, 55),
                Size = new Size(440, 160),
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblIdPesanan = new Label()
            {
                Text = "ID Pesanan  : #" + _idPesanan,
                Font = new Font("Segoe UI", 10),
                Location = new Point(15, 15),
                AutoSize = true
            };

            lblTotal = new Label()
            {
                Text = "Total Bayar   : Rp " + _totalHarga.ToString("N0"),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                Location = new Point(15, 45),
                AutoSize = true
            };

            lblMetode = new Label()
            {
                Text = "Metode        : " + _metode,
                Font = new Font("Segoe UI", 10),
                Location = new Point(15, 75),
                AutoSize = true
            };

            lblBankInfo = new Label()
            {
                Text = GetPaymentInfo(_metode),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.DarkRed,
                Location = new Point(15, 105),
                AutoSize = true
            };

            panelInfo.Controls.AddRange(new Control[] {
                lblIdPesanan, lblTotal, lblMetode, lblBankInfo
            });

            bool isCOD = _metode == "COD";

            lblBukti = new Label()
            {
                Text = "Upload Bukti Pembayaran:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 230),
                AutoSize = true,
                Visible = !isCOD
            };

            btnUploadBukti = new Button()
            {
                Text = "Pilih File Bukti (JPG/PNG)",
                Location = new Point(20, 258),
                Size = new Size(200, 35),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Visible = !isCOD
            };
            btnUploadBukti.Click += BtnUploadBukti_Click;

            picBukti = new PictureBox()
            {
                Location = new Point(20, 305),
                Size = new Size(440, 200),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(245, 245, 245),
                Visible = !isCOD
            };

            Label lblPlaceholder = new Label()
            {
                Text = "Preview bukti pembayaran",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9),
                Location = new Point(140, 90),
                AutoSize = true
            };
            picBukti.Controls.Add(lblPlaceholder);

            lblStatus = new Label()
            {
                Text = "",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.DarkGreen,
                Location = new Point(240, 265),
                AutoSize = true,
                Visible = !isCOD
            };

            btnKirimBukti = new Button()
            {
                Text = isCOD ? "KONFIRMASI PESANAN COD" : "KIRIM BUKTI PEMBAYARAN",
                Location = new Point(20, 520),
                Size = new Size(220, 42),
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnKirimBukti.Click += BtnKirimBukti_Click;

            btnBatal = new Button()
            {
                Text = "TUTUP",
                Location = new Point(255, 520),
                Size = new Size(110, 42),
                BackColor = Color.LightCoral,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnBatal.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblTitle, panelInfo, lblBukti,
                btnUploadBukti, picBukti, lblStatus,
                btnKirimBukti, btnBatal
            });
        }

        private string GetPaymentInfo(string metode)
        {
            switch (metode)
            {
                case "Transfer Bank": return "BCA: 1234567890 a/n Wildhan Diecast\nBRI: 0987654321 a/n Wildhan Diecast";
                case "QRIS": return "Scan QRIS di kasir atau hubungi admin untuk QR Code";
                case "COD": return "Bayar saat barang tiba — tidak perlu upload bukti";
                case "Dompet Digital": return "GoPay/OVO/Dana: 08123456789 a/n Wildhan Diecast";
                default: return "";
            }
        }

        private void BtnUploadBukti_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                ofd.Title = "Pilih Bukti Pembayaran";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _buktiPath = ofd.FileName;
                    picBukti.Image = Image.FromFile(_buktiPath);
                    picBukti.Controls.Clear();
                    lblStatus.Text = "File dipilih: " + Path.GetFileName(_buktiPath);
                    lblStatus.ForeColor = Color.DarkGreen;
                }
            }
        }

        private void BtnKirimBukti_Click(object sender, EventArgs e)
        {
            // COD — langsung konfirmasi tanpa cek bukti
            if (_metode == "COD")
            {
                DialogResult konfirmasi = MessageBox.Show(
                    $"Konfirmasi pesanan COD?\n\n" +
                    $"ID Pesanan : #{_idPesanan}\n" +
                    $"Total      : Rp {_totalHarga:N0}\n\n" +
                    $"Pembayaran dilakukan saat barang tiba.",
                    "Konfirmasi COD",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (konfirmasi != DialogResult.Yes) return;

                UpdateStatusBayar(null);
                return;
            }

            // Non-COD wajib upload bukti
            if (string.IsNullOrEmpty(_buktiPath))
            {
                MessageBox.Show("Silakan upload bukti pembayaran terlebih dahulu!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UpdateStatusBayar(_buktiPath);
        }

        // =============================================
        // ✅ PERBAIKAN — Sekarang memanggil Stored Procedure
        //    sp_KonfirmasiPembayaran, bukan raw query lagi.
        //    SP ini sekaligus mengurus update PEMBAYARAN
        //    dan PESANAN dalam satu transaksi di sisi DB.
        // =============================================
        private void UpdateStatusBayar(string buktiPath)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_KonfirmasiPembayaran", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_pesanan", _idPesanan);

                        SqlParameter paramBukti = new SqlParameter("@bukti_bayar", SqlDbType.VarBinary, -1);
                        paramBukti.Value = !string.IsNullOrEmpty(buktiPath)
                            ? (object)File.ReadAllBytes(buktiPath)   // Non-COD: kirim byte[] gambar
                            : DBNull.Value;                          // COD: kirim NULL
                        cmd.Parameters.Add(paramBukti);

                        cmd.ExecuteNonQuery();
                    }

                    string pesan = _metode == "COD"
                        ? "Pesanan COD berhasil dikonfirmasi!\nAdmin akan segera memproses pesanan Anda."
                        : "Bukti pembayaran berhasil dikirim!\nAdmin akan mengkonfirmasi pembayaran Anda segera.";

                    MessageBox.Show(pesan, "Berhasil",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal kirim bukti: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}