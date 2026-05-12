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

       
