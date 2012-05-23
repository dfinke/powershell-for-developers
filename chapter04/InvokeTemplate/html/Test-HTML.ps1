. ..\Invoke-Template.ps1

Invoke-Template {
    $name = "Test"
    
    dir | 
        select Name, Length, LastWriteTime | 
            ConvertTo-Html -Head (Get-Template header.htm) -Body (Get-Template body.htm) -Title $name

} | Set-Content .\test.htm

Invoke-Item .\test.htm