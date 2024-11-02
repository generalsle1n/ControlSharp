FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ControlSharp.Ui/ControlSharp.Ui.csproj", "ControlSharp.Ui/"]
RUN dotnet restore "ControlSharp.Ui/ControlSharp.Ui.csproj"
COPY . .
WORKDIR "/src/ControlSharp.Ui"
RUN dotnet build "ControlSharp.Ui.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ControlSharp.Ui.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ControlSharp.Ui.dll"]
