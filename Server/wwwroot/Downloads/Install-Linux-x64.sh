#!/bin/bash
HostName=
Organization=
GUID=$(cat /proc/sys/kernel/random/uuid)
ETag=

if [ "$EUID" -ne 0 ]
  then echo "Please run as root"
  exit
fi

if [ "$1" = "--uninstall" ]; then
	echo "--- Uninstalling..."
	systemctl stop remotely-agent
	rm -r -f /usr/local/bin/Remotely
	rm -f /etc/systemd/system/remotely-agent.service
	systemctl daemon-reload
	exit
fi

echo "--- Installing .NET Core and Dependencies..."
{
if [ -f /etc/os-release ]; then
    . /etc/os-release
    OS=$NAME
    VER=$VERSION_ID
elif type lsb_release >/dev/null 2>&1; then
    OS=$(lsb_release -si)
    VER=$(lsb_release -sr)
elif [ -f /etc/lsb-release ]; then
    . /etc/lsb-release
    OS=$DISTRIB_ID
    VER=$DISTRIB_RELEASE
elif [ -f /etc/debian_version ]; then
    OS=Debian
    VER=$(cat /etc/debian_version)
fi

if [ -f "./packages-microsoft-prod.deb" ]; then
        echo "File downloaded. Not needed to download"
elif [ "$OS" = "Ubuntu" ]; then
        echo "Ubuntu OS Detected"
        wget -q https://packages.microsoft.com/config/ubuntu/$VER/packages-microsoft-prod.deb
elif [ "$OS" = "Debian GNU/Linux" ] && [ "$VER" = "9" ]; then
        echo "Debian OS 9 Detected"
        wget -O - https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.asc.gpg
        sudo mv microsoft.asc.gpg /etc/apt/trusted.gpg.d/
        wget https://packages.microsoft.com/config/debian/$VER/prod.list
        sudo mv prod.list /etc/apt/sources.list.d/microsoft-prod.list
        sudo chown root:root /etc/apt/trusted.gpg.d/microsoft.asc.gpg
        sudo chown root:root /etc/apt/sources.list.d/microsoft-prod.list
elif [ "$OS" = "Debian GNU/Linux" ] && [ "$VER" = "10" ]; then
	echo "Debian OS 10 Detected"
        wget -q https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb
fi
dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
add-apt-repository universe
apt-get update
apt-get -y install apt-transport-https aspnetcore-runtime-3.1

apt-get -y install libx11-dev unzip libc6-dev libgdiplus libxtst-dev xclip jq curl
} &> /dev/null

echo "--- Check if installed and removing old installations..."
{
if [ -f "/usr/local/bin/Remotely/ConnectionInfo.json" ]; then
	GUID=`cat "/usr/local/bin/Remotely/ConnectionInfo.json" | jq -r '.DeviceID'`
fi

rm -r -f /usr/local/bin/Remotely
rm -f /etc/systemd/system/remotely-agent.service

mkdir -p /usr/local/bin/Remotely/
cd /usr/local/bin/Remotely/
} &> /dev/null

if [ "$1" = "--path" ]; then
    echo  "--- Copying install files..."
	cp $2 /usr/local/bin/Remotely/Remotely-Linux.zip
else
    echo  "---- Downloading client..."
	wget -q $HostName/Downloads/Remotely-Linux.zip
fi
echo "-- Installing Remotely Client..."
{
unzip ./Remotely-Linux.zip
chmod +x ./Remotely_Agent
chmod +x ./Desktop/Remotely_Desktop

} &> /dev/null
echo "-- Configurating Remotely Client..."
{
connectionInfo="{
	\"DeviceID\":\"$GUID\", 
	\"Host\":\"$HostName\",
	\"OrganizationID\": \"$Organization\",
	\"ServerVerificationToken\":\"\"
}"

echo "$connectionInfo" > ./ConnectionInfo.json

curl --head $HostName/Downloads/Remotely-Linux.zip | grep ETag | cut -d' ' -f 2 > ./etag.txt
} &> /dev/null

echo "-- Creating service..."
{
serviceConfig="[Unit]
Description=The Remotely agent used for remote access.

[Service]
WorkingDirectory=/usr/local/bin/Remotely/
ExecStart=/usr/local/bin/Remotely/Remotely_Agent
Restart=always
StartLimitIntervalSec=0
RestartSec=10

[Install]
WantedBy=graphical.target"

echo "$serviceConfig" > /etc/systemd/system/remotely-agent.service

systemctl enable remotely-agent
systemctl restart remotely-agent
} &> /dev/null
echo "-- Install complete."
