FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /src
COPY *.sln .
COPY 0.CoreLayer/Core.Domain/*.csproj 0.CoreLayer/Core.Domain/
COPY 1.PresentationLayer/Website/*.csproj 1.PresentationLayer/Website/
COPY 2.BusinessLogicLayer/BL.Service/*.csproj 2.BusinessLogicLayer/BL.Service/
COPY 2.BusinessLogicLayer/BL.Service.Tests/*.csproj 2.BusinessLogicLayer/BL.Service.Tests/
COPY 3.DataAccessLayer/DA/DA.Managers/*.csproj 3.DataAccessLayer/DA/DA.Managers/
COPY 3.DataAccessLayer/DA/DA.Repositories/*.csproj 3.DataAccessLayer/DA/DA.Repositories/
RUN dotnet restore

COPY . .
RUN dotnet build "1.PresentationLayer/Website/Website.csproj" -c Release -o /build
RUN dotnet test "2.BusinessLogicLayer/BL.Service.Tests" -c Release
RUN dotnet publish "1.PresentationLayer/Website/Website.csproj" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /publish
COPY --from=build-env /publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Website.dll"]
