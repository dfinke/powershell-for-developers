$url = "http://dougfinke.com/PowerShellForDevelopers/albums.xml"
(Invoke-RestMethod $url).albums.album