FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ./../shoe-grab-admin/*.csproj ./shoe-grab-admin/
COPY ./../shoe-grab-common/ ./shoe-grab-common/
COPY ./../shoe-grab-admin/ ./shoe-grab-admin/

RUN dotnet restore ./shoe-grab-admin/*.csproj
RUN dotnet publish ./shoe-grab-admin/*.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /out .
EXPOSE 10042
ENTRYPOINT ["dotnet", "shoe-grab-admin.dll"]
