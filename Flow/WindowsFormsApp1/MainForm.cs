using System;
using System.Windows.Forms;
using WindowsFormsApp1.Core;
using WindowsFormsApp1.Core.Flows;
using WindowsFormsApp1.Shell;

// based on ideas from https://egghead.io/courses/cycle-js-fundamentals

namespace WindowsFormsApp1
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //

        private readonly FlowEnvironment env = new FlowEnvironment();

        private void SetUp()
        {
            env.AddInput("weight", tbWeight.Intercept("ValueChanged"));
            env.AddInput("height", tbHeight.Intercept("ValueChanged"));
            env.AddInput("save", btnSave.Intercept("Click"));
            env.AddInput("restore", btnRestore.Intercept("Click"));

            env.AddFlow(new SliderFlow("weight", "Weight (kg)", 40));
            env.AddFlow(new SliderFlow("height", "Height (cm)", 150));
            env.AddFlow(new BmiFlow());
            env.AddFlow(new SaveRestoreFlow());

            env.AddOutput("weight", new PropertySetter(lblWeight));
            env.AddOutput("height", new PropertySetter(lblHeight));
            env.AddOutput("bmi", new PropertySetter(lblBMI));
            env.AddOutput("save-weight", new PropertySetter(lblSavedWeight));
            env.AddOutput("save-height", new PropertySetter(lblSavedHeight));
            env.AddOutput("set-weight", new PropertySetter(tbWeight));
            env.AddOutput("set-height", new PropertySetter(tbHeight));
            env.AddOutput("enable-restore", new PropertySetter(btnRestore));
        }

        //

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetUp();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            env.Dispose();
        }
    }
}