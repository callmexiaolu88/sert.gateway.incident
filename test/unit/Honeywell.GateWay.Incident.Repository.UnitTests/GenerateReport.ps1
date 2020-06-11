
if (Test-Path "report\")
{
    ri "report\" -Force -Recurse        
}

$CONF = (Get-Content "TestConfig.json" -Raw) | ConvertFrom-Json

$num = $CONF.Config.Length

$reportGenerator = $ENV:USERPROFILE + '\.nuget\packages\reportgenerator\4.2.15\tools\netcoreapp2.1\ReportGenerator.dll'

for($i=0;$i -lt $num;$i++)
{
   $includeItem = $CONF.Config.Item($i).Include

   $pInclude = $pInclude + "[$includeItem]*" + "%2c"   
}

$resultContent = & dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:Include=$pInclude

#/p:ExcludeByFile=\"../dir1/class1.cs,../dir2/*.cs,../dir3/**/*.cs\" `
#/p:ExcludeByAttribute="Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute" `


dotnet $reportGenerator -reports:coverage.opencover.xml -targetdir:.\report -reporttypes:HTML

if($resultContent -like "*Test Run Successful*"){
$resultContent
    exit 0
}
else{
    "Unit test failed."
    $resultContent
    exit 1
}