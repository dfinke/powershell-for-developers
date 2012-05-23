$url = 'http://search.twitter.com/search.json?q=powershell'
# (Invoke-RestMethod $url).results
(irm $url).results