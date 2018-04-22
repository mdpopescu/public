using System;
using System.Windows.Forms;
using Challenge4.Library.Services;

namespace Challenge4
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            elevator = new Elevator();
        }

        //

        private readonly Elevator elevator;

        private void UpdateState()
        {
            btnCallTo3rd.Enabled = elevator.Info.Floor3.Button1Enabled;
            btnGoFrom3rdTo1st.Enabled = elevator.Info.Floor3.Button2Enabled;
            btnGoFrom3rdTo2nd.Enabled = elevator.Info.Floor3.Button3Enabled;
            txt3rdScreen.Text = elevator.Info.Floor3.Screen;

            btnCallTo2nd.Enabled = elevator.Info.Floor2.Button1Enabled;
            btnGoUp.Enabled = elevator.Info.Floor2.Button2Enabled;
            btnGoDown.Enabled = elevator.Info.Floor2.Button3Enabled;
            txt2ndScreen.Text = elevator.Info.Floor2.Screen;

            btnCallTo1st.Enabled = elevator.Info.Floor1.Button1Enabled;
            btnGoFrom1stTo3rd.Enabled = elevator.Info.Floor1.Button2Enabled;
            btnGoFrom1stTo2nd.Enabled = elevator.Info.Floor1.Button3Enabled;
            txt1stScreen.Text = elevator.Info.Floor1.Screen;
        }

        //

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void btnCallTo3rd_Click(object sender, EventArgs e)
        {
            elevator.CallTo3rd();
            UpdateState();
        }

        private void btnGoFrom3rdTo1st_Click(object sender, EventArgs e)
        {
            elevator.GoTo1st();
            UpdateState();
        }

        private void btnGoFrom3rdTo2nd_Click(object sender, EventArgs e)
        {
            elevator.GoTo2nd();
            UpdateState();
        }

        private void btnCallTo2nd_Click(object sender, EventArgs e)
        {
            elevator.CallTo2nd();
            UpdateState();
        }

        private void btnGoUp_Click(object sender, EventArgs e)
        {
            elevator.GoTo3rd();
            UpdateState();
        }

        private void btnGoDown_Click(object sender, EventArgs e)
        {
            elevator.GoTo1st();
            UpdateState();
        }

        private void btnCallTo1st_Click(object sender, EventArgs e)
        {
            elevator.CallTo1st();
            UpdateState();
        }

        private void btnGoFrom1stTo3rd_Click(object sender, EventArgs e)
        {
            elevator.GoTo3rd();
            UpdateState();
        }

        private void btnGoFrom1stTo2nd_Click(object sender, EventArgs e)
        {
            elevator.GoTo2nd();
            UpdateState();
        }
    }
}