#!/bin/bash
echo "Thanks for trying Remotely!"
echo

read -p "Enter path where the Remotely server files should be installed (typically /var/www/remotely): " appRoot
if [ -z "$appRoot" ]; then
    appRoot="/var/www/remotely"
fi

read -p "Enter server host (e.g. remotely.yourdomainname.com): " serverHost

yum update

# Install .NET Core Runtime.
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm

yum -y install apt-transport-https
yum -y update
yum -y install aspnetcore-runtime-5.0


 # Install other prerequisites.
yum -y install https://dl.fedoraproject.org/pub/epel/epel-release-latest-7.noarch.rpm
yum -y install yum-utils
yum-config-manager --enable rhui-REGION-rhel-server-extras rhui-REGION-rhel-server-optional
yum -y install unzip
yum -y install acl
yum -y install libc6-dev
yum -y install libgdiplus


# Download and install Remotely files.
mkdir -p $appRoot
wget "https://github.com/lucent-sea/Remotely/releases/latest/download/Remotely_Server_Linux-x64.zip"
unzip -o Remotely_Server_Linux-x64.zip -d $appRoot
rm Remotely_Server_Linux-x64.zip
setfacl -R -m u:apache:rwx $appRoot
chown -R apache:apache $appRoot


# Install Nginx
yum -y install nginx

systemctl start nginx


# Configure Nginx
nginxConfig="server {
    listen        80;
    server_name   $serverHost *.$serverHost;
    location / {
        proxy_pass         http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade \$http_upgrade;
        proxy_set_header   Connection close;
        proxy_set_header   Host \$host;
        proxy_cache_bypass \$http_upgrade;
        proxy_set_header   X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto \$scheme;
    }

	location /BrowserHub {
		proxy_pass http://localhost:5000;
		proxy_http_version 1.1;
		proxy_set_header Upgrade \$http_upgrade;
		proxy_set_header Connection \"Upgrade\";
		proxy_set_header Host \$host;
		proxy_cache_bypass \$http_upgrade;
	}
	location /AgentHub {
		proxy_pass http://localhost:5000;
		proxy_http_version 1.1;
		proxy_set_header Upgrade \$http_upgrade;
		proxy_set_header Connection \"Upgrade\";
		proxy_set_header Host \$host;
		proxy_cache_bypass \$http_upgrade;
	}

	location /ViewerHub {
		proxy_pass http://localhost:5000;
		proxy_http_version 1.1;
		proxy_set_header Upgrade \$http_upgrade;
		proxy_set_header Connection \"Upgrade\";
		proxy_set_header Host \$host;
		proxy_cache_bypass \$http_upgrade;
	}
	location /CasterHub {
		proxy_pass http://localhost:5000;
		proxy_http_version 1.1;
		proxy_set_header Upgrade \$http_upgrade;
		proxy_set_header Connection \"Upgrade\";
		proxy_set_header Host \$host;
		proxy_cache_bypass \$http_upgrade;
	}
}"

echo "$nginxConfig" > /etc/nginx/conf.d/remotely.conf

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
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target"

echo "$serviceConfig" > /etc/systemd/system/remotely.service


# Enable service.
systemctl enable remotely.service
# Start service.
systemctl start remotely.service

firewall-cmd --permanent --zone=public --add-service=http
firewall-cmd --permanent --zone=public --add-service=https
firewall-cmd --reload

# Install Certbot and get SSL cert.
yum -y install certbot python3-certbot-nginx

certbot --nginx