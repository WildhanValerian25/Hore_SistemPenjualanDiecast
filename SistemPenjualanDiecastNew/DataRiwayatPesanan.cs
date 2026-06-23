using System.Collections.Generic;

namespace SistemPenjualanDiecastNew
{
    /// <summary>
    /// Representasi 1 baris data riwayat pesanan, dipakai sebagai
    /// DataSource untuk Crystal Report (LaporanRiwayatPesanan.rpt).
    ///
    /// PENTING: HargaSatuan dan Subtotal sengaja bertipe decimal
    /// (bukan string "Rp 45.000" seperti sebelumnya), supaya Crystal
    /// Reports bisa melakukan operasi Summary (Sum) untuk menghitung
    /// total keseluruhan di Report Footer.
    ///
    /// Formatting tampilan "Rp" dan pemisah ribuan dilakukan di
    /// Crystal Reports Designer (klik field -> Format Field ->
    /// Number -> Customize, atau pakai Format Currency bawaan),
    /// BUKAN di sini atau di SQL. Ini memisahkan data mentah dari
    /// presentasi, dan field jadi bisa di-Sum.
    /// </summary>
    public class DataRiwayatPesanan
    {
        public string TanggalPesan { get; set; }
        public string NamaProduk { get; set; }
        public int Qty { get; set; }
        public decimal HargaSatuan { get; set; }
        public decimal Subtotal { get; set; }
        public string MetodeBayar { get; set; }
        public string StatusBayar { get; set; }
        public string StatusPesanan { get; set; }
        public string NomorResi { get; set; }
    }

    /// <summary>
    /// Kumpulan baris DataRiwayatPesanan, dipakai sebagai DataSource
    /// di report.SetDataSource(listData).
    /// </summary>
    public class ListRiwayatPesanan : List<DataRiwayatPesanan>
    {
    }
}