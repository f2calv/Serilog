FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Directory.build.props", "Directory.build.props"]
COPY ["src/webapi/webapi.csproj", "src/webapi/"]
COPY ["src/SharedLibrary/SharedLibrary.csproj", "src/SharedLibrary/"]
RUN dotnet restore "src/webapi/webapi.csproj"
COPY . .

RUN dotnet publish "src/webapi/webapi.csproj" -c Release -o /app/publish

#can't use alpine for final due to mssql client library issues
#https://github.com/dotnet/SqlClient/issues/220
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "webapi.dll"]