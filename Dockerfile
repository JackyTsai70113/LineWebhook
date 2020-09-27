ARG VERSION=3.1-alpine3.10

FROM mcr.microsoft.com/dotnet/core/sdk:$VERSION AS build-env
WORKDIR /app

ADD *.sln .
ADD 0.CoreLayer/Core.Domain/*.csproj 0.CoreLayer/Core.Domain/
ADD 1.PresentationLayer/UI/Website/*.csproj 1.PresentationLayer/UI/Website/
ADD 2.BusinessLogicLayer/BL/BL.Services/*.csproj 2.BusinessLogicLayer/BL/BL.Services/
ADD 3.DataAccessLayer/DA/DA.Managers/*.csproj 3.DataAccessLayer/DA/DA.Managers/
ADD 3.DataAccessLayer/DA/DA.Repositories/*.csproj 3.DataAccessLayer/DA/DA.Repositories/
RUN dotnet restore

# Copy everything else and build
ADD . ./
RUN dotnet publish -c Release -o ./output

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:$VERSION
WORKDIR /app
COPY --from=build-env /app/output .
#ENV DOTNET_RUNNING_IN_CONTAINER=true ASPNETCORE_URLS=http://+:8080

###  
#EXPOSE 8080
#ENTRYPOINT ["dotnet", "Website.dll"]
#

### heroku uses the following
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Website.dll
#