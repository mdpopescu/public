using System;
using System.Drawing;
using System.Windows.Forms;
using Challenge2New.Helpers;
using Challenge2New.Library.Contracts;
using Challenge2New.Library.Models;
using Challenge2New.Library.Services;

namespace Challenge2New
{
    public partial class MainForm : Form, IUserInterface
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public void SetUp(EventHandler onStartStop, EventHandler onHold, EventHandler onReset)
        {
            this.UIChange(
                () =>
                {
                    btnStartStop.Click += onStartStop;
                    btnHold.Click += onHold;
                    btnReset.Click += onReset;
                }
            );
        }

        public void TearDown(EventHandler onStartStop, EventHandler onHold, EventHandler onReset)
        {
            this.UIChange(
                () =>
                {
                    btnStartStop.Click -= onStartStop;
                    btnHold.Click -= onHold;
                    btnReset.Click -= onReset;
                }
            );
        }

        public void SetEnabled(OperableActionName actionName, bool value)
        {
            this.UIChange(
                () =>
                {
                    var control = GetControl(actionName);
                    control.Enabled = value;
                    control.BackColor = value ? Color.Lime : Color.DarkGray;
                }
            );
        }

        public void SetDisplay(string value)
        {
            this.UIChange(() => lblDisplay.Text = value);
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

        private void SelectImpl(Control control)
        {
            btnImplWithIfs.BackColor = Color.DarkGray;
            btnImplWithState.BackColor = Color.DarkGray;
            btnImplRxState.BackColor = Color.DarkGray;
            btnImplRxFunc.BackColor = Color.DarkGray;

            control.BackColor = Color.Lime;
        }

        private Control GetControl(OperableActionName actionName) => actionName switch
        {
            OperableActionName.START_STOP => btnStartStop,
            OperableActionName.HOLD => btnHold,
            OperableActionName.RESET => btnReset,
            _ => throw new ArgumentOutOfRangeException(nameof(actionName), actionName, null),
        };

        //

        private void btnImplWithIfs_Click(object sender, EventArgs e)
        {
            SelectImpl(btnImplWithIfs);
            gbInterface.Enabled = true;

            logic?.Dispose();
            logic = new TimerLogicWithIfs(this);
            logic.Initialize();
        }

        private void btnImplWithState_Click(object sender, EventArgs e)
        {
            SelectImpl(btnImplWithState);
            gbInterface.Enabled = true;

            logic?.Dispose();
            //logic = new TimerLogicWithState(this);
            logic.Initialize();
        }

        private void btnImplRxState_Click(object sender, EventArgs e)
        {
            SelectImpl(btnImplRxState);
            gbInterface.Enabled = true;

            logic?.Dispose();
            //logic = new TimerLogicRxState(this);
            logic.Initialize();
        }

        private void btnImplRxFunc_Click(object sender, EventArgs e)
        {
            SelectImpl(btnImplRxFunc);
            gbInterface.Enabled = true;

            logic?.Dispose();
            //logic = new TimerLogicRxFunc(this);
            logic.Initialize();
        }
    }
}