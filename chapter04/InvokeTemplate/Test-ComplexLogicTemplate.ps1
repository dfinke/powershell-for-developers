# dot source it
. .\Invoke-Template.ps1

Invoke-Template "$pwd\etc" {

	Import-Csv $pwd\properties.csv | ForEach {

		$type=$_.type; $name=$_.name
		Get-Template properties.txt
	}
}