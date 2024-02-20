> dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput="./../TestResults/"
> reportgenerator.exe "-reports:TestResults\coverage.cobertura.xml" "-targetdir:TestResults\html" "-reporttypes:HTML;"
