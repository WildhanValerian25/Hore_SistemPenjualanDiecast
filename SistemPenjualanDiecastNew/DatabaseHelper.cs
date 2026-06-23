using System;
using System.Data;
using System.Data.SqlClient;

namespace SistemPenjualanDiecastNew
{
    /// <summary>
    /// Data Access Layer (DAL) generik untuk project ini.
    ///
    /// TUJUAN:
    /// Memisahkan kode akses database (SqlConnection, SqlCommand, dst)
    /// dari kode UI (Form). Form cukup memanggil method di sini,
    /// tanpa perlu menulis ulang boilerplate koneksi setiap kali.
    ///
    /// CATATAN PENTING UNTUK PROJECT INI:
    /// Form-form yang SUDAH ADA (FormAdminDashboard, FormUser,
    /// FormAdminPembayaran, FormRiwayatPesanan, dst) TIDAK diubah
    /// untuk memakai class ini -- mereka sudah berjalan dengan pola
    /// akses database manual (inline SqlConnection/SqlCommand) dan
    /// sudah teruji. Mengubahnya sekarang berisiko menimbulkan bug
    /// baru di fitur yang sudah jalan.
    ///
    /// Class ini dipakai HANYA untuk fitur-fitur BARU yang belum
    /// dibuat (misalnya Import Excel, atau form baru lainnya),
    /// supaya kode baru lebih rapi tanpa mengganggu kode lama.
    ///
    /// SEMUA method di sini mengasumsikan target adalah Stored
    /// Procedure (CommandType.StoredProcedure), karena seluruh
    /// akses database di project ini sudah dirancang lewat SP
    /// (bukan raw SQL query), sesuai pola yang sudah dipakai di
    /// form-form sebelumnya.
    /// </summary>
    public static class DatabaseHelper
    {
        /// <summary>
        /// Menjalankan Stored Procedure dan mengembalikan hasilnya
        /// sebagai DataTable. Cocok untuk SP yang mengembalikan
        /// banyak baris (SELECT), misalnya untuk mengisi DataGridView.
        /// </summary>
        /// <param name="namaSp">Nama Stored Procedure, contoh: "sp_GetProdukAktif"</param>
        /// <param name="parameters">
        /// Parameter SP, atau null jika SP tidak butuh parameter.
        /// Contoh: new SqlParameter[] { new SqlParameter("@keyword", keyword) }
        /// </param>
        public static DataTable ExecuteDataTable(string namaSp, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(Koneksi.GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(namaSp, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                return dt;
            }
        }

        /// <summary>
        /// Menjalankan Stored Procedure dan mengembalikan satu nilai
        /// saja (baris pertama, kolom pertama). Cocok untuk SP yang
        /// melakukan SELECT COUNT(*), SELECT MAX(...), dsb.
        /// </summary>
        public static object ExecuteScalar(string namaSp, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(Koneksi.GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(namaSp, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Menjalankan Stored Procedure yang tidak mengembalikan hasil
        /// (INSERT/UPDATE/DELETE biasa, tanpa output parameter).
        /// Mengembalikan jumlah baris yang terdampak.
        /// </summary>
        public static int ExecuteNonQuery(string namaSp, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(Koneksi.GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(namaSp, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Menjalankan Stored Procedure yang punya OUTPUT parameter
        /// bernama "@hasil" bertipe INT -- pola yang sudah konsisten
        /// dipakai di banyak SP project ini (sp_InsertProduk,
        /// sp_RegisterPelanggan, sp_KonfirmasiLunas, dst).
        ///
        /// Mengembalikan nilai @hasil setelah SP dieksekusi.
        ///
        /// CONTOH PAKAI:
        ///   var p = new[] {
        ///       new SqlParameter("@nama_produk", txtNama.Text),
        ///       new SqlParameter("@harga", harga)
        ///   };
        ///   int hasil = DatabaseHelper.ExecuteNonQueryWithResultCode("sp_InsertProduk", p);
        ///   if (hasil == 1) { /* berhasil */ }
        /// </summary>
        /// <param name="namaSp">Nama Stored Procedure</param>
        /// <param name="parameters">
        /// Parameter input SP (TIDAK termasuk parameter @hasil --
        /// itu ditambahkan otomatis oleh method ini)
        /// </param>
        /// <param name="namaParameterHasil">
        /// Nama parameter output, default "@hasil" sesuai konvensi
        /// SP yang sudah ada di project ini. Ganti jika SP memakai
        /// nama lain (misal "@berhasil" di sp_LoginPelanggan).
        /// </param>
        public static int ExecuteNonQueryWithResultCode(
            string namaSp,
            SqlParameter[] parameters,
            string namaParameterHasil = "@hasil")
        {
            using (SqlConnection conn = new SqlConnection(Koneksi.GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(namaSp, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                SqlParameter pHasil = new SqlParameter(namaParameterHasil, SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(pHasil);

                conn.Open();
                cmd.ExecuteNonQuery();

                return Convert.ToInt32(pHasil.Value);
            }
        }

        /// <summary>
        /// Menjalankan Stored Procedure yang punya SATU OUTPUT
        /// parameter bertipe apapun (bukan cuma INT @hasil),
        /// misalnya untuk sp_CountProduk yang outputnya @Total INT,
        /// atau SP lain dengan nama/tipe output custom.
        ///
        /// CONTOH PAKAI:
        ///   object totalObj = DatabaseHelper.ExecuteScalarOutput(
        ///       "sp_CountProduk", "@Total", SqlDbType.Int);
        ///   int total = Convert.ToInt32(totalObj);
        /// </summary>
        public static object ExecuteScalarOutput(
            string namaSp,
            string namaParameterOutput,
            SqlDbType tipeOutput,
            params SqlParameter[] parametersInput)
        {
            using (SqlConnection conn = new SqlConnection(Koneksi.GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(namaSp, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parametersInput != null)
                    cmd.Parameters.AddRange(parametersInput);

                SqlParameter pOutput = new SqlParameter(namaParameterOutput, tipeOutput)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(pOutput);

                conn.Open();
                cmd.ExecuteNonQuery();

                return pOutput.Value;
            }
        }

        /// <summary>
        /// Membuka SqlConnection baru. Dipakai HANYA jika sebuah
        /// operasi butuh mengontrol koneksi/transaksi sendiri secara
        /// manual (misalnya beberapa SqlCommand dalam satu
        /// SqlTransaction) -- untuk kasus sederhana, pakai method
        /// di atas (ExecuteDataTable/ExecuteScalar/dst) saja.
        ///
        /// Pemanggil WAJIB membungkus hasil method ini dengan
        /// "using" supaya koneksi tertutup dengan benar:
        ///
        ///   using (SqlConnection conn = DatabaseHelper.BukaKoneksi())
        ///   {
        ///       conn.Open();
        ///       ...
        ///   }
        /// </summary>
        public static SqlConnection BukaKoneksi()
        {
            return new SqlConnection(Koneksi.GetConnectionString());
        }
    }
}   