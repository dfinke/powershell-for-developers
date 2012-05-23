ipmo showui

New-Window -WindowStartupLocation CenterScreen -Width 1000 -Height 700 -Show -On_Loaded{
    $Search.Focus()
} {
    Grid -Rows 35, 100*, 100* -Columns 200*, Auto, 300* {
        TextBox -Name Search -Row 0 -Column 0 -Margin 5 -On_PreviewKeyUp {
            if($_.key -eq "Enter") {                
                $OwnerRepo.DataContext = $null
                $SearchResults.DataContext = (Invoke-RestMethod "http://github.com/api/v2/json/repos/search/$($this.Text)").repositories
            }
        }

        GridSplitter -RowSpan 3 -Row 0 -Column 1 -Width 4 -Background Gray -VerticalAlignment Stretch -HorizontalAlignment Center

        ListView -Name SearchResults -Margin 5 -Row 1 -Column 0 -RowSpan 2 -DataBinding @{ItemsSource="."} -On_SelectionChanged {
            $owner = ($this.Selecteditem).owner            
            $url   = ($this.Selecteditem).url
            #$Browser.Navigate($url)
            $OwnerGroupBox.Header = " Repos for Owner: $owner "
            #$OwnerRepo.DataContext = (Invoke-RestMethod "http://github.com/api/v2/json/repos/show/$($owner)").repositories
            $OwnerRepo.DataContext = Invoke-RestMethod "https://api.github.com/users/$($owner)/repos"
            
        } -View {
            GridView -Columns {
                GridViewColumn "name"
                GridViewColumn "owner"
                GridViewColumn "watchers"
                GridViewColumn "url"
            }
        }
        
        GroupBox -Name OwnerGroupBox -Header " Repos for Owner " -Margin 5 -Row 0 -Column 2 -RowSpan 2 -Content {
            ListView -Margin 5 -Name OwnerRepo -DataBinding @{ItemsSource=New-Binding} -View {
                GridView -Columns {
                    GridViewColumn "name"
                    GridViewColumn "watchers"
                    GridViewColumn "url"
                }
            }
        }

#        WebBrowser -Name Browser -Row 2 -Column 2 -Margin 5 -On_Loaded {
#            $this.Navigate("about:blank")
#        }
    }
}