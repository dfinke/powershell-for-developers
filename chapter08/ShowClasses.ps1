param (
    $targetDirectory="..\chapter05\BeaverMusic"
)

Import-Module ShowUI

New-Window -Title "PowerShell/Rosyln Class Viewer" -WindowStartupLocation CenterScreen -Height 500 -Width 800 -Show {
    Grid -Columns 50*, 100*{
     
        ListBox -Column 0 -Margin 5 `
            -DataContext ( . .\GetCSharpClass.ps1 $targetDirectory | Sort Name) `
            -DataBinding @{ItemsSource = "."} `
            -DisplayMemberPath 'Name' `
            -On_SelectionChanged {
                $TB.Text = $this.SelectedItem.Class.Parent.ToString().Trim()
            }

        TextBox -Column 1 -Margin 5 -Name TB `
            -IsReadOnly -VerticalScrollBarVisibility Auto `
            -HorizontalScrollBarVisibility Auto
    }
}