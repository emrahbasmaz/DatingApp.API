FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["DatingApp.Api.csproj", "./"]
RUN dotnet restore "DatingApp.Api.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "DatingApp.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DatingApp.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatingApp.Api.dll"]
