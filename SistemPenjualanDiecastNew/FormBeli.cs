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

        