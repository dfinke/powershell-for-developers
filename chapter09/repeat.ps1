function repeat {
    param (
        [int]$HowManyTimes,
        [scriptblock]$block
    )

    1..$HowManyTimes | Foreach { & $block }
}

repeat 3 {"Hello World"}