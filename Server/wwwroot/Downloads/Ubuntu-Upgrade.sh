#!/bin/bash

AppRoot=$(cat /etc/systemd/system/remotely.service | grep -i "execstart" | cut -d' ' -f 2 | sed -e 's/\/[^\/]*$/\//')

echo "Remotely server upgrade started."

echo "Target path: $AppRoot"

read -p "If this is not correct, press Ctrl + C now to abort!"

echo "Ensuring dependencies are installed."

 # Install other prerequisites.
apt-get -y install unzip
apt-get -y install acl


echo "Downloading latest Remotely package."
# Download and install Remotely files.
mkdir -p $AppRoot
wget "https://github.com/lucent-sea/Remotely/releases/latest/download/Remotely_Server_Linux-x64.zip"
unzip -o Remotely_Server_Linux-x64.zip -d $AppRoot
rm Remotely_Server_Linux-x64.zip
setfacl -R -m u:www-data:rwx $AppRoot
chown -R "$USER":www-data $AppRoot

# Restart service.
systemctl restart remotely.service

echo "Update complete."
