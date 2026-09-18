#!/bin/sh
set -e

echo "Applying database migrations..."

./efbundle

echo "Starting EducationContentService..."

exec dotnet EducationContentService.Web.dll