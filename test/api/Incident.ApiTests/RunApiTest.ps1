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

[environment]::SetEnvironmentvariable("ApiTest_environment","cd", "Machine")
$resultContent =  dotnet vstest Incident.ApiTests.dll
Set-Content -Path $AttachmentFile -Value $resultContent
#send mail
if($resultContent -like "*Test Run Successful*"){
    ..\..\assets\SendEmail.ps1 -MailtoAddress "$MailtoAddress" -MailSubject "[Successed] Gateway---Incident API test on $URL" -MailPriority "Normal" -AttachmentFile "$AttachmentFile"
    exit 0
}
else{
    ..\..\assets\SendEmail.ps1 -MailtoAddress "$MailtoAddress" -MailSubject "[Failed!] Gateway---Incident API test on $URL" -MailPriority "High" -AttachmentFile "$AttachmentFile"
    exit 1
}


