# https://docs.docker.com/engine/examples/dotnetcore/

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY . ./
RUN cd Server && dotnet publish -c Release -o out

# Runtime:

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/Server/out .
ENTRYPOINT ["dotnet", "Server.dll"]
