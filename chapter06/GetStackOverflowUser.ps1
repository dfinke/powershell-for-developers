function Get-StackOverflowUser ($id) {            

    $url = "http://api.stackoverflow.com/1.1/users/$id"
    $data = (New-Object Net.WebClient).DownloadData($url)
    $memoryStream = New-Object System.IO.MemoryStream(,$data)
    $decompress = New-Object `
        System.IO.Compression.GZipStream($memoryStream,"Decompress")
    $reader = New-Object System.IO.StreamReader($decompress)
    $ret = $reader.ReadToEnd()            

    ($ret | ConvertFrom-Json).Users
}            

Get-StackOverflowUser –id 110865
