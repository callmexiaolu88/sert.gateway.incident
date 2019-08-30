::bat for copy ps to generate UT report
set SourcePath="%cd%\..\..\..\..\sert.devops\Dev\GenerateReport.ps1"
set DestinationPath="%cd%"

xcopy %SourcePath% %DestinationPath% /e /y /I

powershell -executionpolicy remotesigned -File "GenerateReport.ps1"

del GenerateReport.ps1
