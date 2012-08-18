function SlowTrain($text) {

    $h = @{}
    [regex]::split($text.ToLower(), '\W+') |
        ForEach {
            $h[$_] =''
        }
    $h
}

function FastTrain($text) {

    $h = @{}
    ForEach ($word in [regex]::split($text.ToLower(), '\W+') )
    {
        $h[$word] = ''
    }
    $h
}

function Do-Timing {
    param(
        $Caption,
        $ScriptBlock
    )

    "{0} Seconds: {1}" -f  $Caption, (Measure-Command $ScriptBlock).TotalSeconds
}

$Text = [IO.File]::ReadAllText( "$pwd\holmes.txt" )

Write-Host -ForegroundColor Green "Timing the SlowTrain"
Do-Timing "SlowTrain in" {SlowTrain $Text}
Write-Host -ForegroundColor Green "Timing the FastTrain"
Do-Timing "FastTrain in" {FastTrain $Text}