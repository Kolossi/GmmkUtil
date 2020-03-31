# GmmkUtil

## Summary

An executable that allows setting the profile of the Glorious PC Gaming Race
GMMK Keyboard to be set on the command line.

My use case for this is to set to a Red wave profile whilst autotype of my
password manager is in operation, then set back to usual profile when it
has completed.

I was disappointed that the manufacturer [GMMK Keyboard Editor](https://www.pcgamingrace.com/pages/gmmk-software-download) app
did not support command-line options to issue commands, and support
just bounced requests for support to develop an open-source app to enhance
their product.  Boo!

I proceeded to develop this by sniffing the USB packets sent by the 
GMMK Keyboard Editor app.

## Usage

The command has help text if run without parameters.

### <a name="setprofile"></a>--setprofile

An installer may come later, for now it's just copy the .exe from the github
[releases page](https://github.com/Kolossi/GmmkUtil/releases) by hand.  Suggestion is to `C:\GmmkUtil` on windows.

Simplest usage is to set the keyboard to use on of the 3 preset profiles:

```
C:\GmmkUtil>GmmkUtilConsole --setprofile 1
```

### <a name="initprofile"></a>--initprofile

Alternatively, the util can be set to stay running and monitor for the
keyboard being inserted in the USB port and set the profile when that
happens:

```
C:\GmmkUtil>GmmkUtilConsole --initprofile 2
```

Why is this useful? I use my keyboard with 2 PCs, connected via a USB switcher
sometimes it easy to forget which PC the keyboard is connected to. I have set
up two profiles to match the different colour schemes of the 2 PCs.

Once using this command, as soon as I switch the keyboard, the software
running on the PC it's switched to changes the keyboard RGB profile.

Ideally, it's run on machine startup, in which case the following
windows registry file will set it up to run without leaving a console
window running:

```
Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run]
"GmmkUtil"="powershell -WindowStyle Hidden Start-Process -FilePath \"C:\\GmmkUtil\\GmmkUtilConsole.exe\" -ArgumentList \"--initprofile=1\" -NoNewWindow"
```

(On the second PC, edit to read `--initprofile=2`)

Suggestions for how to do this on Linux would be gratefully received.


## Future 

To be developed over time.

Ultimately I'd like to enable a [DasKeyboard 5Q](https://www.daskeyboard.com/x/x50q-rgb-mechanical-keyboard/)-like experience on my GMMK
keyboard using the leds as a dashboard for stats, alerts etc.

## Supported models

Currently this app is only guaranteed to work on my UK ISO Full Keyboard
with S/N 2019xxxxxxxxxx! The profile-change will probably work on any keyboard
though.

## Contribute

It would be great to get reports of this working on other keyboards.

It would be even greater to receive Pull Requests for extra device support,
features etc.

## If you like this ...

Check out [r/MechanicalKeyboards](https://www.reddit.com/r/MechanicalKeyboards/)

## Support

**Note** - The value of support guaranteed for this util is exactly what you paid
for it :-D

