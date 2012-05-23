. .\Get-WebData.ps1

$url = "http://www.cpsc.gov/cpscpub/prerel/prerel.xml"
(Get-WebData $url).rss.channel.item