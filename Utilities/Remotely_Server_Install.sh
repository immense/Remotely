#!/bin/bash
echo "If you haven't already, publish the Remotely Server app using the 'dotnet publish' 
command (e.g. dotnet publish <path to csproj file> -o <output directory>).  
The output directory is the app root path. This would typically be in /var/www/[appname]/.
"
read -p "Enter app root path: " appRoot
read -p "Enter server host (e.g. example.com): " serverHost

apt-get install acl

setfacl -R -m u:www-data:rwx $appRoot

# Install .NET Core Runtime.
wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
add-apt-repository universe
apt-get install -y apt-transport-https
apt-get update
apt-get install -y aspnetcore-runtime-2.2
rm packages-microsoft-prod.deb


# Install Nginx
apt-get update
apt-get install -y nginx

systemctl start nginx


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
	location /DeviceHub {
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
	location /RCDeviceHub {
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

# Test config.
nginx -t

# Reload.
nginx -s reload




# Create service.

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


# Enable service.
systemctl enable remotely.service
# Start service.
systemctl start remotely.service


# Install Certbot and get SSL cert.
apt-get -y install certbot python-certbot-nginx

certbot --nginx

apt-get -y install ffmpeg

apt-get -y install libgdiplus
