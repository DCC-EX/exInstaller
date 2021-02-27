dotnet publish -c release -r linux-x64
dotnet publish -c release -r win-x64
dotnet publish -c release -r win-x86
dotnet publish -c release -r osx-x64
dotnet publish -c release -r linux-arm64
dotnet publish -c release -r linux-arm
del exInstaller-*
cd "F:\TrainStuff\exInstaller\BaseStationInstaller\bin\Release\netcoreapp3.1\linux-arm\publish"
Remove-Item arduino-cli-runtimes\*_32bit -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*_*64* -Recurse -Force -Confirm:$false
wsl tar -vczf /mnt/f/TrainStuff/exInstaller/exInstaller-linux-arm.tar.gz *
cd "F:\TrainStuff\exInstaller\BaseStationInstaller\bin\Release\netcoreapp3.1\linux-arm64\publish"
Remove-Item arduino-cli-runtimes\*_32bit -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*_64bit -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*_ARM -Recurse -Force -Confirm:$false
wsl  tar -vczf /mnt/f/TrainStuff/exInstaller/exInstaller-linux-arm64.tar.gz *
cd "F:\TrainStuff\exInstaller\BaseStationInstaller\bin\Release\netcoreapp3.1\linux-x64\publish"
Remove-Item arduino-cli-runtimes\Win* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*ARM* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\mac* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\Linux_32bit -Recurse -Force -Confirm:$false
wsl  tar -vczf /mnt/f/TrainStuff/exInstaller/exInstaller-linux-x64.tar.gz *
cd "F:\TrainStuff\exInstaller\BaseStationInstaller\bin\Release\netcoreapp3.1\win-x86\publish"
Remove-Item arduino-cli-runtimes\Linux* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\mac* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*_64bit -Recurse -Force -Confirm:$false
wsl  zip -r /mnt/f/TrainStuff/exInstaller/exInstaller-win-x86.zip *
cd "F:\TrainStuff\exInstaller\BaseStationInstaller\bin\Release\netcoreapp3.1\win-x64\publish"
Remove-Item arduino-cli-runtimes\Linux* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\mac* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*_32bit -Recurse -Force -Confirm:$false
wsl  zip -r /mnt/f/TrainStuff/exInstaller/exInstaller-win-x64.zip *
cd "F:\TrainStuff\exInstaller\BaseStationInstaller\bin\Release\netcoreapp3.1\osx-x64\publish"
Remove-Item arduino-cli-runtimes\Linux* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\Win* -Recurse -Force -Confirm:$false
wsl tar -vczf /mnt/f/TrainStuff/exInstaller/exInstaller-osx-x64.tar.gz *
cd F:\TrainStuff\exInstaller


