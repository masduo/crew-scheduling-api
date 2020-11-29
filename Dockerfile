### BUILD TEST PUBLISH ##############################

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as publish

WORKDIR /sdk

COPY ./src/CrewScheduling.Api.csproj ./src/CrewScheduling.Api.csproj
RUN dotnet restore ./src

COPY ./tests.unit/Tests.Unit.csproj ./tests.unit/
RUN dotnet restore ./tests.unit

COPY ./tests.integration/Tests.Integration.csproj ./tests.integration/
RUN dotnet restore ./tests.integration

COPY . .
RUN dotnet test tests.unit
RUN dotnet test tests.integration

RUN dotnet publish -c Release -o /compiled-app /nologo src

### RUN #####################################

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app

ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000

COPY --from=publish ./compiled-app .

ENTRYPOINT dotnet CrewScheduling.Api.dll