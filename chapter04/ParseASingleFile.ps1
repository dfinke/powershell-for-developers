$regex = "\s+const\s+\w+\s+(?<name>.*)\s+=\s+(?<value>.*);"

Select-String $regex .\test.cs |
    ForEach {
        $fileName = $_.Path
        ForEach($match in $_.Matches) {
            New-Object PSObject -Property @{
                FileName = $fileName
                Name     = $match.Groups["name"].Value
                Value    = $match.Groups["value"].Value            
            }
        }
    } 