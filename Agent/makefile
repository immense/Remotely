
# Notes:
# Possibly use the automake suite to build this, but since this is package is geared towards both Windows and Linux, I do not think it prudent.

currentversion=2021.03.04

all:
	@ echo "This is help with compiling the Agent. This assumes you have the dotnet application installed."
	@ echo "You should now run: make build"

build:
	#dotnet build Agent.csproj
	dotnet publish /p:Version=$(currentversion) /p:FileVersion=$(currentversion) --runtime linux-x64 --configuration Release -p:PublishTrimmed=True --output ".\bin\Release\net5.0\linux-x64\publish" "..\Agent"

package:
	    zip -r linux-x64.zip ./bin/Release/net5.0/linux-x64.zip


