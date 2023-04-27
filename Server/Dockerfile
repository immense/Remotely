FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy

SHELL ["/bin/bash", "-c"]

EXPOSE 5000

ENV ASPNETCORE_ENVIRONMENT="Production"
ENV ASPNETCORE_URLS="http://*:5000"

WORKDIR /src

COPY /_immense.Remotely/Server/DockerMain.sh .
COPY /_immense.Remotely/Server/linux-x64/Server.zip .

RUN \
  apt-get -y update && \
  apt-get -y install unzip && \
  unzip -o ./Server.zip -d /app && \
  rm ./Server.zip

WORKDIR /app

RUN \
  mkdir -p /remotely-data && \
  sed -i 's/DataSource=Remotely.db/DataSource=\/remotely-data\/Remotely.db/' ./appsettings.json

VOLUME "/remotely-data"

RUN chmod +x "/src/DockerMain.sh"

ENTRYPOINT ["/src/DockerMain.sh"]