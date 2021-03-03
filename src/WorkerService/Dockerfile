FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Directory.build.props", "Directory.build.props"]
COPY ["src/WorkerService/WorkerService.csproj", "src/WorkerService/"]
COPY ["src/SharedLibrary/SharedLibrary.csproj", "src/SharedLibrary/"]
RUN dotnet restore "src/WorkerService/WorkerService.csproj"
COPY . .

FROM build AS publish
RUN dotnet publish "/src/src/WorkerService/WorkerService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkerService.dll"]