# specifies the base image on which we want our image to be built.
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
# sets the working directory or context inside the image
WORKDIR /app
EXPOSE 80
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy content and restore as distinct layers
COPY *.sln .
COPY 0.CoreLayer/Core.Domain/*.csproj 0.CoreLayer/Core.Domain/
COPY 1.PresentationLayer/UI/Website/*.csproj 1.PresentationLayer/UI/Website/
COPY 2.BusinessLogicLayer/BL.Service/*.csproj 2.BusinessLogicLayer/BL.Service/
COPY 2.BusinessLogicLayer/BL.Service.Tests/*.csproj 2.BusinessLogicLayer/BL.Service.Tests/
COPY 3.DataAccessLayer/DA/DA.Managers/*.csproj 3.DataAccessLayer/DA/DA.Managers/
COPY 3.DataAccessLayer/DA/DA.Repositories/*.csproj 3.DataAccessLayer/DA/DA.Repositories/
RUN dotnet restore

# copy and publish app and libraries
COPY . .
WORKDIR "/src/."
RUN dotnet build "1.PresentationLayer/UI/Website/Website.csproj" -c Release -o /app/build
RUN dotnet test "2.BusinessLogicLayer/BL.Service.Tests" -c Release
RUN dotnet publish "1.PresentationLayer/UI/Website/Website.csproj" -c Release -o /app/publish

# Build runtime image
FROM base AS final
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Website.dll"]
