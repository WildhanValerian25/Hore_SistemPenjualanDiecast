using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SistemPenjualanDiecastNew
{
    public partial class FormRegister : Form
    {
        // Kontrol
        private Label lblTitle, lblSubtitle;
        private Panel pnlAccentBar;
        private Label lblNama, lblUsername, lblPassword, lblConfirm, lblEmail, lblAlamat, lblNoHP;
        private Label lblPhNama, lblPhUser, lblPhPass, lblPhConfirm, lblPhEmail, lblPhAlamat, lblPhNoHP;
        private TextBox txtNama, txtUsername, txtPassword, txtConfirm, txtEmail, txtAlamat, txtNoHP;
        private Panel pnlCard;
        private Panel pnlNamaWrap, pnlUserWrap, pnlPassWrap, pnlConfirmWrap;
        private Panel pnlEmailWrap, pnlAlamatWrap, pnlNoHPWrap;
        private Button btnRegister, btnBack;

        // Warna (disamakan dengan FormLogin)
        private readonly Color cBgDark = Color.FromArgb(8, 18, 38);
        private readonly Color cBgMid = Color.FromArgb(11, 30, 62);
        private readonly Color cCard = Color.FromArgb(14, 38, 78);
        private readonly Color cCardBord = Color.FromArgb(25, 60, 110);
        private readonly Color cAccent = Color.FromArgb(26, 86, 219);
        private readonly Color cInputBg = Color.FromArgb(10, 26, 55);
        private readonly Color cInputBrd = Color.FromArgb(40, 70, 130);
        private readonly Color cTextPri = Color.FromArgb(230, 238, 255);
        private readonly Color cTextMut = Color.FromArgb(100, 130, 180);

        public FormRegister()
        {
            InitializeComponent();

            // BuildField TIDAK boleh dipanggil dari dalam InitializeComponent()
            // karena WinForms Designer mem-parsing method itu secara statis
            // dan akan mencoba mencari "BuildField" sebagai method bawaan Form.
            // Jadi semua field dibangun di sini, setelah InitializeComponent selesai.
            BuildAllFields();

            ApplyRoundRegion(btnRegister, 8);
            ApplyRoundRegion(btnBack, 8);
        }

        // ════════════════════════════════════════════════════════
        // InitializeComponent — HANYA deklarasi kontrol dasar.
        // Tidak ada pemanggilan method custom di sini supaya
        // designer (.cs [Design]) tetap bisa dibuka tanpa error.
        // ════════════════════════════════════════════════════════
        private void InitializeComponent()
        {
            this.pnlCard = new Panel();
            this.pnlAccentBar = new Panel();
            this.lblTitle = new Label();
            this.lblSubtitle = new Label();

            this.lblNama = new Label();
            this.lblUsername = new Label();
            this.lblPassword = new Label();
            this.lblConfirm = new Label();
            this.lblEmail = new Label();
            this.lblAlamat = new Label();
            this.lblNoHP = new Label();

            this.lblPhNama = new Label();
            this.lblPhUser = new Label();
            this.lblPhPass = new Label();
            this.lblPhConfirm = new Label();
            this.lblPhEmail = new Label();
            this.lblPhAlamat = new Label();
            this.lblPhNoHP = new Label();

            this.txtNama = new TextBox();
            this.txtUsername = new TextBox();
            this.txtPassword = new TextBox();
            this.txtConfirm = new TextBox();
            this.txtEmail = new TextBox();
            this.txtAlamat = new TextBox();
            this.txtNoHP = new TextBox();

            this.pnlNamaWrap = new Panel();
            this.pnlUserWrap = new Panel();
            this.pnlPassWrap = new Panel();
            this.pnlConfirmWrap = new Panel();
            this.pnlEmailWrap = new Panel();
            this.pnlAlamatWrap = new Panel();
            this.pnlNoHPWrap = new Panel();

            this.btnRegister = new Button();
            this.btnBack = new Button();

            this.SuspendLayout();

            // ── FORM ─────────────────────────────────────────────
            this.Text = "Register - Sistem Penjualan Diecast";
            this.ClientSize = new Size(560, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(8, 18, 38);
            this.Font = new Font("Segoe UI", 9.5F);
            this.Name = "FormRegister";
            this.Paint += new PaintEventHandler(this.FormRegister_Paint);

            // ── CARD ─────────────────────────────────────────────
            this.pnlCard.Size = new Size(460, 636);
            this.pnlCard.Location = new Point(50, 32);
            this.pnlCard.BackColor = Color.FromArgb(14, 38, 78);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Paint += new PaintEventHandler(this.PnlCard_Paint);

            // ── JUDUL ─────────────────────────────────────────────
            this.lblTitle.Text = "Buat Akun Baru";
            this.lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = Color.Transparent;
            this.lblTitle.Name = "lblTitle";

            this.lblSubtitle.Text = "Isi data diri kamu untuk mendaftar";
            this.lblSubtitle.Font = new Font("Segoe UI", 9F);
            this.lblSubtitle.ForeColor = Color.FromArgb(100, 130, 180);
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.BackColor = Color.Transparent;
            this.lblSubtitle.Name = "lblSubtitle";

            this.pnlAccentBar.Size = new Size(40, 3);
            this.pnlAccentBar.BackColor = Color.FromArgb(26, 86, 219);
            this.pnlAccentBar.Name = "pnlAccentBar";

            // ── BUTTON DAFTAR ─────────────────────────────────────
            this.btnRegister.Text = "Daftar";
            this.btnRegister.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            this.btnRegister.ForeColor = Color.White;
            this.btnRegister.BackColor = Color.FromArgb(26, 86, 219);
            this.btnRegister.FlatStyle = FlatStyle.Flat;
            this.btnRegister.FlatAppearance.BorderSize = 0;
            this.btnRegister.FlatAppearance.MouseOverBackColor = Color.FromArgb(35, 100, 235);
            this.btnRegister.FlatAppearance.MouseDownBackColor = Color.FromArgb(15, 60, 170);
            this.btnRegister.Size = new Size(176, 44);
            this.btnRegister.TabIndex = 10;
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Cursor = Cursors.Hand;
            this.btnRegister.Click += new EventHandler(this.btnRegister_Click);

            // ── BUTTON KEMBALI ────────────────────────────────────
            this.btnBack.Text = "Kembali";
            this.btnBack.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            this.btnBack.ForeColor = Color.FromArgb(150, 180, 230);
            this.btnBack.BackColor = Color.FromArgb(20, 50, 100);
            this.btnBack.FlatStyle = FlatStyle.Flat;
            this.btnBack.FlatAppearance.BorderSize = 1;
            this.btnBack.FlatAppearance.BorderColor = Color.FromArgb(40, 80, 150);
            this.btnBack.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 65, 120);
            this.btnBack.Size = new Size(176, 44);
            this.btnBack.TabIndex = 11;
            this.btnBack.Name = "btnBack";
            this.btnBack.Cursor = Cursors.Hand;
            this.btnBack.Click += new EventHandler(this.btnBack_Click);

            // ── SUSUN KE CARD (kontrol field ditambahkan nanti di BuildAllFields) ──
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.lblSubtitle);
            this.pnlCard.Controls.Add(this.pnlAccentBar);
            this.pnlCard.Controls.Add(this.btnRegister);
            this.pnlCard.Controls.Add(this.btnBack);

            this.Controls.Add(this.pnlCard);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // ════════════════════════════════════════════════════════
        // Membangun semua baris field + menata posisi card/judul/tombol.
        // Dipanggil dari constructor (BUKAN dari InitializeComponent).
        // ════════════════════════════════════════════════════════
        private void BuildAllFields()
        {
            int cardWidth = pnlCard.Width;
            int marginX = 30;
            int fieldWidth = cardWidth - (marginX * 2);

            // Posisi judul & subtitle (center horizontal di card)
            lblTitle.Location = new Point((cardWidth - lblTitle.PreferredWidth) / 2, 28);
            lblSubtitle.Location = new Point((cardWidth - lblSubtitle.PreferredWidth) / 2, 62);
            pnlAccentBar.Location = new Point((cardWidth - pnlAccentBar.Width) / 2, 92);

            int y = 116;          // posisi awal baris field pertama
            int rowGap = 64;      // jarak antar baris field (label + textbox)

            y = AddField("NAMA LENGKAP", "Masukkan nama lengkap", marginX, y, fieldWidth, false,
                ref lblNama, ref lblPhNama, ref txtNama, ref pnlNamaWrap, 0,
                TxtNama_GotFocus, TxtNama_LostFocus, TxtNama_TextChanged, rowGap);

            y = AddField("USERNAME", "Masukkan username", marginX, y, fieldWidth, false,
                ref lblUsername, ref lblPhUser, ref txtUsername, ref pnlUserWrap, 1,
                TxtUser_GotFocus, TxtUser_LostFocus, TxtUser_TextChanged, rowGap);

            y = AddField("PASSWORD", "Minimal 8 karakter", marginX, y, fieldWidth, true,
                ref lblPassword, ref lblPhPass, ref txtPassword, ref pnlPassWrap, 2,
                TxtPass_GotFocus, TxtPass_LostFocus, TxtPass_TextChanged, rowGap);

            y = AddField("KONFIRMASI PASSWORD", "Ulangi password", marginX, y, fieldWidth, true,
                ref lblConfirm, ref lblPhConfirm, ref txtConfirm, ref pnlConfirmWrap, 3,
                TxtConfirm_GotFocus, TxtConfirm_LostFocus, TxtConfirm_TextChanged, rowGap);

            y = AddField("EMAIL", "contoh@email.com", marginX, y, fieldWidth, false,
                ref lblEmail, ref lblPhEmail, ref txtEmail, ref pnlEmailWrap, 4,
                TxtEmail_GotFocus, TxtEmail_LostFocus, TxtEmail_TextChanged, rowGap);

            y = AddField("ALAMAT", "Masukkan alamat lengkap", marginX, y, fieldWidth, false,
                ref lblAlamat, ref lblPhAlamat, ref txtAlamat, ref pnlAlamatWrap, 5,
                TxtAlamat_GotFocus, TxtAlamat_LostFocus, TxtAlamat_TextChanged, rowGap);

            y = AddField("NO HP", "Contoh: 08123456789", marginX, y, fieldWidth, false,
                ref lblNoHP, ref lblPhNoHP, ref txtNoHP, ref pnlNoHPWrap, 6,
                TxtNoHP_GotFocus, TxtNoHP_LostFocus, TxtNoHP_TextChanged, rowGap);

            // Tombol diletakkan setelah baris field terakhir, berdampingan
            int buttonsY = y + 14;
            btnRegister.Location = new Point(marginX, buttonsY);
            btnBack.Location = new Point(cardWidth - marginX - btnBack.Width, buttonsY);
        }

        // ════════════════════════════════════════════════════════
        // Helper: buat satu baris field (label + wrapper + textbox + placeholder)
        // Mengembalikan posisi Y untuk baris berikutnya.
        // ════════════════════════════════════════════════════════
        private int AddField(
            string labelText, string placeholder, int x, int y, int width, bool isPassword,
            ref Label lbl, ref Label lblPh, ref TextBox txt, ref Panel wrap, int tabIndex,
            EventHandler onGot, EventHandler onLost, EventHandler onChanged, int rowGap)
        {
            lbl.Text = labelText;
            lbl.Font = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(138, 164, 207);
            lbl.AutoSize = true;
            lbl.BackColor = Color.Transparent;
            lbl.Location = new Point(x, y);

            wrap.Size = new Size(width, 42);
            wrap.Location = new Point(x, y + 20);
            wrap.BackColor = cInputBg;

            txt.BorderStyle = BorderStyle.None;
            txt.Font = new Font("Segoe UI", 10.5f);
            txt.ForeColor = cTextPri;
            txt.BackColor = cInputBg;
            txt.Size = new Size(width - 28, 22);
            txt.Location = new Point(14, 10);
            txt.TabIndex = tabIndex;
            if (isPassword) txt.UseSystemPasswordChar = true;
            txt.GotFocus += onGot;
            txt.LostFocus += onLost;
            txt.TextChanged += onChanged;

            lblPh.Text = placeholder;
            lblPh.Font = new Font("Segoe UI", 10f);
            lblPh.ForeColor = Color.FromArgb(60, 90, 140);
            lblPh.BackColor = Color.Transparent;
            lblPh.AutoSize = true;
            lblPh.Location = new Point(16, 11);
            lblPh.Enabled = false;

            wrap.Controls.Add(lblPh);
            wrap.Controls.Add(txt);
            txt.BringToFront();

            Panel capturedWrap = wrap;
            TextBox capturedTxt = txt;
            wrap.Paint += delegate (object s, PaintEventArgs e)
            {
                DrawRoundBorder(e.Graphics, capturedWrap, capturedTxt.Focused);
            };

            pnlCard.Controls.Add(lbl);
            pnlCard.Controls.Add(wrap);

            return y + rowGap;
        }

        // ════════════════════════════════════════════════════════
        // EVENT HANDLERS — Nama
        // ════════════════════════════════════════════════════════
        private void TxtNama_GotFocus(object sender, EventArgs e) { pnlNamaWrap.Invalidate(); lblPhNama.Visible = false; }
        private void TxtNama_LostFocus(object sender, EventArgs e) { pnlNamaWrap.Invalidate(); lblPhNama.Visible = string.IsNullOrEmpty(txtNama.Text); }
        private void TxtNama_TextChanged(object sender, EventArgs e) { lblPhNama.Visible = string.IsNullOrEmpty(txtNama.Text); }

        // USERNAME
        private void TxtUser_GotFocus(object sender, EventArgs e) { pnlUserWrap.Invalidate(); lblPhUser.Visible = false; }
        private void TxtUser_LostFocus(object sender, EventArgs e) { pnlUserWrap.Invalidate(); lblPhUser.Visible = string.IsNullOrEmpty(txtUsername.Text); }
        private void TxtUser_TextChanged(object sender, EventArgs e) { lblPhUser.Visible = string.IsNullOrEmpty(txtUsername.Text); }

        // PASSWORD
        private void TxtPass_GotFocus(object sender, EventArgs e) { pnlPassWrap.Invalidate(); lblPhPass.Visible = false; }
        private void TxtPass_LostFocus(object sender, EventArgs e) { pnlPassWrap.Invalidate(); lblPhPass.Visible = string.IsNullOrEmpty(txtPassword.Text); }
        private void TxtPass_TextChanged(object sender, EventArgs e) { lblPhPass.Visible = string.IsNullOrEmpty(txtPassword.Text); }

        // KONFIRMASI
        private void TxtConfirm_GotFocus(object sender, EventArgs e) { pnlConfirmWrap.Invalidate(); lblPhConfirm.Visible = false; }
        private void TxtConfirm_LostFocus(object sender, EventArgs e) { pnlConfirmWrap.Invalidate(); lblPhConfirm.Visible = string.IsNullOrEmpty(txtConfirm.Text); }
        private void TxtConfirm_TextChanged(object sender, EventArgs e) { lblPhConfirm.Visible = string.IsNullOrEmpty(txtConfirm.Text); }

        // EMAIL
        private void TxtEmail_GotFocus(object sender, EventArgs e) { pnlEmailWrap.Invalidate(); lblPhEmail.Visible = false; }
        private void TxtEmail_LostFocus(object sender, EventArgs e) { pnlEmailWrap.Invalidate(); lblPhEmail.Visible = string.IsNullOrEmpty(txtEmail.Text); }
        private void TxtEmail_TextChanged(object sender, EventArgs e) { lblPhEmail.Visible = string.IsNullOrEmpty(txtEmail.Text); }

        // ALAMAT
        private void TxtAlamat_GotFocus(object sender, EventArgs e) { pnlAlamatWrap.Invalidate(); lblPhAlamat.Visible = false; }
        private void TxtAlamat_LostFocus(object sender, EventArgs e) { pnlAlamatWrap.Invalidate(); lblPhAlamat.Visible = string.IsNullOrEmpty(txtAlamat.Text); }
        private void TxtAlamat_TextChanged(object sender, EventArgs e) { lblPhAlamat.Visible = string.IsNullOrEmpty(txtAlamat.Text); }

        // NO HP
        private void TxtNoHP_GotFocus(object sender, EventArgs e) { pnlNoHPWrap.Invalidate(); lblPhNoHP.Visible = false; }
        private void TxtNoHP_LostFocus(object sender, EventArgs e) { pnlNoHPWrap.Invalidate(); lblPhNoHP.Visible = string.IsNullOrEmpty(txtNoHP.Text); }
        private void TxtNoHP_TextChanged(object sender, EventArgs e) { lblPhNoHP.Visible = string.IsNullOrEmpty(txtNoHP.Text); }

        // ════════════════════════════════════════════════════════
        // PAINT HANDLERS
        // ════════════════════════════════════════════════════════
        private void FormRegister_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush br = new LinearGradientBrush(
                this.ClientRectangle, cBgDark, cBgMid,
                LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(br, this.ClientRectangle);
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
        // LOGIKA REGISTER — tidak diubah dari versi asli
        // ════════════════════════════════════════════════════════
        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (txtNama.Text == "" || txtUsername.Text == "" ||
                txtPassword.Text == "" || txtConfirm.Text == "" ||
                txtEmail.Text == "" || txtAlamat.Text == "" ||
                txtNoHP.Text == "")
            {
                MessageBox.Show("Semua field wajib diisi!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtUsername.Text.Contains(" "))
            {
                MessageBox.Show("Username tidak boleh mengandung spasi!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
            {
                MessageBox.Show("Format email tidak valid! Contoh: nama@email.com", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassword.Text.Length < 8)
            {
                MessageBox.Show("Password minimal 8 karakter!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassword.Text != txtConfirm.Text)
            {
                MessageBox.Show("Konfirmasi password tidak cocok!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connStr = @"Data Source=LAPTOP-24A5CGHI\WILDHANFIGHT;Initial Catalog=db_penjualan_diecast;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // ✅ Pakai Stored Procedure sp_RegisterPelanggan
                    // (menggantikan 3 query manual: cek username, cek email, insert)
                    SqlCommand cmd = new SqlCommand("sp_RegisterPelanggan", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                    cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@password", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@no_telepon", txtNoHP.Text);
                    cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text);

                    SqlParameter pHasil = new SqlParameter("@hasil", System.Data.SqlDbType.Int)
                    { Direction = System.Data.ParameterDirection.Output };
                    cmd.Parameters.Add(pHasil);

                    cmd.ExecuteNonQuery();

                    int hasil = Convert.ToInt32(pHasil.Value);

                    switch (hasil)
                    {
                        case 1:
                            MessageBox.Show("Registrasi berhasil! Silakan login.", "Sukses",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            Form1 login = new Form1();
                            login.Show();
                            this.Close();
                            break;

                        case 2:
                            MessageBox.Show("Username sudah terdaftar, gunakan username lain!", "Peringatan",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;

                        case 3:
                            MessageBox.Show("Email sudah terdaftar, gunakan email lain!", "Peringatan",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;

                        default:
                            MessageBox.Show("Registrasi gagal, silakan coba lagi.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error register: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Form1 login = new Form1();
            login.Show();
            this.Close();
        }
    }
}