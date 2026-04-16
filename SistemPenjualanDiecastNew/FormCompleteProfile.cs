using System;
using System.Drawing;
using System.Windows.Forms;

namespace SistemPenjualanDiecastNew
{
    public partial class FormCompleteProfile : Form
    {
        Label lblWelcome;
        string username;

        public FormCompleteProfile(string username)
        {
            this.username = username;
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "User Dashboard";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            