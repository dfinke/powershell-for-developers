. .\graphviz.ps1

New-Graph G {
    ForEach($x in 1..15) {
        Add-Edge $x ($x*$x)
    }
} | dot -Tpng -o .\test.png

.\test.png
