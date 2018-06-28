FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY ECA.Services.Document.Signature.sln ./
COPY ../ECA.Services.Document.Signature/ECA.Services.Document.Signature.csproj ../ECA.Services.Document.Signature/
COPY ../ECA.Services.Document.Signature.DAL/ECA.Services.Document.Signature.DAL.csproj ../ECA.Services.Document.Signature.DAL/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/../ECA.Services.Document.Signature
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ECA.Services.Document.Signature.dll"]
