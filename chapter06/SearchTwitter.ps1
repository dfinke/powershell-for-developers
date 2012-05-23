. .\Get-WebData.ps1

$result = Get-WebData "http://search.twitter.com/search.rss?q=PowerShell"
$result.rss.channel.item | select -First 1 #| 
    #Select title, author