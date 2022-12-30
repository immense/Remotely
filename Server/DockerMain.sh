#!/bin/bash

echo "Entered main script."

RemotelyData=/remotely-data

AppSettingsVolume=/remotely-data/appsettings.json
AppSettingsSrc=/app/appsettings.json

if [ ! -f "$AppSettingsVolume" ]; then
	echo "Copying appsettings.json to volume."
	cp "$AppSettingsSrc" "$AppSettingsVolume"
fi

if [ -f "$AppSettingsSrc" ]; then
	rm "$AppSettingsSrc"
fi

ln -s "$AppSettingsVolume" "$AppSettingsSrc"

echo "Starting Remotely server."
exec /usr/bin/dotnet /app/Remotely_Server.dll