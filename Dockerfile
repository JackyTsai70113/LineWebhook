# NuGet restore
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY *.sln .
COPY 0.CoreLayer/Core.Domain/*.csproj 0.CoreLayer/Core.Domain/
COPY 1.PresentationLayer/UI/Website/*.csproj 1.PresentationLayer/UI/Website/
COPY 2.BusinessLogicLayer/BL/BL.Interfaces/*.csproj 2.BusinessLogicLayer/BL/BL.Interfaces/
COPY 2.BusinessLogicLayer/BL/BL.Services/*.csproj 2.BusinessLogicLayer/BL/BL.Services/
COPY 3.DataAccessLayer/DA/DA.Managers/*.csproj 3.DataAccessLayer/DA/DA.Managers/
COPY 3.DataAccessLayer/DA/DA.Repositories/*.csproj 3.DataAccessLayer/DA/DA.Repositories/
COPY 4.ModelsLayer/Models/*.csproj 4.ModelsLayer/Models/
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
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Website.dll