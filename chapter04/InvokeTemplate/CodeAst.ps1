. .\Invoke-Template.ps1

Invoke-Template $pwd\etc {

    $methodOverrides = Invoke-Template $pwd\etc {
        [System.Management.Automation.Language.AstVisitor].GetMethods() |
            Where { $_.Name -like 'Visit*' } |
                ForEach {
                    $functionName = $_.Name
                    $parameterName = $_.GetParameters()[0].ParameterType.Name

                    Get-Template AstVisitAction.txt
                }
    }

    Get-Template CommandMatcher.txt
}