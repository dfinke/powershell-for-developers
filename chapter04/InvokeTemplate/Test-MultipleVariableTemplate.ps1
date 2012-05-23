# dot source it
. .\Invoke-Template.ps1

Invoke-Template "$pwd\etc" {

	$type = "string"
	$name = "FirstName"
	Get-Template properties.txt
}
