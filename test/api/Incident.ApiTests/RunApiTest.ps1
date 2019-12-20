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

$env:ApiTest_environment=cd
$resultContent =  dotnet vstest Incident.ApiTests.dll
Set-Content -Path $AttachmentFile -Value $resultContent
#send mail
if($resultContent -like "*Test Run Successful*"){
    ..\..\assets\SendEmail.ps1 -MailtoAddress "$MailtoAddress" -MailSubject "Gateway---Incident API test Successful on $URL" -MailPriority "Normal" -AttachmentFile "$AttachmentFile"
    exit 0
}
else{
    ..\..\assets\SendEmail.ps1 -MailtoAddress "$MailtoAddress" -MailSubject "Gateway---Incident API test failed on $URL" -MailPriority "High" -AttachmentFile "$AttachmentFile"
    exit 1
}


