Import-Module .\ShowUI

function Search-Twitter ($q) {    
    $wc = New-Object Net.Webclient
    $url = "http://search.twitter.com/search.rss?q=$q"
    ([xml]$wc.downloadstring($url)).rss.channel.item | select * 
}

$ws = @{
    WindowStartupLocation = "CenterScreen"
    Width  = 500
    Height = 500
}

New-Window @ws -Show {
    ListBox -Background Black -ItemTemplate {
        Grid -Columns 55, 300 {
            Image -Column 0 -Name Image -Margin 5
            TextBlock -Column 1 -Name Title `
                -Margin 5 `
                -Foreground White `
                -TextWrapping Wrap
        } | ConvertTo-DataTemplate -binding @{
            "Image.Source" = "image_link"
            "Title.Text" = "title"
        }
    }  -ItemsSource (Search-Twitter PowerShell)
}