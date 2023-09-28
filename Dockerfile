FROM mcr.microsoft.com/dotnet/aspnet:7.0

COPY output /app
WORKDIR /app
ENTRYPOINT ./LonelyVale.Api
