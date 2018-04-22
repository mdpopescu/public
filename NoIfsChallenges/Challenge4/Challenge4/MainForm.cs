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
            btnCallTo3rd.Enabled = elevator.Info.Floor3.CallEnabled;
            btnGoFrom3rdTo1st.Enabled = !elevator.Info.Floor3.CallEnabled;
            btnGoFrom3rdTo2nd.Enabled = !elevator.Info.Floor3.CallEnabled;
            txt3rdScreen.Text = elevator.Info.Floor3.Screen;

            btnCallTo2nd.Enabled = elevator.Info.Floor2.CallEnabled;
            btnGoUp.Enabled = !elevator.Info.Floor2.CallEnabled;
            btnGoDown.Enabled = !elevator.Info.Floor2.CallEnabled;
            txt2ndScreen.Text = elevator.Info.Floor2.Screen;

            btnCallTo1st.Enabled = elevator.Info.Floor1.CallEnabled;
            btnGoFrom1stTo3rd.Enabled = !elevator.Info.Floor1.CallEnabled;
            btnGoFrom1stTo2nd.Enabled = !elevator.Info.Floor1.CallEnabled;
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