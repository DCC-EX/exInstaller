# BaseStation-Installer

![.NET Core](https://github.com/DCC-EX/BaseStation-Installer/workflows/.NET%20Core/badge.svg)

This project provides a "one-click" installer for DCC++. At runtime, you can select either the "Classic" version or the new "DCC++ EX" version.

## Requirements

1. Windows/Linux/MacOS Computer
2. DCC-EX Compatible motor shield
3. DCC-EX Compatible Microcontroller such as Arduino Mega/Uno or SparkFun SAM21 Development board
4. Trains 🚄 🚅 🚈 🚝 🚞 🚃 🚋 🚆 🚉 🚊 🚇 🚟 🚠 🚡 🚂

## Here are the steps to use this installer

1. Download from [Releases](https://github.com/DCC-EX/BaseStation-Installer/releases) Section for your Operating system.
2. Unzip files from archive into separate folder
3. Run Basestation Installer.exe file
4. Select which Base Station Config you would like to run.
5. Select Supported boards for Base Station.
6. Select Support Motor Shield
7. Refresh and Select available Com Port
8. Press compile and upload to Compile BaseStation code and upload to your Selected Board
9. Enjoy running your trains

## For Support you can see our thread on Train Board or join our discord

[Train Board Thread on DCC Update Project](https://www.trainboard.com/highball/index.php?threads/dcc-update-project-2020.130071/)

[Discord](https://discord.gg/y2sB4Fp)

## How to edit and compile this project

1. Install .Net Core 3.1 sdk from the following link [.Net](https://dotnet.microsoft.com/download)
2. Clone the repo or download the zip file and extract
3. If you use Visual Studio open the BaseStationInstaller.sln if not open folder with your favorite IDE or text editor
4. Make changes where you want.
5. To compile you can use a shell window and type `dotnet build` and it should auto build for your platform a debug version of the project. 
6. Run and test your changes.