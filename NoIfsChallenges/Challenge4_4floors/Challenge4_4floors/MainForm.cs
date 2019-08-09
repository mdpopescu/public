using System;
using System.Drawing;
using System.Windows.Forms;
using Challenge4_4floors.Library.Models;

namespace Challenge4_4floors
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            building = new Building();
            elevator = new Elevator();
        }

        //

        private readonly Building building;
        private readonly Elevator elevator;

        private new void Update()
        {
            rb1stFloor.Checked = building.GetFloor(1).HasElevator;
            rb2ndFloor.Checked = building.GetFloor(2).HasElevator;
            rb3rdFloor.Checked = building.GetFloor(3).HasElevator;
            rb4thFloor.Checked = building.GetFloor(4).HasElevator;
            txt1stFloorDisplay.Text = building.GetFloor(1).Display;
            txt2ndFloorDisplay.Text = building.GetFloor(2).Display;
            txt3rdFloorDisplay.Text = building.GetFloor(3).Display;
            txt4thFloorDisplay.Text = building.GetFloor(4).Display;

            btnGoTo1stFloor.ForeColor = elevator.IsLit1 ? Color.Lime : SystemColors.ControlText;
            btnGoTo2ndFloor.ForeColor = elevator.IsLit2 ? Color.Lime : SystemColors.ControlText;
            btnGoTo3rdFloor.ForeColor = elevator.IsLit3 ? Color.Lime : SystemColors.ControlText;
            btnGoTo4thFloor.ForeColor = elevator.IsLit4 ? Color.Lime : SystemColors.ControlText;
            txtElevatorDisplay.Text = elevator.Display;
        }

        //

        private void MainForm_Load(object sender, EventArgs e)
        {
            Update();
        }
    }
}