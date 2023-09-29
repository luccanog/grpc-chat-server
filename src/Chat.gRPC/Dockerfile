FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App
EXPOSE 2222 80 
EXPOSE 443

# Copy everything
COPY ./src/Chat.gRPC/ ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "Chat.gRPC.dll"]