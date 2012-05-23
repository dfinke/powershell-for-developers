$url = "http://dougfinke.com/PowerShellForDevelopers/albums.csv"
Invoke-RestMethod $url | ConvertFrom-Csv