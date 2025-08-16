FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source
EXPOSE 80
EXPOSE 443



COPY . .
RUN dotnet restore "./AIHarmony.csproj" --disable-parallel
RUN dotnet publish "./AIHarmony.csproj" -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/sdk:6.0 
WORKDIR /app
COPY --from=build /app ./
EXPOSE 80
EXPOSE 443



ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT [ "dotnet", "AIHarmony.dll" ]

