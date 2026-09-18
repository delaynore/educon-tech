FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release
ARG GITHUB_USERNAME

WORKDIR /src

COPY ["EducationContentService/Directory.Build.props", "."]
COPY ["EducationContentService/Directory.Packages.props", "."]
COPY ["nuget.config", "."]

COPY ["EducationContentService/EducationContentService.Contracts/EducationContentService.Contracts.csproj", "EducationContentService.Contracts/"]
COPY ["EducationContentService/EducationContentService.Web/EducationContentService.Web.csproj", "EducationContentService.Web/"]
COPY ["EducationContentService/EducationContentService.Infrastructure.Postgres/EducationContentService.Infrastructure.Postgres.csproj", "EducationContentService.Infrastructure.Postgres/"]
COPY ["EducationContentService/EducationContentService.UseCases/EducationContentService.UseCases.csproj", "EducationContentService.UseCases/"]
COPY ["EducationContentService/EducationContentService.Domain/EducationContentService.Domain.csproj", "EducationContentService.Domain/"]

RUN --mount=type=secret,id=github_packages_token \
	dotnet nuget update source github \
		--username "$GITHUB_USERNAME" \
		--password $(cat /run/secrets/github_packages_token) \
		--store-password-in-clear-text

RUN dotnet restore "EducationContentService.Web/EducationContentService.Web.csproj"

COPY EducationContentService .

RUN dotnet tool install --global dotnet-ef --version 10.0.12
ENV PATH="/root/.dotnet/tools:${PATH}"

WORKDIR "/src/EducationContentService.Web"

RUN dotnet ef migrations bundle \
    --project "../EducationContentService.Infrastructure.Postgres/EducationContentService.Infrastructure.Postgres.csproj" \
    --startup-project "./EducationContentService.Web.csproj" \
    --output /app/efbundle \
    --configuration $BUILD_CONFIGURATION \
    --verbose

RUN dotnet publish "./EducationContentService.Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
COPY --from=build /app/efbundle .

COPY EducationContentService/entrypoint.sh /entrypoint.sh

RUN chmod +x /entrypoint.sh /app/efbundle

USER $APP_UID

ENTRYPOINT ["/entrypoint.sh"]
