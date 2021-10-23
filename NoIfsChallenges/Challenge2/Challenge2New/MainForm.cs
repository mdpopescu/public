using System;
using System.Windows.Forms;
using Challenge2New.Library.Contracts;
using Challenge2New.Library.Models;

namespace Challenge2New
{
    public partial class MainForm : Form, IUserInterface
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public void SetEnabled(OperableControl control, bool value)
        {
            GetControl(control).Enabled = value;
        }

        public void SetDisplay(string value)
        {
            lblDisplay.Text = value;
        }

        //

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            gbInterface.Enabled = false;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            logic?.Dispose();

            base.OnFormClosed(e);
        }

        //

        private ITimerLogic? logic;

        private Control GetControl(OperableControl control) => control switch
        {
            OperableControl.START_STOP => btnStartStop,
            OperableControl.HOLD => btnHold,
            OperableControl.RESET => btnReset,
            _ => throw new ArgumentOutOfRangeException(nameof(control), control, null),
        };

        //

        private void btnImplWithIfs_Click(object sender, EventArgs e)
        {
            logic?.Dispose();
            //logic = new TimerLogicWithIfs();
            logic.Initialize(this);
        }

        private void btnImplWithState_Click(object sender, EventArgs e)
        {
            logic?.Dispose();
            //logic = new TimerLogicWithState();
            logic.Initialize(this);
        }

        private void btnImplRxState_Click(object sender, EventArgs e)
        {
            logic?.Dispose();
            //logic = new TimerLogicRxState();
            logic.Initialize(this);
        }

        private void btnImplRxFunc_Click(object sender, EventArgs e)
        {
            logic?.Dispose();
            //logic = new TimerLogicRxFunc();
            logic.Initialize(this);
        }
    }
}