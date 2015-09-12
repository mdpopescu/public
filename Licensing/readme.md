# Licensing library for a C# GUI application

## Usage:

    var options = new LicenserOptions { ... };
    var licenser = Licenser.Create(options);

### Initialize and check for validity

    var isValid = licenser.Check();

This will return *true* if the application is licensed and *false* otherwise.

### Display registration information

    licenser.ShowRegistration();

Shows a modal window indicating either that the application is registered and to whom, or that it's a trial. In that case, it will show the remaining days / runs
and it will allow for the registration key to be entered.

## Options

Name        | Description
----------- | -----------
Password    | The encryption key; if not specified, the registry will be unencrypted
DisplayName | The product name displayed on the registration page; defaults to the one from the main assembly
Contact     | Contact information (e.g., email address)
BuyUrl      | The link to the web page where a license can be bought
CheckUrl    | The link used to check the validity of the license (see below); if not specified, the license key is assumed valid if it exists and is a valid guid

## Limits

The install can set up the following limits; at least one of them must be present:

Name   | Description
------ | -----------
Days   | The total number of days (defaults to unlimited)
Runs   | The total number of runs (defaults to unlimited)

If any of these conditions is met the trial expires - the validity check returns false.

## CheckUrl - checking the license

If CheckUrl is present, the application will first check that there is a key and that it 1) is a valid GUID and 2) has not expired. If that check fails,
the application treats the key as non-existent. If both conditions are met, the initialization makes a GET call to

https://{CheckUrl}?Key={key}&ProcessorId={id} 

If the call fails or does not return a correct response, the key is considered invalid (the application is not registered).

A correct response will have the form

{key} expires on {date}

With {date} formatted as yyyy-mm-dd. A permanent license will have a date of 9999-12-31.
