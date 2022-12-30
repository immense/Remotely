FROM mcr.microsoft.com/dotnet/aspnet:6.0
#FROM mcr.microsoft.com/dotnet/sdk:6.0

EXPOSE 5000

ENV ASPNETCORE_ENVIRONMENT="Production"
ENV ASPNETCORE_URLS="http://*:5000"

WORKDIR /src

COPY /.docker/DockerMain.sh /src
COPY /_immense.Remotely/Server/linux-x64/Server.zip /src

RUN \
  apt-get update && \
  apt-get -y install unzip && \
  unzip -o /src/Server.zip -d /app

WORKDIR /app

RUN \
  mkdir -p /remotely-data && \
  sed -i 's/DataSource=Remotely.db/DataSource=\/remotely-data\/Remotely.db/' ./appsettings.json

VOLUME "/remotely-data"

ENTRYPOINT ["/src/DockerMain.sh"]