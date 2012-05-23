# Adapted from:
#     http://blogs.msdn.com/b/powershell/archive/2009/03/20/get-progid.aspx

<#
    .Synopsis            
        Gets all of the ProgIDs registered on a system            
    .Description            
        Gets all ProgIDs registered on the system.  The ProgIDs returned can be used with New-Object -comObject            
    .Example            
        Get-ProgID            
    .Example            
        Get-ProgID | Where-Object { $_.ProgID -like "*Image*" }             
#>
    
param([string]$ProgId)

$paths = @("REGISTRY::HKEY_CLASSES_ROOT\CLSID")            

if ($env:Processor_Architecture -eq "amd64") {            
    $paths+="REGISTRY::HKEY_CLASSES_ROOT\Wow6432Node\CLSID"            
}             

Get-ChildItem $paths -include VersionIndependentPROGID -recurse |
    ForEach {
        New-Object PSObject -Property @{
            ProgId  = $_.GetValue("")
            '32Bit' = & {
                if ($env:Processor_Architecture -eq "amd64") {
                    $_.PSPath.Contains("Wow6432Node")
                } else {
                    $true
                }
            }
        }
    } | Where {$_.ProgId -match $ProgId}

#(New-Object -ComObject ( Get-ProgID internetexp | select -First 1 ).ProgId).Visible=$true