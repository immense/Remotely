FROM ubuntu:focal

EXPOSE 5000

ENV ASPNETCORE_ENVIRONMENT="Production"
ENV ASPNETCORE_URLS="http://*:5000"


RUN \
  apt-get -y update && \
  apt-get -y install \
  apt-utils \
  wget \
  apt-transport-https \
  unzip \
  acl \
  libssl1.0

RUN \
  wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb && \
  dpkg -i packages-microsoft-prod.deb && \
  apt-get -y update && \
  apt-get -y install aspnetcore-runtime-5.0

RUN \
  mkdir -p /var/www/remotely && \
  mkdir /config && \
  wget -q https://github.com/lucent-sea/Remotely/releases/latest/download/Remotely_Server_Linux-x64.zip && \
  unzip -o Remotely_Server_Linux-x64.zip -d /var/www/remotely && \
  rm Remotely_Server_Linux-x64.zip && \
  setfacl -R -m u:www-data:rwx /var/www/remotely && \
  chown -R www-data:www-data /var/www/remotely

WORKDIR /var/www/remotely
ENTRYPOINT ["/usr/bin/dotnet", "/var/www/remotely/Remotely_Server.dll"]