$url  = "http://feeds.feedburner.com/DevelopmentInABlink"
$feed = (New-Object Net.WebClient).DownloadString($url)
([xml]$feed).rss.channel.item | Select title, pubDate