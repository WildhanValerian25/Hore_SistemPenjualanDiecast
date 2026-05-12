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

        bool isDashboardActive = false;
        string userRole;
        string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

        public FormAdminDashboard(string role)
        {
            userRole = role;
            InitializeComponent();
            if (lblRole != null) lblRole.Text = "ROLE: " + userRole.ToUpper();

            try
            {
                SetupAccess();
                LoadDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Admin Dashboard";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // HEADER
            header = new Panel() { Size = new Size(1000, 60), Dock = DockStyle.Top, BackColor = Color.DarkBlue };
            lblTitle = new Label() { Text = "ADMIN DASHBOARD", ForeColor = Color.White, Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
            lblRole = new Label() { ForeColor = Color.White, Location = new Point(800, 20), AutoSize = true };
            header.Controls.AddRange(new Control[] { lblTitle, lblRole });

            // SIDEBAR
            sidebar = new Panel() { Size = new Size(200, 600), Dock = DockStyle.Left, BackColor = Color.LightGray };

            btnDashboard = CreateButton("Dashboard", 30);
            btnProduk = CreateButton("Produk", 80);

            Button btnPembayaran = CreateButton("Pembayaran", 130);
            btnPembayaran.Click += (s, e) =>
            {
                FormAdminPembayaran f = new FormAdminPembayaran();
                f.ShowDialog();
                if (isDashboardActive) LoadDashboard();
            };

            btnUser = CreateButton("User", 180);
            btnLogout = CreateButton("Logout", 530);

            // Tombol Test Koneksi
            Button btnConnect = CreateButton("Test Koneksi", 230);
            btnConnect.BackColor = Color.LightGreen;
            btnConnect.Click += BtnConnect_Click;

            // Tombol Reset Data
            Button btnResetData = CreateButton("Reset Data", 280);
            btnResetData.BackColor = Color.LightYellow;
            btnResetData.Click += BtnResetData_Click;

            // Tombol Test SQL Injection
            Button btnTestInjection = CreateButton("Test Injection", 330);
            btnTestInjection.BackColor = Color.LightCoral;
            btnTestInjection.Click += BtnTestInjection_Click;

            btnDashboard.Click += (s, e) => { isDashboardActive = true; LoadDashboard(); };
            btnProduk.Click += (s, e) => { isDashboardActive = false; RebuildContentArea(); LoadProduk(); };
            btnUser.Click += (s, e) => { isDashboardActive = false; RebuildContentAreaUser(); BtnUser_Click(s, e); };
            btnLogout.Click += BtnLogout_Click;

            sidebar.Controls.AddRange(new Control[] {
                btnDashboard, btnProduk, btnPembayaran, btnUser,
                btnConnect, btnResetData, btnTestInjection,
                btnLogout
            });

            // CONTENT
            content = new Panel() { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke };
            lblNama = new Label() { Text = "Nama Produk", Location = new Point(20, 10), AutoSize = true };
            lblHarga = new Label() { Text = "Harga", Location = new Point(180, 10), AutoSize = true };
            lblStok = new Label() { Text = "Stok", Location = new Point(310, 10), AutoSize = true };
            lblJenis = new Label() { Text = "Merek", Location = new Point(440, 10), AutoSize = true };

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

            grid = new DataGridView()
            {
                Location = new Point(20, 80),
                Size = new Size(740, 420),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false
            };
            grid.CellClick += Grid_CellClick;

            content.Controls.AddRange(new Control[] {
                lblNama, lblHarga, lblStok, lblJenis,
                txtNamaProduk, txtHarga, txtStok, txtJenisProduk,
                btnAdd, btnUpdate, btnDelete, grid
            });

            this.Controls.Add(content);
            this.Controls.Add(sidebar);
            this.Controls.Add(header);
        }

       