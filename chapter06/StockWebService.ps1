function Get-Quote {
    param(
        [Parameter(ValueFromPipeline=$true)]
        [string[]]$symbol,
        [Switch]$Raw
    )

    Begin {
        $url = "http://www.webservicex.net/stockquote.asmx?wsdl"
        $proxy = New-WebServiceProxy $url
    }

    Process {
        $result = $proxy.GetQuote($symbol)

        if($Raw) { return $result }

        [xml]$result
    }
}

#DRRX SPRD IART RFMD VICL SNTA PSMT DRRX CNGL BNVI IART SHLD APPY FFKY TCI BAC SCHW NILE

"IBM", "AAPL", "GM", "GE", "MSFT", "GOOG" |
    Get-Quote |
    ForEach {$_.StockQuotes.Stock} |
    Format-Table