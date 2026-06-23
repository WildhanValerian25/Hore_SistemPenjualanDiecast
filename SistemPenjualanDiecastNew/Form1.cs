using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public partial class Form1 : Form
    {
        private Label lblTitle, lblSubtitle, lblUsername, lblPassword;
        private Label lblPhUser, lblPhPass;
        private TextBox txtUsername, txtPassword;
        private Button btnLogin;
        private LinkLabel linkRegister;
        private Panel pnlCard, pnlUserWrap, pnlPassWrap, pnlAccentBar;
        private Panel pnlLeft, pnlRight;  // ✅ Split layout kiri-kanan
        private Label lblBrand, lblTagline; // ✅ Branding di panel kiri

        private Color cBgDark = Color.FromArgb(8, 18, 38);
        private Color cBgMid = Color.FromArgb(11, 30, 62);
        private Color cCard = Color.FromArgb(14, 38, 78);
        private Color cCardBord = Color.FromArgb(25, 60, 110);
        private Color cAccent = Color.FromArgb(26, 86, 219);
        private Color cInputBg = Color.FromArgb(10, 26, 55);
        private Color cInputBrd = Color.FromArgb(40, 70, 130);
        private Color cTextPri = Color.FromArgb(230, 238, 255);
        private Color cTextMut = Color.FromArgb(100, 130, 180);

        public Form1()
        {
            InitializeComponent();
            ApplyRoundRegion(btnLogin, 8);
            linkRegister.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkRegister_LinkClicked);
        }

        private void InitializeComponent()
        {
            this.pnlLeft = new Panel();
            this.pnlRight = new Panel();
            this.pnlCard = new Panel();
            this.lblBrand = new Label();
            this.lblTagline = new Label();
            this.lblTitle = new Label();
            this.lblSubtitle = new Label();
            this.pnlAccentBar = new Panel();
            this.lblUsername = new Label();
            this.pnlUserWrap = new Panel();
            this.lblPhUser = new Label();
            this.txtUsername = new TextBox();
            this.lblPassword = new Label();
            this.pnlPassWrap = new Panel();
            this.lblPhPass = new Label();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.linkRegister = new LinkLabel();

            this.SuspendLayout();

            // ════════════════════════════════════════════════════
            // FORM — Full Screen
            // ════════════════════════════════════════════════════
            this.Text = "Login - Sistem Penjualan Diecast";
            this.WindowState = FormWindowState.Normal;       // ✅ Full screen
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = cBgDark;
            this.Font = new Font("Segoe UI", 9.5F);
            this.MaximizeBox = true;
            this.Paint += new PaintEventHandler(this.Form1_Paint);

            // ════════════════════════════════════════════════════
            // PANEL KIRI — Branding
            // ════════════════════════════════════════════════════
            pnlLeft = new Panel()
            {
                Dock = DockStyle.Left,
                Width = 500,           // akan di-resize saat form resize
                BackColor = Color.Transparent
            };
            pnlLeft.Paint += PnlLeft_Paint;

            // Logo / Brand
            lblBrand = new Label()
            {
                Text = "🚗 DIECAST\nSTORE",
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.None,
                AutoSize = false,
                Size = new Size(400, 120),
                Location = new Point(50, 180)
            };

            lblTagline = new Label()
            {
                Text = "Koleksi Diecast Terlengkap\nHarga Terbaik, Kualitas Terjamin",
                Font = new Font("Segoe UI", 13, FontStyle.Regular),
                ForeColor = Color.FromArgb(150, 190, 255),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Size = new Size(400, 80),
                Location = new Point(50, 320)
            };

            // Dekorasi titik-titik bawah
            Label lblDeco = new Label()
            {
                Text = "● ● ●",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(26, 86, 219),
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(195, 430)
            };

            pnlLeft.Controls.AddRange(new Control[] { lblBrand, lblTagline, lblDeco });

            // ════════════════════════════════════════════════════
            // PANEL KANAN — Form Login
            // ════════════════════════════════════════════════════
            pnlRight = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            pnlRight.Paint += PnlRight_Paint;

            // ════════════════════════════════════════════════════
            // CARD LOGIN — di tengah panel kanan
            // ════════════════════════════════════════════════════
            pnlCard = new Panel()
            {
                Size = new Size(420, 450),
                BackColor = cCard
            };
            pnlCard.Paint += new PaintEventHandler(this.PnlCard_Paint);

            // Judul
            lblTitle = new Label()
            {
                Text = "Selamat Datang",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(46, 38)
            };

            lblSubtitle = new Label()
            {
                Text = "Silakan masuk untuk melanjutkan",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = cTextMut,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(46, 80)
            };

            pnlAccentBar = new Panel()
            {
                BackColor = cAccent,
                Location = new Point(192, 112),
                Size = new Size(40, 3)
            };

            // Username
            lblUsername = new Label()
            {
                Text = "USERNAME",
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(138, 164, 207),
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(46, 134)
            };

            pnlUserWrap = new Panel()
            {
                BackColor = cInputBg,
                Location = new Point(46, 156),
                Size = new Size(328, 44)
            };
            pnlUserWrap.Paint += new PaintEventHandler(this.PnlUserWrap_Paint);

            lblPhUser = new Label()
            {
                Text = "Masukkan username",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(60, 90, 140),
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(16, 12),
                Enabled = false
            };

            txtUsername = new TextBox()
            {
                BackColor = cInputBg,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10.5F),
                ForeColor = cTextPri,
                Location = new Point(14, 11),
                Size = new Size(296, 24)
            };
            txtUsername.TextChanged += new EventHandler(this.TxtUsername_TextChanged);
            txtUsername.GotFocus += new EventHandler(this.TxtUsername_GotFocus);
            txtUsername.LostFocus += new EventHandler(this.TxtUsername_LostFocus);

            pnlUserWrap.Controls.AddRange(new Control[] { lblPhUser, txtUsername });

            // Password
            lblPassword = new Label()
            {
                Text = "PASSWORD",
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(138, 164, 207),
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(46, 218)
            };

            pnlPassWrap = new Panel()
            {
                BackColor = cInputBg,
                Location = new Point(46, 240),
                Size = new Size(328, 44)
            };
            pnlPassWrap.Paint += new PaintEventHandler(this.PnlPassWrap_Paint);

            lblPhPass = new Label()
            {
                Text = "Masukkan password",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(60, 90, 140),
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(16, 12),
                Enabled = false
            };

            txtPassword = new TextBox()
            {
                BackColor = cInputBg,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10.5F),
                ForeColor = cTextPri,
                Location = new Point(14, 11),
                Size = new Size(296, 24),
                UseSystemPasswordChar = true
            };
            txtPassword.TextChanged += new EventHandler(this.TxtPassword_TextChanged);
            txtPassword.GotFocus += new EventHandler(this.TxtPassword_GotFocus);
            txtPassword.LostFocus += new EventHandler(this.TxtPassword_LostFocus);
            txtPassword.KeyDown += new KeyEventHandler(this.TxtPassword_KeyDown);

            pnlPassWrap.Controls.AddRange(new Control[] { lblPhPass, txtPassword });

            // Tombol Login
            btnLogin = new Button()
            {
                Text = "Masuk",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = cAccent,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(46, 312),
                Size = new Size(328, 48),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(35, 100, 235);
            btnLogin.FlatAppearance.MouseDownBackColor = Color.FromArgb(15, 60, 170);
            btnLogin.Click += new EventHandler(this.btnLogin_Click);

            // Link Register
            linkRegister = new LinkLabel()
            {
                Text = "Belum punya akun?  Daftar sekarang",
                Font = new Font("Segoe UI", 9F),
                ForeColor = cTextMut,
                BackColor = Color.Transparent,
                LinkColor = Color.FromArgb(80, 140, 240),
                AutoSize = true,
                Location = new Point(83, 390),
                LinkArea = new LinkArea(18, 16),
                UseCompatibleTextRendering = true
            };

            pnlCard.Controls.AddRange(new Control[] {
                lblTitle, lblSubtitle, pnlAccentBar,
                lblUsername, pnlUserWrap,
                lblPassword, pnlPassWrap,
                btnLogin, linkRegister
            });

            // ✅ Card di tengah panel kanan — di-handle saat resize
            pnlRight.Controls.Add(pnlCard);
            pnlRight.Resize += (s, e) => CenterCard();

            this.Controls.Add(pnlRight);
            this.Controls.Add(pnlLeft);

            // ✅ Resize panel kiri proporsional
            this.Resize += (s, e) =>
            {
                pnlLeft.Width = this.ClientSize.Width / 2;
                CenterCard();
            };

            this.ResumeLayout(false);
        }

        // ✅ Center card di panel kanan
        private void CenterCard()
        {
            if (pnlCard == null || pnlRight == null) return;
            pnlCard.Location = new Point(
                (pnlRight.Width - pnlCard.Width) / 2,
                (pnlRight.Height - pnlCard.Height) / 2
            );
        }

        // ════════════════════════════════════════════════════════
        // PAINT HANDLERS
        // ════════════════════════════════════════════════════════
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush br = new LinearGradientBrush(
                this.ClientRectangle, cBgDark, cBgMid,
                LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(br, this.ClientRectangle);
            }
        }

        private void PnlLeft_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Gradient kiri — lebih gelap dengan sentuhan biru
            using (LinearGradientBrush br = new LinearGradientBrush(
                pnlLeft.ClientRectangle,
                Color.FromArgb(8, 18, 38),
                Color.FromArgb(15, 40, 90),
                LinearGradientMode.ForwardDiagonal))
            {
                g.FillRectangle(br, pnlLeft.ClientRectangle);
            }

            // Garis pemisah kanan
            using (Pen pen = new Pen(Color.FromArgb(25, 60, 110), 1))
                g.DrawLine(pen, pnlLeft.Width - 1, 0, pnlLeft.Width - 1, pnlLeft.Height);

            // Lingkaran dekorasi background
            using (SolidBrush br = new SolidBrush(Color.FromArgb(15, 26, 86, 219)))
            {
                g.FillEllipse(br, -80, -80, 300, 300);
                g.FillEllipse(br, pnlLeft.Width - 150, pnlLeft.Height - 150, 300, 300);
            }
        }

        private void PnlRight_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush br = new LinearGradientBrush(
                pnlRight.ClientRectangle,
                Color.FromArgb(10, 22, 46),
                Color.FromArgb(14, 35, 70),
                LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(br, pnlRight.ClientRectangle);
            }
        }

        private void PnlCard_Paint(object sender, PaintEventArgs e)
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
        }

        private void PnlUserWrap_Paint(object sender, PaintEventArgs e)
        {
            DrawRoundBorder(e.Graphics, pnlUserWrap, txtUsername.Focused);
        }

        private void PnlPassWrap_Paint(object sender, PaintEventArgs e)
        {
            DrawRoundBorder(e.Graphics, pnlPassWrap, txtPassword.Focused);
        }

        private void DrawRoundBorder(Graphics g, Panel p, bool focused)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle r = new Rectangle(0, 0, p.Width - 1, p.Height - 1);
            using (GraphicsPath path = RoundedPath(r, 8))
            {
                p.Region = new Region(path);
                using (SolidBrush bg = new SolidBrush(cInputBg))
                    g.FillPath(bg, path);
                Color borderColor = focused ? cAccent : cInputBrd;
                float borderWidth = focused ? 1.5f : 1f;
                using (Pen pen = new Pen(borderColor, borderWidth))
                    g.DrawPath(pen, path);
            }
        }

        // ════════════════════════════════════════════════════════
        // EVENT HANDLERS
        // ════════════════════════════════════════════════════════
        private void TxtUsername_GotFocus(object sender, EventArgs e)
        {
            pnlUserWrap.Invalidate();
            lblPhUser.Visible = false;
        }
        private void TxtUsername_LostFocus(object sender, EventArgs e)
        {
            pnlUserWrap.Invalidate();
            lblPhUser.Visible = string.IsNullOrEmpty(txtUsername.Text);
        }
        private void TxtUsername_TextChanged(object sender, EventArgs e)
        {
            lblPhUser.Visible = string.IsNullOrEmpty(txtUsername.Text);
        }
        private void TxtPassword_GotFocus(object sender, EventArgs e)
        {
            pnlPassWrap.Invalidate();
            lblPhPass.Visible = false;
        }
        private void TxtPassword_LostFocus(object sender, EventArgs e)
        {
            pnlPassWrap.Invalidate();
            lblPhPass.Visible = string.IsNullOrEmpty(txtPassword.Text);
        }
        private void TxtPassword_TextChanged(object sender, EventArgs e)
        {
            lblPhPass.Visible = string.IsNullOrEmpty(txtPassword.Text);
        }
        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnLogin_Click(btnLogin, EventArgs.Empty);
        }

        // ════════════════════════════════════════════════════════
        // HELPERS
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
            Rectangle r = new Rectangle(0, 0, c.Width, c.Height);
            c.Region = new Region(RoundedPath(r, radius));
        }

        // ════════════════════════════════════════════════════════
        // LOGIKA LOGIN
        // ════════════════════════════════════════════════════════
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == "" || txtPassword.Text == "")
            {
                MessageBox.Show("Username dan Password wajib diisi!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // Cek pelanggan dulu
                    SqlCommand cmdPelanggan = new SqlCommand("sp_LoginPelanggan", conn);
                    cmdPelanggan.CommandType = CommandType.StoredProcedure;
                    cmdPelanggan.Parameters.AddWithValue("@username", txtUsername.Text);
                    cmdPelanggan.Parameters.AddWithValue("@password", txtPassword.Text);
                    SqlDataReader reader = cmdPelanggan.ExecuteReader();

                    if (reader.Read())
                    {
                        string userLogin = reader["username"].ToString();
                        reader.Close();
                        FormUser userDashboard = new FormUser(userLogin);
                        userDashboard.Show();
                        this.Hide();
                    }
                    else
                    {
                        reader.Close();

                        // Cek admin
                        SqlCommand cmdAdmin = new SqlCommand("sp_LoginAdmin", conn);
                        cmdAdmin.CommandType = CommandType.StoredProcedure;
                        cmdAdmin.Parameters.AddWithValue("@username", txtUsername.Text);
                        cmdAdmin.Parameters.AddWithValue("@password", txtPassword.Text);
                        SqlDataReader readerAdmin = cmdAdmin.ExecuteReader();

                        if (readerAdmin.Read())
                        {
                            string adminLogin = readerAdmin["username"].ToString();
                            readerAdmin.Close();
                            FormAdminDashboard adminDashboard = new FormAdminDashboard(adminLogin);
                            adminDashboard.Show();
                            this.Hide();
                        }
                        else
                        {
                            readerAdmin.Close();
                            MessageBox.Show("Username atau Password salah!", "Akses Ditolak",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal terhubung ke database!\n" + ex.Message, "Error Koneksi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LinkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FormRegister r = new FormRegister();
            r.Show();
            this.Hide();
        }
    }
}