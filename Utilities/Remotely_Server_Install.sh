#!/bin/bash
echo "Thanks for trying Remotely!"
echo

if [ "$EUID" -ne 0 ]
  then echo "Please run as root"
  exit
fi

read -p "Enter path where the Remotely server files should be installed (typically /var/www/remotely): " appRoot
if [ -z "$appRoot" ]; then
    appRoot="/var/www/remotely"
fi

echo
echo "Enter a server host. If you're using a LAN, put the IP Address from the server host. For WAN, put the domain name."
read -p "Enter server host (e.g. remotely.yourdomainname.com): " serverHost

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

echo "-- Installing .NET Core and Dependencies..."
{
# Install .NET Core Runtime.
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
        wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb
fi
dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
add-apt-repository universe
apt-get update
apt-get -y install apt-transport-https aspnetcore-runtime-3.1

 # Install dependencies
apt-get -y install unzip acl libc6-dev libgdiplus nginx
} &> /dev/null

# Download and install Remotely files.
echo "-- Installing Remotely Server..."
{
if [ ! -f "./Remotely_Server_Linux-x64.zip" ]; then
	wget "https://github.com/Jay-Rad/Remotely/releases/latest/download/Remotely_Server_Linux-x64.zip"
fi
mkdir -p $appRoot
unzip -o Remotely_Server_Linux-x64.zip -d $appRoot
rm ./Remotely_Server_Linux-x64.zip
setfacl -R -m u:www-data:rwx $appRoot
chown -R www-data:www-data $appRoot
} &> /dev/null

echo "-- Configuring Nginx Service..."
{
# Enabling Nginx
systemctl enable nginx
systemctl stop nginx

# Configure Nginx
nginxConfig="server {
    listen        80;
    server_name   $serverHost *.$serverHost;
    location / {
        proxy_pass         http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade \$http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host \$host;
        proxy_cache_bypass \$http_upgrade;
        proxy_set_header   X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto \$scheme;
    }

	location /BrowserHub {
		proxy_pass http://localhost:5000;
		proxy_http_version 1.1;
		proxy_set_header Upgrade \$http_upgrade;
		proxy_set_header Connection \"upgrade\";
		proxy_set_header Host \$host;
		proxy_cache_bypass \$http_upgrade;
	}
	location /AgentHub {
		proxy_pass http://localhost:5000;
		proxy_http_version 1.1;
		proxy_set_header Upgrade \$http_upgrade;
		proxy_set_header Connection \"upgrade\";
		proxy_set_header Host \$host;
		proxy_cache_bypass \$http_upgrade;
	}

	location /RCBrowserHub {
		proxy_pass http://localhost:5000;
		proxy_http_version 1.1;
		proxy_set_header Upgrade \$http_upgrade;
		proxy_set_header Connection \"upgrade\";
		proxy_set_header Host \$host;
		proxy_cache_bypass \$http_upgrade;
	}
	location /CasterHub {
		proxy_pass http://localhost:5000;
		proxy_http_version 1.1;
		proxy_set_header Upgrade \$http_upgrade;
		proxy_set_header Connection \"upgrade\";
		proxy_set_header Host \$host;
		proxy_cache_bypass \$http_upgrade;
	}
}"

echo "$nginxConfig" > /etc/nginx/sites-available/remotely
ln -s /etc/nginx/sites-available/remotely /etc/nginx/sites-enabled/remotely

systemctl start nginx

# Test config & reload.
nginx -t
nginx -s reload
} &> /dev/null

# Create service.
echo "-- Adding Remotely Server Service..."
{
serviceConfig="[Unit]
Description=Remotely Server

[Service]
WorkingDirectory=$appRoot
ExecStart=/usr/bin/dotnet $appRoot/Remotely_Server.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
SyslogIdentifier=remotely
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target"

echo "$serviceConfig" > /etc/systemd/system/remotely.service
} &> /dev/null

echo "-- Enabling Remotely Server Service..."
{
# Enable service & Start.
systemctl enable remotely.service
systemctl restart remotely.service
} &> /dev/null

# Install Certbot and get SSL cert.
echo
echo "Install Certbot? If you're using a local network don't need this. Else if you need to use in WAN you need a domain assigned"
while true; do
    read -p "Do you wish to install this program? " yn
    case $yn in
        [Yy]* ) apt-get -y install certbot python3-certbot-nginx; certbot --nginx;;
        [Nn]* ) exit;;
        * ) echo "Please answer yes or no.";;
    esac
done
