using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace SistemPenjualanDiecastNew
{
    public  partial class FormExportExcel : Form
    {
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

        private DataGridView dgvPreview;
        private Button btnExport, btnRefresh, btnBatal;
        private Label lblTitle, lblInfo, lblLastUpdate;
        private Panel panelHeader;

        string connStr = Koneksi.GetConnectionString();

        public FormExportExcel()
        {
            InitializeUI();
            LoadPreview();
        }

        private void InitializeUI()
        {
            this.Text = "Export Data Produk ke Excel";
            this.Size = new Size(900, 620);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = cBgDark;
            this.Paint += (s, e) =>
            {
                using (LinearGradientBrush br = new LinearGradientBrush(
                    this.ClientRectangle, cBgDark, cBgMid,
                    LinearGradientMode.ForwardDiagonal))
                    e.Graphics.FillRectangle(br, this.ClientRectangle);
            };

            // Header
            panelHeader = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(11, 30, 62)
            };
            panelHeader.Paint += (s, e) =>
            {
                using (Pen p = new Pen(cCardBord, 1))
                    e.Graphics.DrawLine(p, 0, panelHeader.Height - 1,
                        panelHeader.Width, panelHeader.Height - 1);
            };

            lblTitle = new Label()
            {
                Text = "EXPORT DATA PRODUK KE EXCEL",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = cTextPri,
                BackColor = Color.Transparent,
                Location = new Point(20, 16),
                AutoSize = true
            };
            panelHeader.Controls.Add(lblTitle);

            // Info
            lblInfo = new Label()
            {
                Text = "ℹ  Data produk aktif dari database akan di-export ke file Excel secara otomatis.",
                Location = new Point(20, 75),
                Size = new Size(845, 22),
                ForeColor = Color.FromArgb(150, 200, 255),
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 9)
            };

            // Label last update
            lblLastUpdate = new Label()
            {
                Text = "Data per: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Location = new Point(20, 100),
                AutoSize = true,
                ForeColor = cTextMut,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 8.5F)
            };

            // Preview Label
            Label lblPreview = new Label()
            {
                Text = "Preview Data yang akan di-Export:",
                Location = new Point(20, 128),
                AutoSize = true,
                ForeColor = cTextMut,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            // DataGridView Preview
            dgvPreview = new DataGridView()
            {
                Location = new Point(20, 150),
                Size = new Size(845, 340),
                BackgroundColor = cInputBg,
                GridColor = cCardBord,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9)
            };

            dgvPreview.ColumnHeadersDefaultCellStyle.BackColor = cCard;
            dgvPreview.ColumnHeadersDefaultCellStyle.ForeColor = cTextPri;
            dgvPreview.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvPreview.ColumnHeadersDefaultCellStyle.SelectionBackColor = cCard;
            dgvPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvPreview.ColumnHeadersHeight = 32;
            dgvPreview.DefaultCellStyle.BackColor = cInputBg;
            dgvPreview.DefaultCellStyle.ForeColor = cTextPri;
            dgvPreview.DefaultCellStyle.SelectionBackColor = cAccent;
            dgvPreview.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvPreview.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(13, 32, 64);
            dgvPreview.AlternatingRowsDefaultCellStyle.ForeColor = cTextPri;
            dgvPreview.RowTemplate.Height = 28;

            // Warnai baris berdasarkan status stok
            dgvPreview.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                string status = dgvPreview.Rows[e.RowIndex]
                    .Cells["Status Stok"]?.Value?.ToString();
                Color warna;
                switch (status)
                {
                    case "Habis": warna = Color.FromArgb(70, 25, 30); break;
                    case "Hampir Habis": warna = Color.FromArgb(70, 60, 25); break;
                    default: warna = cInputBg; break;
                }
                dgvPreview.Rows[e.RowIndex].DefaultCellStyle.BackColor = warna;
                dgvPreview.Rows[e.RowIndex].DefaultCellStyle.ForeColor = cTextPri;
            };

            // Tombol Refresh
            btnRefresh = CreateButton("🔄  Refresh Data", new Point(20, 505),
                new Size(150, 42), cAccent);
            btnRefresh.Click += (s, e) => LoadPreview();

            // Tombol Export
            btnExport = CreateButton("📥  Export ke Excel", new Point(185, 505),
                new Size(180, 42), Color.FromArgb(35, 130, 90));
            btnExport.Click += BtnExport_Click;

            // Tombol Batal
            btnBatal = CreateButton("✖  Tutup", new Point(380, 505),
                new Size(120, 42), Color.FromArgb(80, 30, 40));
            btnBatal.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                panelHeader, lblInfo, lblLastUpdate,
                lblPreview, dgvPreview,
                btnRefresh, btnExport, btnBatal
            });
        }

        private Button CreateButton(string text, Point loc, Size size, Color bg)
        {
            Button b = new Button()
            {
                Text = text,
                Location = loc,
                Size = size,
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(bg, 0.1f);
            return b;
        }

        // ════════════════════════════════════════════════════════
        // ✅ Load Preview dari SP
        // ════════════════════════════════════════════════════════
        private void LoadPreview()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_GetProdukUntukExcel", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvPreview.DataSource = dt;

                    lblLastUpdate.Text = "Data per: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    lblInfo.Text = $"ℹ  Total {dt.Rows.Count} produk aktif siap di-export.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal load data: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ════════════════════════════════════════════════════════
        // ✅ Export ke Excel pakai ClosedXML
        // ════════════════════════════════════════════════════════
        private void BtnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Simpan File Excel";
                sfd.Filter = "Excel Files|*.xlsx";
                sfd.FileName = $"DataProduk_Diecast_{DateTime.Now:ddMMyyyy_HHmm}.xlsx";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("sp_GetProdukUntukExcel", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // ✅ Buat workbook dengan ClosedXML
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            IXLWorksheet ws = wb.Worksheets.Add("Data Produk");

                            // ── JUDUL ──
                            ws.Cell("A1").Value = "LAPORAN DATA PRODUK DIECAST";
                            ws.Range("A1:F1").Merge();
                            ws.Cell("A1").Style.Font.Bold = true;
                            ws.Cell("A1").Style.Font.FontSize = 14;
                            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromArgb(8, 18, 38);
                            ws.Cell("A1").Style.Font.FontColor = XLColor.White;

                            // ── TANGGAL EXPORT ──
                            ws.Cell("A2").Value = $"Tanggal Export: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                            ws.Range("A2:F2").Merge();
                            ws.Cell("A2").Style.Font.Italic = true;
                            ws.Cell("A2").Style.Font.FontSize = 9;
                            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell("A2").Style.Fill.BackgroundColor = XLColor.FromArgb(11, 30, 62);
                            ws.Cell("A2").Style.Font.FontColor = XLColor.FromArgb(150, 200, 255);

                            // ── HEADER KOLOM (baris 4) ──
                            string[] headers = { "No", "Nama Produk", "Merek", "Harga (Rp)", "Stok", "Status Stok" };
                            for (int i = 0; i < headers.Length; i++)
                            {
                                var cell = ws.Cell(4, i + 1);
                                cell.Value = headers[i];
                                cell.Style.Font.Bold = true;
                                cell.Style.Fill.BackgroundColor = XLColor.FromArgb(26, 86, 219);
                                cell.Style.Font.FontColor = XLColor.White;
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }

                            // ── DATA (mulai baris 5) ──
                            int row = 5;
                            int no = 1;
                            foreach (DataRow dr in dt.Rows)
                            {
                                ws.Cell(row, 1).Value = no++;
                                ws.Cell(row, 2).Value = dr["Nama Produk"].ToString();
                                ws.Cell(row, 3).Value = dr["Merek"].ToString();
                                ws.Cell(row, 4).Value = Convert.ToDecimal(dr["Harga"]);
                                ws.Cell(row, 5).Value = Convert.ToInt32(dr["Stok"]);
                                ws.Cell(row, 6).Value = dr["Status Stok"].ToString();

                                // Format harga
                                ws.Cell(row, 4).Style.NumberFormat.Format = "#,##0";
                                ws.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                                // Format stok
                                ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                // Warnai berdasarkan status stok
                                string statusStok = dr["Status Stok"].ToString();
                                XLColor bgColor;
                                switch (statusStok)
                                {
                                    case "Habis":
                                        bgColor = XLColor.FromArgb(255, 200, 200);
                                        break;
                                    case "Hampir Habis":
                                        bgColor = XLColor.FromArgb(255, 235, 180);
                                        break;
                                    default:
                                        bgColor = row % 2 == 0
                                            ? XLColor.FromArgb(240, 245, 255)
                                            : XLColor.White;
                                        break;
                                }

                                // Apply warna ke seluruh baris
                                for (int col = 1; col <= 6; col++)
                                {
                                    ws.Cell(row, col).Style.Fill.BackgroundColor = bgColor;
                                    ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(row, col).Style.Border.OutsideBorderColor = XLColor.FromArgb(200, 210, 230);
                                }

                                row++;
                            }

                            // ── TOTAL BARIS ──
                            ws.Cell(row, 1).Value = "TOTAL";
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Cell(row, 5).FormulaA1 = $"=SUM(E5:E{row - 1})";
                            ws.Cell(row, 5).Style.Font.Bold = true;
                            ws.Range(row, 1, row, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(220, 230, 255);

                            // ── AUTO FIT KOLOM ──
                            ws.Columns().AdjustToContents();

                            // Set lebar minimum kolom
                            ws.Column(1).Width = 5;   // No
                            ws.Column(2).Width = 30;  // Nama Produk
                            ws.Column(3).Width = 18;  // Merek
                            ws.Column(4).Width = 18;  // Harga
                            ws.Column(5).Width = 8;   // Stok
                            ws.Column(6).Width = 15;  // Status

                            // ── FREEZE HEADER ──
                            ws.SheetView.FreezeRows(4);

                            // ── SIMPAN FILE ──
                            wb.SaveAs(sfd.FileName);
                        }
                    }

                    MessageBox.Show(
                        $"Export berhasil!\n\nFile: {Path.GetFileName(sfd.FileName)}\n" +
                        $"Lokasi: {Path.GetDirectoryName(sfd.FileName)}",
                        "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Tanya buka file langsung
                    if (MessageBox.Show("Buka file Excel sekarang?", "Buka File",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal export: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}