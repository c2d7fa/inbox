# https://docs.docker.com/engine/examples/dotnetcore/

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY . ./
RUN cd Server && dotnet publish -c Release -o out

# Runtime:

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/Server/out .
ENTRYPOINT ["dotnet", "Server.dll"]
