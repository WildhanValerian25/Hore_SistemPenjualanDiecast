using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public partial class FormStrukPembelian : Form
    {
        private CrystalReportViewer crystalViewer;
        private int _idPesanan;

        // Sebaiknya pakai helper Koneksi.GetConnectionString() setelah refactor
        string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

        public FormStrukPembelian(int idPesanan)
        {
            _idPesanan = idPesanan;
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Struk Pembelian";
            this.Size = new System.Drawing.Size(650, 750);
            this.StartPosition = FormStartPosition.CenterParent;

            Button btnCetak = new Button() { Text = "Print", Dock = DockStyle.Top, Height = 35 };
            btnCetak.Click += (s, e) => crystalViewer?.PrintReport();

            crystalViewer = new CrystalReportViewer() { Dock = DockStyle.Fill };

            this.Controls.Add(crystalViewer);
            this.Controls.Add(btnCetak);

            this.Load += FormStrukPembelian_Load;
        }

        private void FormStrukPembelian_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable dtHeader = AmbilHeaderPesanan(_idPesanan);
                dtHeader.TableName = "dtStrukHeader";

                DataTable dtDetail = AmbilDetailPesanan(_idPesanan);
                dtDetail.TableName = "dtStrukDetail";

                ReportDocument report = new ReportDocument();
                report.Load(System.IO.Path.Combine(Application.StartupPath, "Reports", "rptStrukPembelian.rpt"));

                report.SetDataSource(dtHeader);

                if (report.Subreports.Count > 0)
                {
                    foreach (ReportDocument sub in report.Subreports)
                    {
                        sub.SetDataSource(dtDetail);
                    }
                }

                crystalViewer.ReportSource = report;
                crystalViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat struk: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Memanggil SP sp_GetStrukHeader untuk mengambil info pembeli dan pembayaran.
        /// </summary>
        private DataTable AmbilHeaderPesanan(int idPesanan)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("sp_GetStrukHeader", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@idPesanan", SqlDbType.Int).Value = idPesanan;

                conn.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        /// <summary>
        /// Memanggil SP sp_GetStrukDetail untuk mengambil daftar produk pada pesanan.
        /// </summary>
        private DataTable AmbilDetailPesanan(int idPesanan)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("sp_GetStrukDetail", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@idPesanan", SqlDbType.Int).Value = idPesanan;

                conn.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }
    }
}