using System;
using System.Globalization;
using System.Windows.Forms;
using Renfield.Licensing.Library;
using Renfield.Licensing.Library.Contracts;
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

      sys = new WinSys();
    }

    //

    private const string DATE_FORMAT = Constants.DATE_FORMAT;

    private readonly Licenser licenser;
    private readonly Sys sys;

    private void ShowLicense()
    {
      var registration = licenser.LoadRegistration();

      cbIsLicensed.Checked = licenser.IsLicensed;
      cbIsTrial.Checked = licenser.IsTrial;
      cbShouldRun.Checked = licenser.ShouldRun;

      if (registration == null)
        return;

      txtCreatedOn.Text = registration.CreatedOn.ToString(DATE_FORMAT);
      txtLimitsDays.Text = registration.Limits.Days.ToString();
      txtLimitsRuns.Text = registration.Limits.Runs.ToString();

      txtKey.Text = registration.Key;
      txtName.Text = registration.Name;
      txtContact.Text = registration.Contact;
      txtProcessorId.Text = sys.GetProcessorId();
      txtExpiration.Text = registration.Expiration.ToString(DATE_FORMAT);
    }

    private void SaveLicense()
    {
      var registration = new LicenseRegistration
      {
        CreatedOn = DateTime.ParseExact(txtCreatedOn.Text, DATE_FORMAT, CultureInfo.InvariantCulture),
        Limits =
        {
          Days = string.IsNullOrWhiteSpace(txtLimitsDays.Text) ? -1 : int.Parse(txtLimitsDays.Text),
          Runs = string.IsNullOrWhiteSpace(txtLimitsRuns.Text) ? -1 : int.Parse(txtLimitsRuns.Text)
        },
        Key = txtKey.Text,
        Name = txtName.Text,
        Contact = txtContact.Text,
        Expiration = DateTime.ParseExact(txtExpiration.Text, DATE_FORMAT, CultureInfo.InvariantCulture),
      };

      licenser.SaveRegistration(registration);

      // refresh the details
      ShowLicense();
    }

    //

    private void MainForm_Load(object sender, EventArgs e)
    {
      ShowLicense();
    }

    private void btnLoad_Click(object sender, EventArgs e)
    {
      ShowLicense();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      SaveLicense();
    }
  }
}