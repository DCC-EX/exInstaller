dotnet publish -c release -r linux-x64
dotnet publish -c release -r win-x64
dotnet publish -c release -r win-x86
dotnet publish -c release -r osx-x64
dotnet publish -c release -r linux-arm64
dotnet publish -c release -r linux-arm
del exInstaller-*
cd "exInstaller\bin\Release\netcoreapp5.0\linux-arm\publish"
Remove-Item arduino-cli-runtimes\*_32bit -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*_*64* -Recurse -Force -Confirm:$false
7z a  -ttar ..\..\..\..\..\..\exInstaller-linux-arm.tar
cd ..\..\..\..\..\..
echo 7z a .\exInstaller.tar -tgzip .\exInstaller-linux-arm.tar.gz
echo del exInstaller.tar
cd "exInstaller\bin\Release\netcoreapp5.0\linux-arm64\publish"
Remove-Item arduino-cli-runtimes\*_32bit -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*_64bit -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*_ARM -Recurse -Force -Confirm:$false
7z a  -ttar ..\..\..\..\..\..\exInstaller-linux-arm64.tar
cd ..\..\..\..\..\..
echo 7z a .\exInstaller.tar -tgzip .\exInstaller-linux-arm64.tar.gz
echo del exInstaller.tar
cd "exInstaller\bin\Release\netcoreapp5.0\linux-x64\publish"
Remove-Item arduino-cli-runtimes\Win* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*ARM* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\mac* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\Linux_32bit -Recurse -Force -Confirm:$false
7z a  -ttar ..\..\..\..\..\..\exInstaller-linux-x64.tar
cd ..\..\..\..\..\..
echo 7z a .\exInstaller.tar -tgzip .\exInstaller-linux-x64.tar.gz
echo del exInstaller.tar
cd "exInstaller\bin\Release\netcoreapp5.0\win-x86\publish"
Remove-Item arduino-cli-runtimes\Linux* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\mac* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*_64bit -Recurse -Force -Confirm:$false
7z a  -tzip ..\..\..\..\..\..\exInstaller-win-x86.zip
cd ..\..\..\..\..\..
cd "exInstaller\bin\Release\netcoreapp5.0\win-x64\publish"
Remove-Item arduino-cli-runtimes\Linux* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\mac* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\*_32bit -Recurse -Force -Confirm:$false
7z a  -tzip ..\..\..\..\..\..\exInstaller-win-x64.zip
cd ..\..\..\..\..\..
cd "exInstaller\bin\Release\netcoreapp5.0\osx-x64\publish"
Remove-Item arduino-cli-runtimes\Linux* -Recurse -Force -Confirm:$false
Remove-Item arduino-cli-runtimes\Win* -Recurse -Force -Confirm:$false
7z a  -ttar ..\..\..\..\..\..\exInstaller-osx-x64.tar
cd ..\..\..\..\..\..

