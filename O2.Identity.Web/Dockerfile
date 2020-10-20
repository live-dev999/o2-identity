#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY Services/o2-identity/O2.Identity.Web/O2.Identity.Web.csproj Services/o2-identity/O2.Identity.Web/
RUN dotnet restore "Services/o2-identity/O2.Identity.Web/O2.Identity.Web.csproj"
COPY . .
WORKDIR "/src/Services/o2-identity/O2.Identity.Web"
RUN dotnet build "O2.Identity.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "O2.Identity.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "O2.Identity.Web.dll"]
