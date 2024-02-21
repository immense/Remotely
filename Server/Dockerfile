FROM mcr.microsoft.com/dotnet/aspnet:8.0

EXPOSE ${ASPNETCORE_HTTP_PORTS}

RUN apt -y update && apt -y install curl
COPY Server/bin/publish /app

WORKDIR /app

ENTRYPOINT ["dotnet", "Remotely_Server.dll"]

HEALTHCHECK --interval=5m --timeout=3s \
  CMD curl -f http://localhost:${ASPNETCORE_HTTP_PORTS}/api/healthcheck || exit 1