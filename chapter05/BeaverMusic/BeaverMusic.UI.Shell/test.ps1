function New-Album  {
    <#
        .Synopsis
            Creates a new album.
        .Description
            The New-Album function creates a new album and sets both the album name and artist.
        .Example
            New-Album "Sad Wings Of Destiny" "Judas Priest"
    #>

	param(
		[Parameter(ValueFromPipelineByPropertyName=$true)]
		[string]$Name, 
		[Parameter(ValueFromPipelineByPropertyName=$true)]
		[string]$Artist
	)

	Process {
		$album = New-Object BeaverMusic.Album

		$album.Name   = $Name
		$album.Artist = $Artist

		$album
	}
}

function Clear-Album { 
    <#
        .Synopsis
            Deletes the albums in the Beaver Music list.
        .Description
            The Clear-Album deletes the contents of in the Beaver Music main window.
        .Example
            Clear-Album
    #>

	$AlbumRepository.Clear() 
}

function Get-Album { 
    <#
        .Synopsis
            Gets a list of the albums currently in the repository session.
        .Description
            The Get-Album can get the list of albums or can filter on a 
        .Example
            Get-Album
        .Example
            New-Album "Sad Wings Of Destiny" "Judas Priest" | Add-Album
			Get-Album Wings
    #>
	
	param(
		# 
		$name
	)
    $AlbumRepository.GetAlbums() | 
	  Where {$_.Name -match $Name} 
}

function Add-Album { 
    <#
        .Synopsis
            Appends ablums to the album repository.
        .Description
            The Add-Album function adds ablums to the end of the album repository. This results to it showing up in main window.
        .Example
            New-Album "Sad Wings Of Destiny" "Judas Priest" | Add-Album
        .Example
            Add-Album (New-Album "Stained Glass" "Judas Priest")
    #>
	param(
		[Parameter(ValueFromPipeline=$true)]
		$album
	)
	
	Process {
		$AlbumRepository.SaveAlbum($album) | Out-Null 
	}
}

function Remove-Album {
    <#
        .Synopsis
            Appends ablums to the album repository.
        .Description
            The Remove-Album function adds ablums to the end of the album repository. This results to it showing up in main window.
        .Example
		    New-Album "Sad Wings Of Destiny" "Judas Priest" | Add-Album
            Get-Album Wings | Remove-Album  
        .Example
			Import-Default
            Get-Artist Bee | Remove-Album  
    #>

	param(
		[Parameter(ValueFromPipelineByPropertyName=$true)]
		[int]$Id
	)
	
	Begin   { $Ids = @() }
	Process { $Ids += $id }
	End     { $AlbumRepository.RemoveAlbum($Ids) }
}

function Get-Artist ($artist) {
	$AlbumRepository.GetAlbums() | 
  	  Where {$_.artist -match $artist}
}

function Import-Default { 
	Import-Csv .\albums.csv | 
	  Sort Artist | 
	  New-Album | 
	  Add-Album 
}

function Import-VanHalen  { Get-ChinookData "Van Halen" | Import-BeaverMusic }
function Import-U2        { Get-ChinookData U2 | Import-BeaverMusic }
function Import-Metallica { Get-ChinookData Metallica | Import-BeaverMusic }


function Get-ChinookData ($artist) {
	if(!$global:ChinookData) {
		[xml]$ChinookData = [io.file]::ReadAllLines("$pwd\ChinookData.xml")
	}

	if(!$global:artists) {
		
		$global:artists = @{}
		$ChinookData.ChinookDataSet.Artist | 
			Foreach {
				$artists.($_.ArtistId)= $_.Name
			} 
	}

	$(ForEach($item in $ChinookData.ChinookDataSet.Album) {
		New-Album $item.Title $artists.($item.ArtistId)
	}) | Where {$_.Artist -match $artist}
}

function Import-BeaverMusic {
	param(
		[Parameter(ValueFromPipeline=$true)]
		$album
	)
	
	Begin   { Clear-Album }
	Process { Add-Album $album | Out-Null }
}

function Get-PrivateBytes {
	$counterName = "\Process($(Get-CurrentProcessName))\Private Bytes"
	(get-counter $counterName).CounterSamples | 
		Select -Expand CookedValue
}

function Get-CurrentProcessName { ([System.Diagnostics.Process]::GetCurrentProcess()).ProcessName }

function Measure-PrivateBytes ($Message, [scriptblock]$sb) {	
	
	if($sb) { & $sb }

	New-Object PSobject -Property @{
		Message      = $Message
		PrivateBytes = Get-PrivateBytes
		TimeStamp    = Get-Date
	}
}

function UnitTest {
	clear-album
	import-default

	$r = (get-album | group artist -NoElement | sort count -desc)[0]
	$r
	""
	"# of Jackson titles == 3? {0}" -f ($r.Count -eq 3)
	"first artist is Michael Jackon? {0}" -f ($r.Name  -eq 'Michael Jackson')
	"total albums == 26? {0}" -f ((get-album | measure).count -eq 26)
}

function Export-Excel {
	param(
		$fileName = "$pwd\BeaverMusic.csv"
	)

	Get-Album | 
	  Export-Csv -NoTypeInformation $fileName 

	Invoke-Item $fileName 
}

function Show-NewAlbumDialog {
	$contractName="BeaverMusic.UI.Shell.AlbumListViewModel"
	$MEFHelper.GetExport($contractName).NewAlbumCommand.Execute($null)
}

function Get-AlbumFromWeb {
	$wc=New-Object Net.WebClient
	$url="http://dougfinke.com/PowerShellForDevelopers/albums.csv"
	$wc.DownloadString($url) | 
	  ConvertFrom-Csv
}

function Get-YahooMusic {
	$wc = New-Object Net.WebClient
	$url = "http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20music.release.popular"
	[xml]$xml = $wc.DownloadString($url)

	$xml.query.results.Release |
		ForEach {
			New-Object PSObject -Property @{
				Artist=$_.artist.name
				Name=$_.Title
			}
		}
}

return

$r = $(
    Measure-PrivateBytes "Before Import"
    Measure-PrivateBytes "After Import" {Import-Default}
    Measure-PrivateBytes "Clear Albums" {Clear-Album}
    Measure-PrivateBytes "After Clear"
)

$r | Format-Table -Autosize



#$mefhelper.getmefcatalog.parts|select displayname