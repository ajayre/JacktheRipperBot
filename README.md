JackTheRipperBot
================

3D printable DVD ripping robot

![alt text](https://github.com/ajayre/JacktheRipperBot/blob/master/Documents/Renders/JacktheRipperBot.png?raw=true?raw=true)

* http://www.britishideas.com/2013/09/03/jack-the-ripper-bot-i-introduction/
* https://github.com/ajayre/JacktheRipperBot
* https://www.thingiverse.com/thing:146319

## 3D Print
* The 3D print files are located at the two following locations
    * This github repo under /Parts/STls
    * Thingiverse https://www.thingiverse.com/thing:146319

## Installation

### Compile the source code
1. Download the source code. You'll need something like VSCode or Visual Studio Community Edition to compile the projects.

### Maestro
1. Install the Pololu Maestro software (https://www.pololu.com/docs/0J40/3.a) and then connect the Maestro to your PC and start the software.

1. Under the Serial Settings tab choose “USB Dual Port”. Uncheck “Enable CRC”. Check “Never sleep”. Click on Apply Settings
1. The microswitches should already be displaying their status. Press the switches and make sure they are working.

1. Enable the three servo channels and manually move each servo to make sure it works.

1. Use the software to position the arm over the in tray, DVD drive and out tray. Make a note of the servo position values. Be careful not to go near the limits of the servo as it turns into a continuous rotation servo and loses position.

1. Use the software to work out a slow speed for raising and lowering the toolhead and also the value to make it stop without the servo making a buzzing sound. Make a note of these values.

1. Finally use the software to determine the servo values for the grippers being fully open and fully closed. Make a note of these values.

### Tweaking
1. Open the DVD tray and put a DVD into it.
1. Use the two M8 nuts next to the hub to adjust the length of the arm so that the grippers are directly above the hole in the DVD.
1. Swing the arm over the in tray and check if the grippers are positioned over the center. If not swing the tray outwards slightly or add M8 washers as spacers between the tray and the tray anchor. If you move the tray then you will need to check again the servo value need for the in tray position.
1. Repeat this for the out tray.

### Raspberry Pi Setup
Copy to the Raspberry Pi (RPi) the following:

* BotServer.exe
* config.xml
  > Edit config.xml in a text editor and insert the servo values you have worked out under Tweaking

> Note: Mono needs to be installed on the RPi since this is a Windows EXE for .net 4.0

1. Run the server with:
> ?\
1\
mono ./BotServer.exe config.xml\

1. Open the DVD tray and put a DVD into the in tray.
1. Load a disc by visiting: http://192.168.1.70/botapi/?command=loaddisc
1. Replace the IP address with the address of your RPi. The disc should be loaded into the tray.

> You may have to adjust the YAxisLowertoDriveTime in config.xml which is given in 1/1000th of a second and governs the time spent lowering the toolhead before dropping the disc into the drive tray.

Once the load disc function is working test the unloading with: http://192.168.1.70/botapi/?command=unloaddisc

The YAxisLowertoOutTrayDropTime in config.xml governs the time spent lowering the toolhead before dropping the disc into the out tray. It has to drop from low enough that it consistently goes into the tray, but high enough that when the stack in the out tray is full there is space to drop the last disc.

### PC Setup
1. Copy the following files to your PC:
    * JacktheRipperBot.exe
    * Rip.bat
    * ScanningFinished.bat

> Make sure the full .NET Framework 4.0 is installed. If you need to install it then reboot before running JacktheRipperBot.exe.

1. Install DVDFab Passkey or AnyDVD. Both applications have the option to execute a command when scanning of a disc has completed. Set this up to run ScanningFinished.bat.
1. Install Handbrake and check the correct path to HandbrakeCLI.exe is used in Rip.bat.
1. Start JacktheRipperBot.exe and enter the IP address of your RPi.
1. Go to the advanced tab and try the various operations individually.
1. Once you are happy everything is working enter the number of discs in the in tray and click on Start to start ripping!

## Overview
Recently inspired and armed with a 3D printer I decided to build a robot to perform the disc changing. Jack the Ripper Bot was born.

View the video of the robot in action: http://www.youtube.com/watch?v=mmHycIOtYHA

I will divide the description of Jack into five parts: introduction, mechanics, electronics, software and troubleshooting.

My aims were:

Modular software and hardware design
Open source
Reliable
Low cost
Using off-the-shelf electronics
A stack of discs are placed into the “in tray”. An arm moves over to the stack and grabs the top disc. The disc is then lowered into the drive tray. Ripping takes place. Once complete the disc is removed from the drive tray and the arm takes it over to the “out tray” and places it there.

A Raspberry Pi is used to control the robot and a PC is used to control the overall process and the ripping.

## License
All files, including the software and part source files and STLs can be found on github. The files are licensed under GPLv3.
