FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8181

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]

COPY ["EducationContentService.Web/EducationContentService.Web.csproj", "EducationContentService.Web/"]
COPY ["EducationContentService.Infrastructure.Postgres/EducationContentService.Infrastructure.Postgres.csproj", "EducationContentService.Infrastructure.Postgres/"]
COPY ["EducationContentService.UseCases/EducationContentService.UseCases.csproj", "EducationContentService.UseCases/"]
COPY ["EducationContentService.Domain/EducationContentService.Domain.csproj", "EducationContentService.Domain/"]

RUN dotnet restore "EducationContentService.Web/EducationContentService.Web.csproj"

COPY . .
WORKDIR "/src/EducationContentService.Web"
RUN dotnet build "./EducationContentService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EducationContentService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EducationContentService.Web.dll"]
