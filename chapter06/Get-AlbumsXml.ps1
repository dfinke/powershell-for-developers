. .\Get-WebData.ps1

$url = "http://dougfinke.com/PowerShellForDevelopers/albums.xml"
(Get-WebData $url).albums.album