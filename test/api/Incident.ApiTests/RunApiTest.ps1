param(
    [string]$MailtoAddress,
	[string]$MailSubject,
	[string]$Mailbody,
	[string]$AttachmentFile,
	[string]$MailPriority,
	[string]$URL,
    [string]$UserName,
    [string]$Password
)

$resultContent =  dotnet vstest Incident.ApiTests.dll
Set-Content -Path .\TestResult.txt -Value $resultContent
#send mail
if($resultContent -like "*Failed*"){
    ..\..\assets\SendEmail.ps1 -MailtoAddress "$MailtoAddress" -MailSubject "Incident Gateway API test failed on $URL" -MailPriority "High" -AttachmentFile ".\TestResult.txt"
    exit 0
}
else{
    ..\..\assets\SendEmail.ps1 -MailtoAddress "$MailtoAddress" -MailSubject "Incident Gateway API test passed on $URL" -MailPriority "Normal" -AttachmentFile ".\TestResult.txt"
    exit 1
}


