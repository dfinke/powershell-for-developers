function rdp {
    param (
        [Switch]$ClientSQLBox,
        [Switch]$ClientDev,
        [Switch]$ClientIntegrationBox,
        [Switch]$ClientQABox
    )

    if($ClientSQLBox)         { $server = 'ClientSQLBox' }
    if($ClientDev)            { $server = 'ClientDev' }
    if($ClientIntegrationBox) { $server = 'ClientIntegrationBox' }
    if($ClientQABox)          { $server = 'ClientQABox' }
 
    if($server) { mstsc /v:$server }
}