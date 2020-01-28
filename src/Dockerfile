FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build

RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app
COPY . .

WORKDIR /usr/src/app/EmailRouter.Service
RUN dotnet build
RUN dotnet publish -o ./publish -c Release

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-alpine AS production

RUN mkdir -p /opt/app
WORKDIR /opt/app

COPY --from=build /usr/src/app/EmailRouter.Service/publish .

ENTRYPOINT [ "dotnet", "EmailRouter.Service.dll" ]