# NuGet restore
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY *.sln .
COPY 1.PresentationLayer/UI/Website/*.csproj 1.PresentationLayer/UI/Website/ 
COPY 5.CrossCuttingLayer/Utility/*.csproj 5.CrossCuttingLayer/Utility/
RUN dotnet restore
COPY . .

# publish
FROM build AS publish
WORKDIR /src
RUN dotnet publish -c Release -o /src/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=publish /src/publish .
# ENTRYPOINT ["dotnet", "LineWebhook.dll"]
# heroku uses the following
CMD ASPNETCORE_URLS=http://*:$PORT dotnet 1.PresentationLayer/UI/Website/bin/Debug/netcoreapp3.1/Website.dll