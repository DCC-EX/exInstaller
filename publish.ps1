dotnet publish -c release -r linux-x64
dotnet publish -c release -r win-x64
dotnet publish -c release -r win-x86
dotnet publish -c release -r osx-x64
dotnet publish -c release -r linux-arm64
dotnet publish -c release -r linux-arm
del exInstaller-*
cd "exInstaller\exInstaller\bin\Release\netcoreapp3.1\linux-arm\publish"
Remove-Item arduino-cli-runtimes\*_32bit -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*_*64* -Recurse -Force -Confirm:$false
7z a -ttar ..\..\..\..\..\..\exInstaller.tar
cd cd ..\..\..\..\..\..
7z a exInstaller.tar -tgzip .\exInstaller-linux-arm.tar.gz
del exInstaller.tar
cd "exInstaller\exInstaller\bin\Release\netcoreapp3.1\linux-arm64\publish"
Remove-Item arduino-cli-runtimes\*_32bit -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*_64bit -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*_ARM -Recurse -Force -Confirm:$false
7z a -ttar cd ..\..\..\..\..\..\exInstaller.tar
cd cd ..\..\..\..\..\..
7z a exInstaller.tar -tgzip .\exInstaller-linux-arm64.tar.gz
del exInstaller.tar
cd "exInstaller\exInstaller\bin\Release\netcoreapp3.1\linux-x64\publish"
Remove-Item arduino-cli-runtimes\Win* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*ARM* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\mac* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\Linux_32bit -Recurse -Force -Confirm:$false
7z a -ttar ..\..\..\..\..\..\exInstaller.tar
cd cd ..\..\..\..\..\..
7z a exInstaller.tar -tgzip .\exInstaller-linux-x64.tar.gz
del exInstaller.tar
cd "exInstaller\exInstaller\bin\Release\netcoreapp3.1\win-x86\publish"
Remove-Item arduino-cli-runtimes\Linux* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\mac* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*_64bit -Recurse -Force -Confirm:$false
7z a -tzip ..\..\..\..\..\..\exInstaller-win-x86.zip
cd cd ..\..\..\..\..\..
cd "exInstaller\exInstaller\bin\Release\netcoreapp3.1\win-x64\publish"
Remove-Item arduino-cli-runtimes\Linux* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\mac* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*_32bit -Recurse -Force -Confirm:$false
7z a -tzip ..\..\..\..\..\..\exInstaller-win-x64.zip
cd cd ..\..\..\..\..\..
7z a exInstaller.tar -tgzip .\exInstaller-osx-x64.tar.gz
del exInstaller.tar
cd "exInstaller\exInstaller\bin\Release\netcoreapp3.1\osx-x64\publish"
Remove-Item arduino-cli-runtimes\Linux* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\Win* -Recurse -Force -Confirm:$false
7z a -ttar ..\..\..\..\..\..\exInstaller.tar
cd ..\..\..\..\..\..\..

