using System;
using System.Collections.Generic;

namespace SistemPenjualanDiecastNew
{
    public class DataLaporanPenjualan
    {
        public string TanggalPesan { get; set; }
        public string NamaPembeli { get; set; }
        public string NamaProduk { get; set; }
        public int Qty { get; set; }
        public string HargaSatuan { get; set; }
        public string Subtotal { get; set; }
        public string StatusPesanan { get; set; }
    }

    public class ListLaporanPenjualan : List<DataLaporanPenjualan>
    {
        // kosong, hanya wrapper untuk Crystal Reports
    }
}