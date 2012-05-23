function New-Graph  {
    param(
        [string]$Name,
        [scriptblock]$ScriptBlock
    )

    $Script:edges = @()

    function Add-Edge {
        param(
            [string]$Source,
            [string]$Target
        )

        $Script:edges += "`"$Source`"->`"$Target`""
    }

    & $ScriptBlock

@"
digraph $name {
    $Script:edges
}
"@
}