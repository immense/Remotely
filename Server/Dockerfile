FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy

USER app

SHELL ["/bin/bash", "-c"]

EXPOSE 5000
EXPOSE 5001

COPY /_immense.Remotely/Server/linux-x64/Server /app

WORKDIR /app

RUN \
  apt-get -y update && \
  apt-get -y install curl && \
  mkdir -p /remotely-data && \
  sed -i 's/DataSource=Remotely.db/DataSource=\/app\/AppData\/Remotely.db/' /app/appsettings.json

ENTRYPOINT ["dotnet", "Remotely_Server.dll"]

HEALTHCHECK --interval=5m --timeout=3s \
  CMD curl -f http://localhost:5000/api/healthcheck || exit 1