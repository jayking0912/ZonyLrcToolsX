#docker build -t roonlyric -f Dockerfile .
#docker run -it -v /music:/music roonlyric:latest
#添加.net6基础镜像
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
VOLUME ["/music"]
WORKDIR /app

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "ZonyLrcTools.Cli.dll"]