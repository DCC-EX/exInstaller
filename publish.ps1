dotnet publish -c release -r linux-x64
dotnet publish -c release -r win-x64
dotnet publish -c release -r win-x86
dotnet publish -c release -r osx-x64
dotnet publish -c release -r linux-arm64
dotnet publish -c release -r linux-arm

wsl tar -czf exInstaller-arm.tar.gz "BaseStationInstaller/bin/Release/netcoreapp3.1/linux-arm/publish/*"
wsl tar -czf exInstaller-arm64.tar.gz "BaseStationInstaller/bin/Release/netcoreapp3.1/linux-arm64/publish/*"
wsl tar -czf exInstaller-linux-x64.tar.gz "BaseStationInstaller/bin/Release/netcoreapp3.1/linux-x64/publish/*"
wsl zip -r exInstaller-win32.zip "BaseStationInstaller/bin/Release/netcoreapp3.1/win-x86/publish"
wsl zip -r exInstaller-win64.zip "BaseStationInstaller/bin/Release/netcoreapp3.1/win-x64/publish"
wsl tar -czf exInstaller-osx.tar.gz "BaseStationInstaller/bin/Release/netcoreapp3.1/osx-x64/publish/*"

