FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Directory.build.props", "Directory.build.props"]
COPY ["src/ConsoleApp/ConsoleApp.csproj", "src/ConsoleApp/"]
COPY ["src/SharedLibrary/SharedLibrary.csproj", "src/SharedLibrary/"]
RUN dotnet restore "src/ConsoleApp/ConsoleApp.csproj"
COPY . .

RUN dotnet publish "src/ConsoleApp/ConsoleApp.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ConsoleApp.dll"]