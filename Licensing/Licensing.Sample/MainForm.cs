using System;
using System.Globalization;
using System.Windows.Forms;
using Renfield.Licensing.Library.Models;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Sample
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();

      var options = new LicenseOptions
      {
        Password = "password",
        Salt = "saltsaltsalt",
        CheckUrl = null,
      };
      licenser = Licenser.Create(options);

      licenser.Initialize();
    }

    //

    private const string DATE_FORMAT = "yyyy-MM-dd";

    private readonly Licenser licenser;

    private void ShowLicense()
    {
      cbIsLicensed.Checked = licenser.IsLicensed;
      cbIsTrial.Checked = licenser.IsTrial;
      cbShouldRun.Checked = licenser.ShouldRun;

      var registration = licenser.GetRegistration();
      txtCreatedOn.Text = registration.CreatedOn.ToString(DATE_FORMAT);
      txtLimitsDays.Text = registration.Limits.Days.ToString();
      txtLimitsRuns.Text = registration.Limits.Runs.ToString();

      txtKey.Text = registration.Key;
      txtName.Text = registration.Name;
      txtContact.Text = registration.Contact;
      txtProcessorId.Text = registration.ProcessorId;
      txtExpiration.Text = registration.Expiration.ToString(DATE_FORMAT);
    }

    private void SaveLicense()
    {
      var registration = licenser.GetRegistration();

      registration.CreatedOn = DateTime.ParseExact(txtCreatedOn.Text, DATE_FORMAT, CultureInfo.InvariantCulture);
      registration.Limits.Days = string.IsNullOrWhiteSpace(txtLimitsDays.Text) ? -1 : int.Parse(txtLimitsDays.Text);
      registration.Limits.Runs = string.IsNullOrWhiteSpace(txtLimitsRuns.Text) ? -1 : int.Parse(txtLimitsRuns.Text);

      registration.Key = txtKey.Text;
      registration.Name = txtName.Text;
      registration.Contact = txtContact.Text;
      registration.ProcessorId = txtProcessorId.Text;
      registration.Expiration = DateTime.ParseExact(txtExpiration.Text, DATE_FORMAT, CultureInfo.InvariantCulture);

      licenser.SaveRegistration(registration);
    }

    //

    private void MainForm_Load(object sender, EventArgs e)
    {
      ShowLicense();
    }

    private void btnLoad_Click(object sender, EventArgs e)
    {
      // BUG: doesn't actually reload
      ShowLicense();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      SaveLicense();
    }
  }
}