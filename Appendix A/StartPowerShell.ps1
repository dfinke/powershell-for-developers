function Start-PowerShell {
    param (
        [Switch]$NoProfile
    )

    if($NoProfile) {
        Start PowerShell -Args -NoProfile
    } else {
        Start PowerShell 
    }
}