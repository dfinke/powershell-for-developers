param(
	$rowFields,
	$columnFields,
	$values,
	$saveAs,
    [Switch]$autoCloseXl
)

Begin   {
	$targetData = @()

	$xlDatabase            = 1
	$xlPivotTableVersion12 = 3
	$xlHidden              = 0
	$xlRowField            = 1
	$xlColumnField         = 2
	$xlPageField           = 3
	$xlDataField           = 4
}

Process { $targetData += $_ }

End {

	if($saveAs) {
        $path = Split-Path $saveAs
        
        if(!(Test-Path $path )) {
            throw "SaveAs directory [$path] does not exist"
        }
	}

	$XL = New-Object -ComObject Excel.Application

	$XL.DisplayAlerts = $False
	$XL.ScreenUpdating = $false
	$XL.Visible = -not $autoCloseXl

	$Workbook = $XL.Workbooks.Add()
	$Sheet1 = $Workbook.Worksheets.Item(1)
	$Sheet2 = $Workbook.Worksheets.Item(2)

	$Columns = $targetData | Select -First 1 | Get-Member -MemberType *property

	for($idx=0; $idx -lt $Columns.Count; $idx++) {
		$Sheet2.Cells.Item(1,$idx+1) = $Columns[$idx].Name
	}

	$row = 2
	$targetData | % {

		for($idx=0; $idx -lt $Columns.Count; $idx++) {
			$Sheet2.Cells.Item($row,$idx+1) = $_.($Columns[$idx].Name)
		}

		$row++
	}

	$rowCount = $targetData.Count + 1
	$columnCount = $Columns.Count

    Write-Debug "Sheet2!R1C1:R$($rowCount)C$($columnCount)"

	$PivotTable = $Workbook.`
                   PivotCaches().`
                   Create($xlDatabase,`
                          "Sheet2!R1C1:R$($rowCount)C$($columnCount)",`
                          $xlPivotTableVersion12
                   )

	$PivotTable.CreatePivotTable("Sheet1!R1C1") | Out-Null

	if($columnFields) {
		$PivotFields = $Sheet1.PivotTables("PivotTable1").PivotFields($columnFields)
		$PivotFields.Orientation=$xlColumnField
	}

	if($rowFields) {
		$PivotFields = $Sheet1.pivottables("PivotTable1").PivotFields($rowFields)
		$PivotFields.Orientation=$xlRowField
	} else {
        $columns | ? {$_.definition -match "system.string"} | % {
            $PivotFields = $Sheet1.PivotTables("PivotTable1").PivotFields($_.Name)
            $PivotFields.Orientation=$xlRowField
        }
    }

	if($values) {
		$PivotFields = $Sheet1.pivottables("PivotTable1").PivotFields($values)
		$PivotFields.Orientation=$xlDataField
	} else {
        $columns | ? {$_.definition -match "system.double|system.int|system.int64|system.float"} | % {
            $PivotFields = $Sheet1.PivotTables("PivotTable1").PivotFields($_.Name)
            $PivotFields.Orientation=$xlDataField
        }
    }

	$XL.ScreenUpdating = $true
    
    if($saveAs) {
       $Workbook.SaveAs($saveAs)
    }
    
    if($autoCloseXl) {
        $XL.Quit()
    }
}