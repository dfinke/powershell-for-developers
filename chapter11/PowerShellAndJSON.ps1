$json = @"
{
    "firstName": "John",
    "lastName" : "Smith",
    "age"      : 25,
    "address"  :
    {
        "streetAddress": "21 2nd Street",
        "city"         : "New York",
        "state"        : "NY",
        "postalCode"   : "10021"
     },
     "phoneNumber":
     [
         {
            "type"  : "home",
            "number": "212 555-1234"
         },
         {
            "type"  : "fax",
            "number": "646 555-4567"
         }
     ]
 }
"@

cls

$PowerShellRepresentation = $json | ConvertFrom-Json 

$PowerShellRepresentation | Out-String
$PowerShellRepresentation.address | ft -a | Out-String
$PowerShellRepresentation.phoneNumber | ft -a | Out-String


$PowerShellRepresentation | ConvertTo-Json | Out-String