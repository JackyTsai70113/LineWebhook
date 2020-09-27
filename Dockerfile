FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY 0.CoreLayer/Core.Domain/*.csproj 0.CoreLayer/Core.Domain/
COPY 1.PresentationLayer/UI/Website/*.csproj 1.PresentationLayer/UI/Website/
COPY 2.BusinessLogicLayer/BL/BL.Services/*.csproj 2.BusinessLogicLayer/BL/BL.Services/
COPY 3.DataAccessLayer/DA/DA.Managers/*.csproj 3.DataAccessLayer/DA/DA.Managers/
COPY 3.DataAccessLayer/DA/DA.Repositories/*.csproj 3.DataAccessLayer/DA/DA.Repositories/
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/publish .
# ENTRYPOINT ["dotnet", "LineWebhook.dll"]
# heroku uses the following
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Website.dll