using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public partial class FormCompleteProfile : Form
    {
        Label lblWelcome, lblSub, lblEmail, lblAlamat, lblNoHP;
        TextBox txtEmail, txtAlamat, txtNoHP;
        Button btnSimpan;
        string username;

        string connStr = Koneksi.GetConnectionString();

        public FormCompleteProfile(string username)
        {
            this.username = username;
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Lengkapi Profil";
            this.Size = new Size(420, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblWelcome = new Label();
            lblWelcome.Text = "Welcome, " + username;
            lblWelcome.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblWelcome.AutoSize = true;
            lblWelcome.Location = new Point(30, 30);

            lblSub = new Label();
            lblSub.Text = "Lengkapi profil Anda sebelum melanjutkan:";
            lblSub.Font = new Font("Segoe UI", 9);
            lblSub.ForeColor = Color.DimGray;
            lblSub.AutoSize = true;
            lblSub.Location = new Point(30, 75);

            lblEmail = new Label();
            lblEmail.Text = "Email      :";
            lblEmail.Location = new Point(30, 120);
            lblEmail.AutoSize = true;
            lblEmail.Font = new Font("Segoe UI", 10);

            txtEmail = new TextBox();
            txtEmail.Location = new Point(140, 117);
            txtEmail.Size = new Size(220, 22);

            lblAlamat = new Label();
            lblAlamat.Text = "Alamat    :";
            lblAlamat.Location = new Point(30, 155);
            lblAlamat.AutoSize = true;
            lblAlamat.Font = new Font("Segoe UI", 10);

            txtAlamat = new TextBox();
            txtAlamat.Location = new Point(140, 152);
            txtAlamat.Size = new Size(220, 22);

            lblNoHP = new Label();
            lblNoHP.Text = "No HP     :";
            lblNoHP.Location = new Point(30, 190);
            lblNoHP.AutoSize = true;
            lblNoHP.Font = new Font("Segoe UI", 10);

            txtNoHP = new TextBox();
            txtNoHP.Location = new Point(140, 187);
            txtNoHP.Size = new Size(220, 22);

            btnSimpan = new Button();
            btnSimpan.Text = "SIMPAN & LANJUTKAN";
            btnSimpan.Location = new Point(95, 260);
            btnSimpan.Size = new Size(210, 42);
            btnSimpan.BackColor = Color.Black;
            btnSimpan.ForeColor = Color.White;
            btnSimpan.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnSimpan.FlatStyle = FlatStyle.Flat;
            btnSimpan.Click += BtnSimpan_Click;

            this.Controls.AddRange(new Control[] {
                lblWelcome, lblSub,
                lblEmail, txtEmail,
                lblAlamat, txtAlamat,
                lblNoHP, txtNoHP,
                btnSimpan
            });
        }

        // =============================================
        // ✅ Simpan profil lewat Stored Procedure
        //    sp_UpdateProfilPelanggan (SP yang sama
        //    dengan yang dipakai di FormUser.cs), lalu
        //    lanjut ke FormUser dashboard.
        // =============================================
        private void BtnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtAlamat.Text) ||
                string.IsNullOrWhiteSpace(txtNoHP.Text))
            {
                MessageBox.Show("Semua field wajib diisi!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateProfilPelanggan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text.Trim());
                        cmd.Parameters.AddWithValue("@no_telepon", txtNoHP.Text.Trim());

                        SqlParameter pHasil = new SqlParameter("@hasil", SqlDbType.Int)
                        { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(pHasil);

                        cmd.ExecuteNonQuery();

                        if (Convert.ToInt32(pHasil.Value) == 1)
                        {
                            MessageBox.Show("Profil berhasil disimpan!", "Sukses",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            FormUser formUser = new FormUser(username);
                            formUser.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Gagal menyimpan profil!", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menyimpan: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}