function New-Author {
    param (
        [string]$Author,
        [string]$Title,
        [string]$Publisher,
        [int]$Year
    )

    New-Object PSObject -Property @{
        Author    = $Author
        Title     = $Title
        Publisher = $Publisher
        Year      = $Year
    }
}

$authors = $(
    New-Author "Donald E. Knuth" "Literate Programming" "CSLI" 1992 
    New-Author "Jon Bentley" "More Programming Pearls" "Addison-Wesley" 1990
    New-Author "Alfred V. Aho, et. al." "Compilers: Principles, Techniques, and Tools" "Addison-Wesley" 1986
    New-Author "Martin Fowler" "Patterns of Enterprise Application Architecture" "Addison-Wesley" 2002
)

$authors | Export-Csv -NoTypeInformation .\authors.csv
.\authors.csv