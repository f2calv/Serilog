FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Directory.build.props", "Directory.build.props"]
COPY ["src/ConsoleApp/ConsoleApp.csproj", "src/ConsoleApp/"]
COPY ["src/SharedLibrary/SharedLibrary.csproj", "src/SharedLibrary/"]
RUN dotnet restore "src/ConsoleApp/ConsoleApp.csproj"
COPY . .

FROM build AS publish
RUN dotnet publish "/src/src/ConsoleApp/ConsoleApp.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConsoleApp.dll"]