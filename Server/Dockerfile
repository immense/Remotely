FROM mcr.microsoft.com/dotnet/aspnet:8.0

EXPOSE ${ASPNETCORE_HTTP_PORTS}

RUN apt -y update && apt -y install curl
RUN mkdir -p /app/AppData
RUN chown app:app -R /app/AppData

COPY Server/bin/publish /app

WORKDIR /app

USER app
ENTRYPOINT ["dotnet", "Remotely_Server.dll"]

HEALTHCHECK --interval=5m --timeout=3s \
  CMD curl -f http://localhost:5000/api/healthcheck || exit 1