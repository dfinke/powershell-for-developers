[PSCustomObject] @{
    FirstName = "Donald"
    LastName  = "Knuth"
    Date      = Get-Date    
} | Add-Member -PassThru -MemberType ScriptProperty `
      FullName {"{0}, {1}" -f $this.LastName, $this.FirstName} | ft -a