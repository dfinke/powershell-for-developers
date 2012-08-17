#requires -version 3
param($query = "Obama")

$apiKey = "e91adfa87e8f8026712c4d92b54a0b14:0:39364737"

function Get-SemanticNYT {

    param($query)

    $uri = "http://api.nytimes.com/svc/semantic/v2/"+
        "concept/search.json?query=$query&api-key=$apiKey"

    (Invoke-RestMethod $uri).results
}

function Get-SemanticNYTArticles {

    param(
        [Parameter(ValueFromPipelineByPropertyName=$true)]
        $concept_name,
        [Parameter(ValueFromPipelineByPropertyName=$true)]
        $concept_type
    )

    process {
      $uri = "http://api.nytimes.com/svc/semantic/v2/" +
      "concept/name/$concept_type/$concept_name.json?&" +
      "fields=all&api-key=$apiKey"

      (Invoke-RestMethod $uri).results
    }
}

Get-SemanticNYT $query |
    Get-SemanticNYTArticles |
    Where links |
        Select -ExpandProperty article_list |
        Select -ExpandProperty results |
        Select date, title, url| Out-GridView