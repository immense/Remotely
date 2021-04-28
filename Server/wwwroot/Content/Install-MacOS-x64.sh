#!/bin/zsh

HostName=
Organization=
GUID=$(uuidgen)
UpdatePackagePath=""

Args=( "$@" )
ArgLength=${#Args[@]}

for (( i=0; i<${ArgLength}; i+=2 ));
do
    if [ "${Args[$i]}" = "--uninstall" ]; then
        launchctl unload -w /System/Library/LaunchDaemons/remotely-agent.plist
        rm -r -f /Applications/Remotely/
        rm -f /System/Library/LaunchDaemons/remotely-agent.plist
        exit
    elif [ "${Args[$i]}" = "--path" ]; then
        UpdatePackagePath="${Args[$i+1]}"
    fi
done


# Install Homebrew
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
brew update

# Install .NET Runtime
brew install --cask dotnet

# Install dependency for System.Drawing.Common
brew install mono-libgdiplus

# Install other dependencies
brew install wget
brew install curl
brew install jq


if [ -f "/Applications/Remotely/ConnectionInfo.json" ]; then
    GUID=`cat "/Applications/Remotely/ConnectionInfo.json" | jq -r '.DeviceID'`
fi

rm -r -f /Applications/Remotely
rm -f /System/Library/LaunchDaemons/remotely-agent.plist

mkdir -p /Applications/Remotely/
cd /Applications/Remotely/

if [ -z "$UpdatePackagePath" ]; then
    echo  "Downloading client..." >> /tmp/Remotely_Install.log
    wget $HostName/Content/Remotely-MacOS-x64.zip
else
    echo  "Copying install files..." >> /tmp/Remotely_Install.log
    cp "$UpdatePackagePath" /Applications/Remotely/Remotely-MacOS-x64.zip
    rm -f "$UpdatePackagePath"
fi

unzip ./Remotely-MacOS-x64.zip
rm -f ./Remotely-MacOS-x64.zip


connectionInfo="{
    \"DeviceID\":\"$GUID\", 
    \"Host\":\"$HostName\",
    \"OrganizationID\": \"$Organization\",
    \"ServerVerificationToken\":\"\"
}"

echo "$connectionInfo" > ./ConnectionInfo.json

curl --head $HostName/Content/Remotely-MacOS-x64.zip | grep -i "etag" | cut -d' ' -f 2 > ./etag.txt


plistFile="
<?xml version=\"1.0\" encoding=\"UTF-8\"?>
<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">
<plist version=\"1.0\">
<dict>
    <key>Label</key>
    <string>com.translucency.remotely-agent</string>
    <key>ProgramArguments</key>
    <array>
        <string>/usr/bin/dotnet</string>
        <string>/Applications/Remotely/Remotely_Agent.dll</string>
    </array>
    <key>KeepAlive</key>
    <true/>
</dict>
</plist>
"
echo "$plistFile" > "/System/Library/LaunchDaemons/remotely-agent.plist"

launchctl load -w /System/Library/LaunchDaemons/remotely-agent.plist