using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public partial class FormAdminDashboard : Form
    {
        Panel header, sidebar, content;
        Label lblTitle, lblRole;
        Label lblNama, lblHarga, lblStok, lblJenis;
        Button btnDashboard, btnProduk, btnUser, btnLogout;
        Button btnAdd, btnUpdate, btnDelete;
        TextBox txtNamaProduk, txtHarga, txtStok, txtJenisProduk;
        DataGridView grid;

        string userRole;
        string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=DiecastDB;Integrated Security=True";

        public FormAdminDashboard(string role)
        {
            userRole = role;

            // 1. Panggil UI terlebih dahulu
            InitializeComponent();

            // 2. Gunakan pengecekan null agar tidak crash jika lblRole belum terbentuk
            if (lblRole != null) lblRole.Text = "ROLE: " + userRole.ToUpper();

            try
            {
                SetupAccess();
                LoadProduk();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Admin Dashboard";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // ================= HEADER =================
            header = new Panel() { Size = new Size(1000, 60), Dock = DockStyle.Top, BackColor = Color.DarkBlue };
            lblTitle = new Label() { Text = "ADMIN DASHBOARD", ForeColor = Color.White, Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
            lblRole = new Label() { ForeColor = Color.White, Location = new Point(800, 20), AutoSize = true };
            header.Controls.AddRange(new Control[] { lblTitle, lblRole });

            // ================= SIDEBAR =================
            sidebar = new Panel() { Size = new Size(200, 600), Dock = DockStyle.Left, BackColor = Color.LightGray };
            btnDashboard = CreateButton("Dashboard", 30);
            btnProduk = CreateButton("Produk", 80);
            btnUser = CreateButton("User", 130);
            btnLogout = CreateButton("Logout", 480);

            btnDashboard.Click += (s, e) => LoadProduk();
            btnProduk.Click += (s, e) => LoadProduk();
            btnUser.Click += BtnUser_Click;
            btnLogout.Click += BtnLogout_Click;
            sidebar.Controls.AddRange(new Control[] { btnDashboard, btnProduk, btnUser, btnLogout });

            // ================= CONTENT =================
            content = new Panel() { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke };
            lblNama = new Label() { Text = "Nama Produk", Location = new Point(20, 10), AutoSize = true };
            lblHarga = new Label() { Text = "Harga", Location = new Point(180, 10), AutoSize = true };
            lblStok = new Label() { Text = "Stok", Location = new Point(310, 10), AutoSize = true };
            lblJenis = new Label() { Text = "Jenis", Location = new Point(440, 10), AutoSize = true };

            txtNamaProduk = new TextBox() { Location = new Point(20, 30), Width = 150 };
            txtHarga = new TextBox() { Location = new Point(180, 30), Width = 120 };
            txtStok = new TextBox() { Location = new Point(310, 30), Width = 120 };
            txtJenisProduk = new TextBox() { Location = new Point(440, 30), Width = 120 };

            btnAdd = new Button() { Text = "Add", Location = new Point(580, 28), Width = 70 };
            btnUpdate = new Button() { Text = "Update", Location = new Point(660, 28), Width = 70 };
            btnDelete = new Button() { Text = "Delete", Location = new Point(740, 28), Width = 70 };

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;

            grid = new DataGridView() { Location = new Point(20, 80), Size = new Size(740, 420), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            grid.CellClick += Grid_CellClick;

            content.Controls.AddRange(new Control[] { lblNama, lblHarga, lblStok, lblJenis, txtNamaProduk, txtHarga, txtStok, txtJenisProduk, btnAdd, btnUpdate, btnDelete, grid });

            // Urutan penambahan control ke Form sangat penting
            this.Controls.Add(content);
            this.Controls.Add(sidebar);
            this.Controls.Add(header);
        }

        private void LoadProduk()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    // PERBAIKAN: Hanya ambil data yang is_deleted-nya nol
                    string query = "SELECT id_diecast, Nama_Diecast, Harga, Stok, Jenis FROM Produk WHERE is_deleted = 0";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    grid.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat data: " + ex.Message);
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                // CATATAN: Pastikan kolom di DB adalah Nama_Diecast atau NamaProduk? 
                // Di FormUser kamu pakai Nama_Diecast. Saya samakan di sini:
                string query = "INSERT INTO Produk (Nama_Diecast, Harga, Stok, Jenis) VALUES (@nama,@harga,@stok,@jenis)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nama", txtNamaProduk.Text);
                cmd.Parameters.AddWithValue("@harga", txtHarga.Text);
                cmd.Parameters.AddWithValue("@stok", txtStok.Text);
                cmd.Parameters.AddWithValue("@jenis", txtJenisProduk.Text);
                cmd.ExecuteNonQuery();

            }
            ClearForm();
            LoadProduk();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih baris di tabel terlebih dahulu!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    // Gunakan ID sebagai acuan (WHERE id_diecast = @id)
                    string query = "UPDATE Produk SET Nama_Diecast=@nama, Harga=@harga, Stok=@stok, Jenis=@jenis WHERE id_diecast=@id";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    // Ambil ID dari baris yang sedang dipilih di Grid
                    string idTerpilih = grid.SelectedRows[0].Cells[0].Value.ToString();

                    // Bersihkan format harga (hapus koma/titik jika ada agar menjadi angka murni)
                    string hargaBersih = txtHarga.Text.Replace(",", "").Replace(".", "");

                    cmd.Parameters.AddWithValue("@id", idTerpilih);
                    cmd.Parameters.AddWithValue("@nama", txtNamaProduk.Text);
                    cmd.Parameters.AddWithValue("@harga", decimal.Parse(hargaBersih)); // Konversi paksa ke decimal
                    cmd.Parameters.AddWithValue("@stok", int.Parse(txtStok.Text));    // Konversi paksa ke int
                    cmd.Parameters.AddWithValue("@jenis", txtJenisProduk.Text);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Data berhasil diupdate!");
                        ClearForm();
                        LoadProduk();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Update Error: " + ex.Message);
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNamaProduk.Text)) return;

            if (MessageBox.Show("Hapus produk ini?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        // Mengubah status menjadi terhapus
                        string sql = "UPDATE Produk SET is_deleted = 1 WHERE Nama_Diecast = @nama";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@nama", txtNamaProduk.Text);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Data berhasil dihapus dari tampilan!");

                        ClearForm();
                        LoadProduk(); // <--- INI SANGAT PENTING
                    }
                    catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
                }
            }
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = grid.Rows[e.RowIndex];

                // CEK: Apakah kolom "Nama_Diecast" ada di grid saat ini?
                if (grid.Columns.Contains("Nama_Diecast"))
                {
                    txtNamaProduk.Text = row.Cells["Nama_Diecast"].Value.ToString();
                    txtHarga.Text = row.Cells["Harga"].Value.ToString();
                    txtStok.Text = row.Cells["Stok"].Value.ToString();
                    txtJenisProduk.Text = row.Cells["Jenis"].Value.ToString();
                }
                else
                {
                    // Jika yang diklik adalah data User, kita bisa abaikan 
                    // atau kosongkan form agar tidak terjadi error.
                    ClearForm();
                }
            }
        }

        private void ClearForm()
        {
            txtNamaProduk.Clear();
            txtHarga.Clear();
            txtStok.Clear();
            txtJenisProduk.Clear();
        }

        private void BtnUser_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT Username, Email, Alamat, NoHP FROM Users", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                grid.DataSource = dt;
            }
        }

        private void SetupAccess()
        {
            if (userRole.ToLower() != "admin")
            {
                btnUser.Enabled = false;
                btnAdd.Enabled = btnUpdate.Enabled = btnDelete.Enabled = false;
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            // Cari Form1 yang di-hide atau buat baru
            Form loginForm = Application.OpenForms["Form1"];
            if (loginForm != null) loginForm.Show();
            else { Form1 f = new Form1(); f.Show(); }
            this.Close();
        }

        private Button CreateButton(string text, int top)
        {
            return new Button() { Text = text, Width = 180, Height = 40, Location = new Point(10, top), FlatStyle = FlatStyle.Flat, BackColor = Color.White };
        }
    }
}