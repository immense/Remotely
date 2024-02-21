FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

EXPOSE ${ASPNETCORE_HTTP_PORTS}
EXPOSE ${ASPNETCORE_HTTPS_PORTS}

RUN apt -y update && apt -y install curl
RUN mkdir -p /app/AppData
RUN chown app:app -R /app/AppData

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Server/Server.csproj", "Server/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["submodules/Immense.RemoteControl/Immense.RemoteControl.Shared/Immense.RemoteControl.Shared.csproj", "submodules/Immense.RemoteControl/Immense.RemoteControl.Shared/"]
COPY ["submodules/Immense.RemoteControl/Immense.RemoteControl.Server/Immense.RemoteControl.Server.csproj", "submodules/Immense.RemoteControl/Immense.RemoteControl.Server/"]
RUN dotnet restore "./Server/./Server.csproj"
COPY . .
WORKDIR "/src/Server"

RUN dotnet build "./Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER app
ENTRYPOINT ["dotnet", "Remotely_Server.dll"]

HEALTHCHECK --interval=5m --timeout=3s \
  CMD curl -f http://localhost:5000/api/healthcheck || exit 1