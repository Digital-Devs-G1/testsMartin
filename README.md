Coverlet
--------

1) dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput="./../TestResults/"
2) reportgenerator.exe "-reports:TestResults\coverage.cobertura.xml" "-targetdir:TestResults\html" "-reporttypes:HTML;"
