using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;

namespace SistemPenjualanDiecastNew
{
    public partial class FormLaporanPenjualan : Form
    {
        private CrystalReportViewer crystalViewer;
        private DateTimePicker dtpAwal, dtpAkhir;
        private Button btnTampilkan, btnCetak, btnBack;
        private Label lblAwal, lblAkhir;
        private Panel panelFilter;

        // ── PALET WARNA ──
        private readonly Color cBgDark = Color.FromArgb(8, 18, 38);
        private readonly Color cBgMid = Color.FromArgb(11, 30, 62);
        private readonly Color cCard = Color.FromArgb(14, 38, 78);
        private readonly Color cCardBord = Color.FromArgb(25, 60, 110);
        private readonly Color cAccent = Color.FromArgb(26, 86, 219);
        private readonly Color cAccentHover = Color.FromArgb(35, 100, 235);
        private readonly Color cInputBg = Color.FromArgb(10, 26, 55);
        private readonly Color cInputBrd = Color.FromArgb(40, 70, 130);
        private readonly Color cTextPri = Color.FromArgb(230, 238, 255);
        private readonly Color cTextMut = Color.FromArgb(100, 130, 180);

        string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

        public FormLaporanPenjualan()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Laporan Penjualan";
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

            // Tombol Back
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
                Value = DateTime.Now.AddDays(-30),
                CalendarForeColor = cTextPri,
                CalendarMonthBackground = cInputBg
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

            panelFilter.Controls.AddRange(new Control[] {
                btnBack, lblAwal, dtpAwal, lblAkhir, dtpAkhir,
                btnTampilkan, btnCetak
            });

            // ── CRYSTAL REPORT VIEWER ──
            crystalViewer = new CrystalReportViewer()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                DisplayGroupTree = false   // sembunyikan group tree di kiri
            };

            this.Controls.Add(crystalViewer);
            this.Controls.Add(panelFilter);

            // Auto tampilkan saat form dibuka
            this.Load += (s, e) => BtnTampilkan_Click(s, e);
        }

        // ════════════════════════════════════════════════════════
        // ✅ TAMPILKAN REPORT — pakai ListLaporanPenjualan
        // ════════════════════════════════════════════════════════
        private void BtnTampilkan_Click(object sender, EventArgs e)
        {
            try
            {
                // ✅ Ambil data dari SP → konversi ke List
                ListLaporanPenjualan listData = AmbilDataPenjualan(
                    dtpAwal.Value.Date,
                    dtpAkhir.Value.Date);

                if (listData.Count == 0)
                {
                    MessageBox.Show(
                        "Tidak ada data pada rentang tanggal yang dipilih.",
                        "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // ✅ Load file .rpt
                ReportDocument report = new ReportDocument();
                report.Load(System.IO.Path.Combine(
                    Application.StartupPath, "LaporanPenjualan.rpt"));

                // ✅ Set datasource ke ListLaporanPenjualan
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
        // ✅ Ambil data dari SP sp_LaporanPenjualan
        // ════════════════════════════════════════════════════════
        private ListLaporanPenjualan AmbilDataPenjualan(DateTime tglAwal, DateTime tglAkhir)
        {
            ListLaporanPenjualan list = new ListLaporanPenjualan();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("sp_LaporanPenjualan", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@tgl_awal", SqlDbType.Date).Value = tglAwal;
                cmd.Parameters.Add("@tgl_akhir", SqlDbType.Date).Value = tglAkhir;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DataLaporanPenjualan
                        {
                            TanggalPesan = reader["TanggalPesan"].ToString(),
                            NamaPembeli = reader["NamaPembeli"].ToString(),
                            NamaProduk = reader["NamaProduk"].ToString(),
                            Qty = Convert.ToInt32(reader["Qty"]),
                            HargaSatuan = reader["HargaSatuan"].ToString(),
                            Subtotal = reader["Subtotal"].ToString(),
                            StatusPesanan = reader["StatusPesanan"].ToString()
                        });
                    }
                }
            }

            return list;
        }
    }
}