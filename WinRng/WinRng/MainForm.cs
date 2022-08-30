using WinRng.Services;

namespace WinRng;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();

        limiter = new NumericLimiter();
    }

    //

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        txtIntervalMin.KeyPress += Txt_KeyPress;
        txtIntervalMax.KeyPress += Txt_KeyPress;

        lblResult.Text = "Result: 0.0000";
    }

    //

    private readonly Random rng = new();

    private readonly NumericLimiter limiter;

    //

    private void Txt_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (sender is TextBox txtBox)
            e.Handled = !limiter.AllowChar(txtBox.Text, e.KeyChar);
    }

    private void btnGenerate_Click(object sender, EventArgs e)
    {
        var min = double.Parse(txtIntervalMin.Text);
        var max = double.Parse(txtIntervalMax.Text);

        var result = rng.NextDouble() * (max - min) + min;

        lblResult.Text = $"Result: {result:F4}";
    }
}