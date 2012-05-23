# dot source it
. .\Invoke-Template.ps1

Invoke-Template "$pwd\etc" {

	([xml](Get-Content .\Properties.xml)).Properties.property |
		ForEach {
			$type=$_.type; $name=$_.name
			Get-Template properties.txt
		}
}
