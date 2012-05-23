. .\graphviz.ps1

New-Graph G {Add-Edge Hello World} | dot -Tpng -o .\hello.png

.\hello.png