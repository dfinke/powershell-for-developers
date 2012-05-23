$url = "http://dougfinke.com/PowerShellForDevelopers/albums.js"
(Invoke-RestMethod $url)[0].GetType()