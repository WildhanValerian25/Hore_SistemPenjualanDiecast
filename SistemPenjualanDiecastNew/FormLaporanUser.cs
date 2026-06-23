using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public partial class FormLaporanUser : Form
    {
        private CrystalReportViewer crystalViewer;
        private DateTimePicker dtpAwal, dtpAkhir;
        private Button btnTampilkan, btnCetak, btnBack;
        private Label lblAwal, lblAkhir, lblInfo;
        private Panel panelFilter;

        private readonly Color cBgDark = Color.FromArgb(8, 18, 38);
        private readonly Color cBgMid = Color.FromArgb(11, 30, 62);
        private readonly Color cCard = Color.FromArgb(14, 38, 78);
        private readonly Color cCardBord = Color.FromArgb(25, 60, 110);
        private readonly Color cAccent = Color.FromArgb(26, 86, 219);
        private readonly Color cAccentHover = Color.FromArgb(35, 100, 235);
        private readonly Color cInputBrd = Color.FromArgb(40, 70, 130);
        private readonly Color cTextPri = Color.FromArgb(230, 238, 255);
        private readonly Color cTextMut = Color.FromArgb(100, 130, 180);

        private string _username;

        string connStr = Koneksi.GetConnectionString();

        public FormLaporanUser(string username)
        {
            _username = username;
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Riwayat Pesanan Saya";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = cBgDark;
            this.Font = new Font("Segoe UI", 9);

            // ── PANEL FILTER ──
            panelFilter = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(11, 30, 62)
            };
            panelFilter.Paint += (s, e) =>
            {
                using (Pen p = new Pen(cCardBord, 1))
                    e.Graphics.DrawLine(p, 0, panelFilter.Height - 1,
                        panelFilter.Width, panelFilter.Height - 1);
            };

            btnBack = new Button()
            {
                Text = "← Kembali",
                Location = new Point(10, 13),
                Size = new Size(95, 32),
                BackColor = Color.FromArgb(20, 50, 100),
                ForeColor = Color.FromArgb(150, 180, 230),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 1;
            btnBack.FlatAppearance.BorderColor = cInputBrd;
            btnBack.Click += (s, e) => this.Close();

            lblAwal = new Label()
            {
                Text = "Dari:",
                Location = new Point(120, 20),
                AutoSize = true,
                ForeColor = cTextMut,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            dtpAwal = new DateTimePicker()
            {
                Location = new Point(155, 16),
                Size = new Size(130, 28),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddMonths(-1)
            };

            lblAkhir = new Label()
            {
                Text = "Sampai:",
                Location = new Point(300, 20),
                AutoSize = true,
                ForeColor = cTextMut,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            dtpAkhir = new DateTimePicker()
            {
                Location = new Point(355, 16),
                Size = new Size(130, 28),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };

            btnTampilkan = new Button()
            {
                Text = "TAMPILKAN",
                Location = new Point(500, 13),
                Size = new Size(110, 32),
                BackColor = cAccent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTampilkan.FlatAppearance.BorderSize = 0;
            btnTampilkan.FlatAppearance.MouseOverBackColor = cAccentHover;
            btnTampilkan.Click += BtnTampilkan_Click;

            btnCetak = new Button()
            {
                Text = "🖨 CETAK",
                Location = new Point(622, 13),
                Size = new Size(100, 32),
                BackColor = Color.FromArgb(35, 130, 90),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCetak.FlatAppearance.BorderSize = 0;
            btnCetak.Click += (s, e) => crystalViewer?.PrintReport();

            // Label info username
            lblInfo = new Label()
            {
                Text = $"Menampilkan pesanan: {_username}",
                Location = new Point(740, 20),
                AutoSize = true,
                ForeColor = cTextMut,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 8.5F)
            };

            panelFilter.Controls.AddRange(new Control[] {
                btnBack, lblAwal, dtpAwal, lblAkhir, dtpAkhir,
                btnTampilkan, btnCetak, lblInfo
            });

            // ── CRYSTAL REPORT VIEWER ──
            crystalViewer = new CrystalReportViewer()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                DisplayGroupTree = false
            };

            this.Controls.Add(crystalViewer);
            this.Controls.Add(panelFilter);

            this.Load += (s, e) => BtnTampilkan_Click(s, e);
        }

        // ════════════════════════════════════════════════════════
        // ✅ TAMPILKAN REPORT
        // ════════════════════════════════════════════════════════
        private void BtnTampilkan_Click(object sender, EventArgs e)
        {
            try
            {
                ListRiwayatPesanan listData = AmbilDataRiwayat(
                    _username,
                    dtpAwal.Value.Date,
                    dtpAkhir.Value.Date);

                if (listData.Count == 0)
                {
                    MessageBox.Show(
                        "Tidak ada pesanan pada rentang tanggal yang dipilih.",
                        "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ReportDocument report = new ReportDocument();
                report.Load(System.IO.Path.Combine(
                    Application.StartupPath, "LaporanRiwayatPesanan.rpt"));

                // ✅ Set datasource ke ListRiwayatPesanan
                report.SetDataSource(listData);

                crystalViewer.ReportSource = report;
                crystalViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat laporan: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════
        // ✅ Ambil data dari SP sp_LaporanRiwayatPesanan
        //
        // ✅ PERBAIKAN: HargaSatuan dan Subtotal sekarang dibaca
        //    dengan Convert.ToDecimal(), BUKAN .ToString() lagi.
        //
        //    Alasan: di Crystal Reports Designer, field bertipe
        //    string tidak bisa dipakai untuk operasi Summary (Sum)
        //    di Report Footer — opsi "Sum" tidak akan muncul di
        //    dropdown "Calculate this summary", hanya muncul
        //    Count/Distinct count/Mode dsb yang berlaku untuk teks.
        //
        //    Dengan membaca sebagai decimal di sini (dan SP yang
        //    mengembalikan angka mentah, bukan string "Rp ..."),
        //    field HargaSatuan dan Subtotal di report jadi field
        //    numerik asli, sehingga:
        //      - Bisa di-Sum untuk Grand Total di Report Footer
        //      - Format "Rp" dan pemisah ribuan diatur di Crystal
        //        Reports Designer (klik kanan field -> Format Field
        //        -> Number -> Customize), bukan dari sisi data.
        //
        //    SP sp_LaporanRiwayatPesanan JUGA HARUS diubah agar
        //    kolom HargaSatuan dan Subtotal mengembalikan angka
        //    mentah (dp.harga_satuan, dp.subtotal), bukan string
        //    yang sudah diformat dengan 'Rp ' + FORMAT(...).
        // ════════════════════════════════════════════════════════
        private ListRiwayatPesanan AmbilDataRiwayat(
            string username, DateTime tglAwal, DateTime tglAkhir)
        {
            ListRiwayatPesanan list = new ListRiwayatPesanan();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("sp_LaporanRiwayatPesanan", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                cmd.Parameters.Add("@tgl_awal", SqlDbType.Date).Value = tglAwal;
                cmd.Parameters.Add("@tgl_akhir", SqlDbType.Date).Value = tglAkhir;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DataRiwayatPesanan
                        {
                            TanggalPesan = reader["TanggalPesan"].ToString(),
                            NamaProduk = reader["NamaProduk"].ToString(),
                            Qty = Convert.ToInt32(reader["Qty"]),
                            HargaSatuan = Convert.ToDecimal(reader["HargaSatuan"]),
                            Subtotal = Convert.ToDecimal(reader["Subtotal"]),
                            MetodeBayar = reader["MetodeBayar"].ToString(),
                            StatusBayar = reader["StatusBayar"].ToString(),
                            StatusPesanan = reader["StatusPesanan"].ToString(),
                            NomorResi = reader["NomorResi"].ToString()
                        });
                    }
                }
            }

            return list;
        }
    }
}