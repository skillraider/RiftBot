FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RiftBot/RiftBot.csproj", "RiftBot/"]
COPY ["RiftBot.Database/RiftBot.Database.csproj", "RiftBot.Database/"]
COPY ["RiftBot.Types/RiftBot.Types.csproj", "RiftBot.Types/"]
COPY ["RunescapeApi/RunescapeApi.csproj", "RunescapeApi/"]
RUN dotnet restore "RiftBot/RiftBot.csproj"
COPY . .
WORKDIR "/src/RiftBot"
RUN dotnet build "RiftBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RiftBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RiftBot.dll"]