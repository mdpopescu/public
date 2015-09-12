# Licensing library for a C# GUI application

## Usage:

    var options = new LicenseOptions { ... };
    var licenser = Licenser.Create(options);

### Initialize and check for validity

    var isValid = licenser.IsLicensed();

This will return *true* if the application is licensed and *false* otherwise.

### Check if the trial is still available

    var isTrial = licenser.IsTrial();

This will return *true* if the trial is still available and *false* if the limits have been reached (the application should be closed in that case).

### Check whether the application should run

    var shouldRun = licenser.ShouldRun();

This will return *true* if the application should run (either the license is valid, or the trial hasn't reached its limits) and *false* otherwise.

### Showing registration information

    var details = licenser.GetRegistration();

Returns the registration details saved in the registry; the application might then choose to display those details, eg a message showing how many days
are remaining. It is recommended that a text box be provided for the user to enter a license key; if that happens, the application can use the
CreateRegistration method (below).

### Create registration information (eg from the installer)

    var details = new LicenseRegistration { ... };
    licenser.CreateRegistration(details);

Creates the registry entry with the given registration details, optionally encrypted with a password. If CheckUrl is not empty, it will first try to register
the application at the given URL and only save to the registry if everything went ok.

### Sample code:

    var options = new LicenseOptions { ... };
    var licenser = Licenser.Create(options);
    var details = licenser.GetRegistration();

    // initial check
    if (!licenser.IsLicensed())
    {
        // show registration / trial screen
    }

    // the customer might have just bought a license and entered the key, check again
    if (!licenser.IsLicensed())
    {
        if (!licenser.IsTrial())
        {
            Application.Terminate();
        }
    }

The double check can be simplified by using the ShouldRun method:

    var options = new LicenseOptions { ... };
    var licenser = Licenser.Create(options);
    var details = licenser.GetRegistration();

    // initial check
    if (!licenser.IsLicensed())
    {
        // show registration / trial screen
    }

    // should I run the application?
    if (!licenser.ShouldRun())
    {
        Application.Terminate();
    }


## Options

Name        | Description
----------- | -----------
Password    | The encryption key; if not specified, the registry will be unencrypted
CheckUrl    | The link used to check the validity of the license (see below); if not specified, the license key is assumed valid if it exists and is a valid guid

### Checking the license

If CheckUrl is present, the licenser will first check that there is a key and that it 1) is a valid GUID and 2) has not expired. If that check fails,
the licenser treats the key as non-existent. If both conditions are met, the initialization makes a GET call to

https://{CheckUrl}?Key={key}&ProcessorId={id} 

If the call fails or does not return a correct response, the key is considered invalid (the application is not registered).

A correct response will have the form

{key} {date}

With {date} formatted as yyyy-mm-dd. A permanent license will have a date of 9999-12-31.

The licenser will overwrite the expiration date in the registry with the given date.

### Registering

If CheckUrl is present, the CreateRegistration method will (after making sure that the registration information is internally valid) send a POST request to

https://{CheckUrl}

with Content-Type set to application/x-www-form-urlencoded and the LicenseRegistration details.


## Limits

The install can set up the following limits; at least one of them must be present:

Name   | Description
------ | -----------
Days   | The total number of days (defaults to unlimited)
Runs   | The total number of runs (defaults to unlimited)

If any of these conditions is met the trial expires - the validity check returns false.
