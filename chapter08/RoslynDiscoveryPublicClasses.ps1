$dll = .\Get-RosylnPath.ps1

Add-Type -Path $dll -PassThru |
    Where {$_.IsPublic -And $_.BaseType} | Sort Name