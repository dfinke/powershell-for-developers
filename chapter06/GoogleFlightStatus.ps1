function Get-FlightStatus {

	param($query="dl269")

	$result = Invoke-WebRequest "https://www.google.com/search?q=flight status for $query"
	$result.AllElements |
        Where Class -eq "obcontainer" |
        Select -ExpandProperty innerText
}

Get-FlightStatus