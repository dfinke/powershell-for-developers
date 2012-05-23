filter times ([Scriptblock]$Block) {
    1..$_ | ForEach { & $Block }    
}


3 | times {"Hello World"}