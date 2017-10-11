using System;
using System.Reactive.Linq;
using System.Windows.Forms;
using WindowsFormsApp3.Core;

namespace WindowsFormsApp3
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //

        private readonly FlowEnvironment env = new FlowEnvironment();

        //

        private void MainForm_Load(object sender, EventArgs e)
        {
            env.AddInput("tbWeight", Observable.FromEventPattern(tbWeight, "ValueChanged"));
            env.AddInput("tbHeight", Observable.FromEventPattern(tbHeight, "ValueChanged"));

            // the BMI component must be first - it must react to the initial values from the sliders
            env.AddComponent(new BmiFlow());
            env.AddComponent(new SliderFlow("tbWeight", "weights", "lblWeight:Text", "Weight (kg): {0:##0}", 40));
            env.AddComponent(new SliderFlow("tbHeight", "heights", "lblHeight:Text", "Height (cm): {0:##0}", 150));

            env.AddOutput<string>("lblWeight:Text", text => lblWeight.Text = text);
            env.AddOutput<string>("lblHeight:Text", text => lblHeight.Text = text);
            env.AddOutput<string>("lblBMI:Text", text => lblBMI.Text = text);

            env.Start();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //
        }
    }
}