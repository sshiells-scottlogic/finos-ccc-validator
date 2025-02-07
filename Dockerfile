FROM mcr.microsoft.com/dotnet/sdk:9.0 as build-env

WORKDIR /app
COPY . ./
RUN dotnet publish "./Finos.CCC.Validator/Finos.CCC.Validator.csproj" -c Release -o out --no-self-contained

LABEL maintainer="Stevie Shiells <sshiells@scottlogic.com>"
LABEL repository="https://github.com/sshiells-scottlogic/finos-ccc-validator"
LABEL homepage="https://github.com/sshiells-scottlogic/finos-ccc-validator"

LABEL com.github.actions.name="FINOS CCC Validator"
LABEL com.github.actions.description="A GitHub action to validate the content the services listed under the FINOS Common Cloud Controls (CCC) project."
LABEL com.github.actions.icon="check-square"
LABEL com.github.actions.color="blue"

FROM mcr.microsoft.com/dotnet/runtime:9.0
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Finos.CCC.Validator.dll"]