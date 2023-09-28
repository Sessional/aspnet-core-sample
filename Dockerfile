FROM mcr.microsoft.com/dotnet/runtime:7.0

COPY output /app
WORKDIR /app
ENTRYPOINT ./LonelyVale.Api
