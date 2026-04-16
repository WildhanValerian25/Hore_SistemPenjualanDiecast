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

        