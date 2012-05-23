$ie = New-Object -ComObject InternetExplorer.Application
$ie.Navigate2('http:\\www.bing.com')
$ie.Visible = $true