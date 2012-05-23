# dot source it
. .\Invoke-Template.ps1

Invoke-Template "$pwd\etc" {
    $name = "World"
    Get-Template TestHtml.htm
}