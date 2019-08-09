using System;
using System.Drawing;
using System.Windows.Forms;
using FastRead.Contracts;
using FastRead.Services;

namespace FastRead
{
    public partial class MainForm : Form, MainUI
    {
        public MainForm()
        {
            InitializeComponent();

            var parser = new KeyParser();
            logic = new MainLogic(this, parser);

            calculator = new GraphicsCalculator();
        }

        public void EnterFullScreen()
        {
            WindowState = FormWindowState.Normal;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }

        public void LeaveFullScreen()
        {
            FormBorderStyle = FormBorderStyle.Sizable;
            WindowState = FormWindowState.Normal;
        }

        public void Display(string word)
        {
            HideWord();
            SetWord(word);
            ShowWord();
            UpdateScreen();
        }

        //

        private readonly MainLogic logic;

        private readonly GraphicsCalculator calculator;

        private void HideWord()
        {
            lblWord.ForeColor = BackColor;
        }

        private void ShowWord()
        {
            lblWord.ForeColor = Color.White;
        }

        private void SetWord(string word)
        {
            lblWord.Text = word;
            lblWord.Location = calculator.GetCenterPosition(Size, lblWord.Size);
        }

        private static void UpdateScreen()
        {
            Application.DoEvents();
        }

        //

        private void MainForm_Load(object sender, EventArgs e)
        {
            EnterFullScreen();
            Display("Press Home for demo");
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            logic.Handle(e.KeyData);
            e.Handled = e.SuppressKeyPress = true;
        }
    }
}