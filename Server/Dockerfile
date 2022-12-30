FROM mcr.microsoft.com/dotnet/aspnet:6.0

EXPOSE 5000

ENV ASPNETCORE_ENVIRONMENT="Production"
ENV ASPNETCORE_URLS="http://*:5000"

WORKDIR /src

RUN \
  if [ ! -f ./_immense.Remotely/Server/linux-x64/Server.zip ] \
  then \
    echo "Build artifact not found at path './_immense.Remotely/Server/linux-x64/Server.zip'" \
    exit 1 \
  fi \

RUN apt-get -y install unzip
RUN unzip -o ./_immense.Remotely/Server/linux-x64/Server.zip -d /app

WORKDIR /app

RUN \
  mkdir -p /remotely-data && \
  sed -i 's/DataSource=Remotely.db/DataSource=\/remotely-data\/Remotely.db/' ./appsettings.json

VOLUME "/remotely-data"

COPY --from=build /src/Server/DockerMain.sh ./DockerMain.sh
ENTRYPOINT ["DockerMain.sh"]