FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy

SHELL ["/bin/bash", "-c"]

EXPOSE 5000

ENV ASPNETCORE_ENVIRONMENT="Production"
ENV ASPNETCORE_URLS="http://*:5000"

COPY /_immense.Remotely/Server/DockerMain.sh .
COPY /_immense.Remotely/Server/linux-x64/Server /app

WORKDIR /app

RUN \
  mkdir -p /remotely-data && \
  sed -i 's/DataSource=Remotely.db/DataSource=\/remotely-data\/Remotely.db/' /app/appsettings.json

VOLUME "/remotely-data"

RUN chmod +x "/src/DockerMain.sh"

ENTRYPOINT ["/src/DockerMain.sh"]