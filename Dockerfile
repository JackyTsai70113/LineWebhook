ARG VERSION=6.0

FROM mcr.microsoft.com/dotnet/sdk:$VERSION AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY 0.CoreLayer/Core.Domain/*.csproj 0.CoreLayer/Core.Domain/
COPY 1.PresentationLayer/UI/Website/*.csproj 1.PresentationLayer/UI/Website/
COPY 2.BusinessLogicLayer/BL.Service/*.csproj 2.BusinessLogicLayer/BL.Service/
COPY 2.BusinessLogicLayer/BL.Service.Tests/*.csproj 2.BusinessLogicLayer/BL.Service.Tests/
COPY 3.DataAccessLayer/DA/DA.Managers/*.csproj 3.DataAccessLayer/DA/DA.Managers/
COPY 3.DataAccessLayer/DA/DA.Repositories/*.csproj 3.DataAccessLayer/DA/DA.Repositories/
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet test 2.BusinessLogicLayer/BL.Service.Tests -c Release
RUN dotnet publish 1.PresentationLayer/UI/Website -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:$VERSION AS runtime
WORKDIR /app
COPY --from=build-env /app/out ./
#ENV DOTNET_RUNNING_IN_CONTAINER=true ASPNETCORE_URLS=http://+:8080

###  
#EXPOSE 8080
#ENTRYPOINT ["dotnet", "Website.dll"]
#

### heroku uses the following
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Website.dll
#