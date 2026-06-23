using System;
using System.Configuration;

namespace SistemPenjualanDiecastNew
{
    /// <summary>
    /// Sumber tunggal (single source of truth) untuk connection string
    /// database di seluruh aplikasi.
    ///
    /// KENAPA INI PENTING UNTUK DEPLOYMENT KE LAPTOP CLIENT:
    /// Sebelumnya, setiap form (FormAdminDashboard, FormUser, FormRegister,
    /// dst) menulis connection string sendiri-sendiri sebagai string
    /// literal hardcoded, contoh:
    ///
    ///   string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;...";
    ///
    /// Itu HANYA akan jalan di laptop kamu sendiri, karena nama SQL Server
    /// instance "LAPTOP-24A5CGHI\WILDHANFIGHT" itu spesifik untuk laptop
    /// kamu. Begitu aplikasi di-install di laptop client (dosen/asdos),
    /// nama instance SQL Server-nya pasti berbeda, sehingga semua form
    /// akan gagal connect ke database.
    ///
    /// Dengan class ini, connection string dibaca dari App.config
    /// (file App.exe.config setelah build/install). Artinya saat
    /// deploy ke laptop client, kamu HANYA perlu edit satu baris di
    /// App.exe.config — tidak perlu compile ulang aplikasi.
    /// </summary>
    public static class Koneksi
    {
        // Fallback default — dipakai HANYA jika App.config tidak ditemukan
        // atau key-nya belum diisi. Untuk development di laptop kamu sendiri.
        private const string DEFAULT_CONN_STRING =
            @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

        /// <summary>
        /// Mengambil connection string aktif. Urutan prioritas:
        /// 1. App.config -> connectionStrings -> name="DbDiecast"
        /// 2. Fallback ke DEFAULT_CONN_STRING jika App.config tidak ada/kosong
        /// </summary>
        public static string GetConnectionString()
        {
            try
            {
                var setting = ConfigurationManager.ConnectionStrings["DbDiecast"];
                if (setting != null && !string.IsNullOrWhiteSpace(setting.ConnectionString))
                {
                    return setting.ConnectionString;
                }
            }
            catch
            {
                // Jika App.config rusak/tidak terbaca, jatuh ke default di bawah
            }

            return DEFAULT_CONN_STRING;
        }
    }
}