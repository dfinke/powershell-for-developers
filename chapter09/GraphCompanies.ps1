. .\graphviz.ps1

New-Graph G {

    Get-Process |
      Where {$_.Company -match 'Inc\.|Corp\.'} |
      ForEach {
          Add-Edge "$($_.Company)" "$($_.Name)" 
      }

} | dot -Tpng -o .\test.png

.\test.png