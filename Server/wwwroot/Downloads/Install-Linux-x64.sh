#!/bin/bash
HostName=
Organization=
GUID=$(cat /proc/sys/kernel/random/uuid)

systemctl stop remotely-agent
rm -r -f /usr/local/bin/Remotely
rm -f /etc/systemd/system/remotely-agent.service
systemctl daemon-reload

if [ "$1" = "--uninstall" ]; then
	exit
fi

apt-get -y install unzip
apt-get -y install libc6-dev
apt-get -y install libgdiplus
apt-get -y install libxtst-dev
apt-get -y install xclip

mkdir -p /usr/local/bin/Remotely/
cd /usr/local/bin/Remotely/

if [ "$1" = "--path" ]; then
    echo  "Copying install files..."
	cp $2 /usr/local/bin/Remotely/Remotely-Linux.zip
else
    echo  "Downloading client..."
	wget $HostName/Downloads/Remotely-Linux.zip
fi

unzip ./Remotely-Linux.zip
chmod +x ./Remotely_Agent
chmod +x ./ScreenCast/Remotely_ScreenCast.Linux

cat > ./ConnectionInfo.json << EOL
{
	"DeviceID":"$GUID", 
	"Host":"$HostName",
	"OrganizationID": "$Organization",
	"ServerVerificationToken":""
}
EOL


echo Creating service...

cat > /etc/systemd/system/remotely-agent.service << EOL
[Unit]
Description=The Remotely agent used for remote access.

[Service]
WorkingDirectory=/usr/local/bin/Remotely/
ExecStart=/usr/local/bin/Remotely/Remotely_Agent
Restart=always
StartLimitIntervalSec=0
RestartSec=10"

[Install]
WantedBy=graphical.target
EOL

systemctl enable remotely-agent
systemctl start remotely-agent

echo Install complete.