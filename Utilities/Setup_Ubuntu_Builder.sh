add-apt-repository universe
apt-get update
wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
apt-get install apt-transport-https
apt-get update
apt-get install dotnet-sdk-2.2
apt-get install git

mkdir ~/Downloads/Remotely
cd ~/Downloads/Remotely
git clone https://github.com/Jay-Rad/Remotely.git