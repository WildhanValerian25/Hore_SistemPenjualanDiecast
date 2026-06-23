using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using ExcelDataReader;

namespace SistemPenjualanDiecastNew
{
    public partial class FormImportExcel : Form
    {
        private Button btnPilihFile, btnImport, btnBack;
        private Label lblTitle, lblFilePath, lblHasil, lblFormatInfo;
        private DataGridView dgvPreview;
        private ProgressBar progressImport;
        private Panel pnlCard;

        private string _filePath = "";
        private DataTable _previewData;
        private readonly int _idAdmin;

        // ── PALET WARNA (disamakan dengan form lain) ──
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

        /// <summary>
        /// idAdmin: ID admin yang sedang login, dipakai sebagai
        /// id_admin di setiap baris produk yang diimport.
        /// </summary>
        public FormImportExcel(int idAdmin)
        {
            _idAdmin = idAdmin;
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Import Produk dari Excel";
            this.Size = new Size(900, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = cBgDark;
            this.Font = new Font("Segoe UI", 9.5F);
            this.Paint += Form_Paint;

            pnlCard = new Panel()
            {
                Location = new Point(24, 20),
                Size = new Size(852, 620),
                BackColor = Color.Transparent
            };
            pnlCard.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle r = new Rectangle(0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
                using (GraphicsPath path = RoundedPath(r, 14))
                {
                    using (SolidBrush fillBr = new SolidBrush(cCard))
                        g.FillPath(fillBr, path);
                    using (Pen pen = new Pen(cCardBord, 1f))
                        g.DrawPath(pen, path);
                    pnlCard.Region = new Region(path);
                }
            };

            btnBack = new Button()
            {
                Text = "← Kembali",
                Location = new Point(20, 18),
                Size = new Size(100, 32),
                BackColor = Color.FromArgb(20, 50, 100),
                ForeColor = Color.FromArgb(150, 180, 230),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 1;
            btnBack.FlatAppearance.BorderColor = cInputBrd;
            ApplyRoundRegion(btnBack, 8);
            btnBack.Click += (s, e) => this.Close();

            lblTitle = new Label()
            {
                Text = "Import Produk dari Excel",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = cTextPri,
                BackColor = Color.Transparent,
                Location = new Point(132, 22),
                AutoSize = true
            };

            lblFormatInfo = new Label()
            {
                Text = "Format kolom Excel (tanpa header / mulai baris 1): Nama Produk | Merek | Harga | Stok",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = cTextMut,
                BackColor = Color.Transparent,
                Location = new Point(20, 64),
                AutoSize = true
            };

            btnPilihFile = new Button()
            {
                Text = "Pilih File Excel",
                Location = new Point(20, 96),
                Size = new Size(180, 38),
                BackColor = cAccent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPilihFile.FlatAppearance.BorderSize = 0;
            btnPilihFile.FlatAppearance.MouseOverBackColor = cAccentHover;
            ApplyRoundRegion(btnPilihFile, 8);
            btnPilihFile.Click += BtnPilihFile_Click;

            lblFilePath = new Label()
            {
                Text = "Belum ada file dipilih.",
                Font = new Font("Segoe UI", 9),
                ForeColor = cTextMut,
                BackColor = Color.Transparent,
                Location = new Point(212, 106),
                AutoSize = true
            };

            dgvPreview = new DataGridView()
            {
                Location = new Point(20, 146),
                Size = new Size(812, 340),
                BackgroundColor = cInputBg,
                GridColor = cCardBord,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9F)
            };
            dgvPreview.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(18, 44, 88);
            dgvPreview.ColumnHeadersDefaultCellStyle.ForeColor = cTextPri;
            dgvPreview.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvPreview.ColumnHeadersHeight = 32;
            dgvPreview.DefaultCellStyle.BackColor = cInputBg;
            dgvPreview.DefaultCellStyle.ForeColor = cTextPri;
            dgvPreview.DefaultCellStyle.SelectionBackColor = cAccent;
            dgvPreview.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(13, 32, 64);
            dgvPreview.AlternatingRowsDefaultCellStyle.ForeColor = cTextPri;

            progressImport = new ProgressBar()
            {
                Location = new Point(20, 500),
                Size = new Size(812, 22),
                Visible = false
            };

            btnImport = new Button()
            {
                Text = "MULAI IMPORT",
                Location = new Point(20, 534),
                Size = new Size(180, 42),
                BackColor = Color.FromArgb(35, 130, 90),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnImport.FlatAppearance.BorderSize = 0;
            btnImport.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 150, 105);
            ApplyRoundRegion(btnImport, 8);
            btnImport.Click += BtnImport_Click;

            lblHasil = new Label()
            {
                Text = "",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = cTextMut,
                BackColor = Color.Transparent,
                Location = new Point(214, 546),
                AutoSize = true
            };

            pnlCard.Controls.AddRange(new Control[] {
                btnBack, lblTitle, lblFormatInfo,
                btnPilihFile, lblFilePath, dgvPreview,
                progressImport, btnImport, lblHasil
            });

            this.Controls.Add(pnlCard);
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush br = new LinearGradientBrush(
                this.ClientRectangle, cBgDark, cBgMid, LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(br, this.ClientRectangle);
            }
        }

        // ════════════════════════════════════════════════════════
        // PILIH FILE EXCEL & PREVIEW
        // ════════════════════════════════════════════════════════
        private void BtnPilihFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Excel Files|*.xls;*.xlsx";
                ofd.Title = "Pilih File Excel Produk";

                if (ofd.ShowDialog() != DialogResult.OK) return;

                _filePath = ofd.FileName;
                lblFilePath.Text = Path.GetFileName(_filePath);
                lblFilePath.ForeColor = cTextPri;

                try
                {
                    _previewData = BacaFileExcel(_filePath);
                    dgvPreview.DataSource = _previewData;
                    btnImport.Enabled = _previewData.Rows.Count > 0;
                    lblHasil.Text = $"{_previewData.Rows.Count} baris ditemukan, siap diimport.";
                    lblHasil.ForeColor = cTextMut;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal membaca file Excel: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnImport.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Membaca file Excel pakai ExcelDataReader, mengembalikan
        /// DataTable dengan 4 kolom sesuai format yang ditentukan:
        /// Nama Produk | Merek | Harga | Stok (tanpa baris header).
        /// </summary>
        private DataTable BacaFileExcel(string path)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Nama Produk", typeof(string));
            dt.Columns.Add("Merek", typeof(string));
            dt.Columns.Add("Harga", typeof(decimal));
            dt.Columns.Add("Stok", typeof(int));

            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                bool barisPertama = true;

                do
                {
                    while (reader.Read())
                    {
                        // Lewati baris pertama jika dianggap header
                        // (heuristik: jika kolom ke-3/Harga bukan angka, anggap header)
                        if (barisPertama)
                        {
                            barisPertama = false;
                            if (!decimal.TryParse(reader.GetValue(2)?.ToString(), out _))
                                continue;
                        }

                        string nama = reader.GetValue(0)?.ToString()?.Trim() ?? "";
                        string merek = reader.GetValue(1)?.ToString()?.Trim() ?? "";

                        if (string.IsNullOrWhiteSpace(nama)) continue; // baris kosong, skip

                        decimal.TryParse(reader.GetValue(2)?.ToString(), out decimal harga);
                        int.TryParse(reader.GetValue(3)?.ToString(), out int stok);

                        dt.Rows.Add(nama, merek, harga, stok);
                    }
                } while (reader.NextResult());
            }

            return dt;
        }

        // ════════════════════════════════════════════════════════
        // PROSES IMPORT — panggil sp_ImportProdukExcel PER BARIS
        // lewat DatabaseHelper (DAL)
        // ════════════════════════════════════════════════════════
        private void BtnImport_Click(object sender, EventArgs e)
        {
            if (_previewData == null || _previewData.Rows.Count == 0) return;

            btnImport.Enabled = false;
            btnPilihFile.Enabled = false;
            progressImport.Visible = true;
            progressImport.Minimum = 0;
            progressImport.Maximum = _previewData.Rows.Count;
            progressImport.Value = 0;

            int berhasil = 0, duplikat = 0, invalid = 0, errorLain = 0;

            foreach (DataRow row in _previewData.Rows)
            {
                string nama = row["Nama Produk"].ToString();
                string merek = row["Merek"].ToString();
                decimal harga = Convert.ToDecimal(row["Harga"]);
                int stok = Convert.ToInt32(row["Stok"]);

                try
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@id_admin", _idAdmin),
                        new SqlParameter("@nama_produk", nama),
                        new SqlParameter("@merek", merek),
                        new SqlParameter("@harga", harga),
                        new SqlParameter("@stok", stok)
                    };

                    int hasil = DatabaseHelper.ExecuteNonQueryWithResultCode(
                        "sp_ImportProdukExcel", parameters);

                    switch (hasil)
                    {
                        case 1: berhasil++; break;
                        case 0: duplikat++; break;
                        case -1: invalid++; break;
                        default: errorLain++; break;
                    }
                }
                catch
                {
                    errorLain++;
                }

                progressImport.Value++;
                Application.DoEvents(); // supaya progress bar terlihat update real-time
            }

            progressImport.Visible = false;
            btnPilihFile.Enabled = true;

            lblHasil.Text = $"Selesai: {berhasil} berhasil, {duplikat} duplikat (dilewati), " +
                            $"{invalid} data tidak valid, {errorLain} error lain.";
            lblHasil.ForeColor = berhasil > 0 ? Color.FromArgb(120, 220, 150) : Color.FromArgb(255, 120, 130);

            MessageBox.Show(
                $"Import selesai!\n\n" +
                $"Berhasil   : {berhasil}\n" +
                $"Duplikat   : {duplikat} (dilewati)\n" +
                $"Tidak valid: {invalid}\n" +
                $"Error lain : {errorLain}",
                "Hasil Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ════════════════════════════════════════════════════════
        // HELPERS VISUAL
        // ════════════════════════════════════════════════════════
        private static GraphicsPath RoundedPath(Rectangle r, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void ApplyRoundRegion(Control c, int radius)
        {
            if (c.Width <= 0 || c.Height <= 0) return;
            Rectangle r = new Rectangle(0, 0, c.Width, c.Height);
            c.Region = new Region(RoundedPath(r, radius));
        }
    }
}