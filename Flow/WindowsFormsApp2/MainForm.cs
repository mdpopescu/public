using System;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp2.Core;
using WindowsFormsApp2.Shell;

namespace WindowsFormsApp2
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            eventGetter = new EventGetter();
            propertySetter = new PropertySetter(FindControl);
        }

        //

        private readonly EventGetter eventGetter;
        private readonly PropertySetter propertySetter;

        private MainLogic logic;

        private Control FindControl(string name) =>
            Controls
                .Cast<Control>()
                .Where(it => string.Equals(it.Name, name, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

        //

        private void MainForm_Load(object sender, EventArgs e)
        {
            logic = new MainLogic(eventGetter.Get(this));
            logic.ViewChanges.Subscribe(propertySetter.Set);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //
        }
    }
}