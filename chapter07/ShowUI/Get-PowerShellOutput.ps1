function Get-PowerShellOutput
{
    <#
    .Synopsis
        Gets the output from a PowerShell data source
    .Description
        Gets the output from a PowerShell data source
        
    #>
    [CmdletBinding(DefaultParameterSetName='TimeStampedOutput')]
    param(
    [String]
    $Name,
    
    $Visual = $this,
        
    [Parameter(Mandatory=$true,ParameterSetName='OutputOnly')]
    [Switch]
    $OutputOnly,    
    
    [Parameter(Mandatory=$true,ParameterSetName='ErrorOnly')]
    [Switch]
    $ErrorOnly,    
    
    [Parameter(Mandatory=$true,ParameterSetName='ProgressOnly')]
    [Switch]
    $ProgressOnly,    
    
    [Parameter(Mandatory=$true,ParameterSetName='VerboseOnly')]
    [Switch]
    $VerboseOnly,    

    [Parameter(Mandatory=$true,ParameterSetName='DebugOnly')]
    [Switch]
    $DebugOnly,    

    [Parameter(Mandatory=$true,ParameterSetName='WarningOnly')]
    [Switch]
    $WarningOnly,    
    
    [Parameter(Mandatory=$true,ParameterSetName='GetDataSource')]
    [switch]
    $GetDataSource,

    [switch]$Last
    )
    
    process {
        $item = $Visual
        
        while ($item) {
            
            if ($item.DataContext -is [ShowUI.PowerShellDataSource]) {
                break 
            } 
            
            $item = [Windows.Media.VisualTreeHelper]::GetParent($item)                
        }
        
        if ($psCmdlet.ParameterSetName -ne 'GetDataSource') {
            if ($item) {
                $streamName = $psCmdlet.ParameterSetName.Replace("Only", "")
                if ($last) { $streamName = "Last$StreamName" }             
                $item.DataContext.$streamName
            }
        } else {
            $item.DataContext
        } 
    }
}
