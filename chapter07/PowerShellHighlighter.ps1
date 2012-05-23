param(
    $filename="$pwd\PowerShellHighlighter.ps1"
)

[System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms") | out-null

$replacementColours = @{
    "Command"=[system.drawing.color]::Yellow
    "CommandParameter"=[system.drawing.color]::Yellow
    "Variable"=[system.drawing.color]::LightGreen
    "Operator"=[system.drawing.color]::DarkCyan
    "Grouper"=[system.drawing.color]::DarkCyan
    "GroupStart"=[system.drawing.color]::DarkCyan
    "GroupEnd"=[system.drawing.color]::DarkCyan
    "StatementSeparator"=[system.drawing.color]::DarkCyan
    "String"=[system.drawing.color]::Cyan
    "Number"=[system.drawing.color]::Cyan
    "CommandArgument"=[system.drawing.color]::Cyan
    "Keyword"=[system.drawing.color]::Magenta
    "Attribute"=[system.drawing.color]::DarkGoldenrod
    "Property"=[system.drawing.color]::DarkGoldenrod
    "Member"=[system.drawing.color]::DarkGoldenrod
    "Type"=[system.drawing.color]::DarkGoldenrod
    "Comment"=[system.drawing.color]::Red
}

$form = New-Object Windows.Forms.Form
$form.Size = New-Object Drawing.Size @(600,600)
$form.StartPosition = [System.Windows.Forms.FormStartPosition]::CenterScreen
$form.Text = $fileName

$rtb = New-Object System.Windows.Forms.RichTextBox
$rtb.Dock = "Fill"

$form.Controls.Add($rtb)

$file = (Resolve-Path $filename).Path
$content = [IO.File]::ReadAllText($file)

$tokens = [System.Management.Automation.PsParser]::Tokenize($content, [ref] $null)

$rtb.Text = $content
$nllcCount = 0

$rtb.BackColor = [system.drawing.color]::Black
foreach ($token in $tokens)
{
   $rtb.SelectionStart  = $token.Start - $nllcCount
   $rtb.SelectionLength = $token.Length

   if ($token.Type.ToString() -eq "LineContinuation" -Or
       $token.Type.ToString() -eq "NewLine")
   {
      $nllcCount++
   }
   else
   {
      $rtb.SelectionColor = $replacementColours[$token.Type.ToString()]
   }
}

$rtb.SelectionStart = 0
$rtb.SelectionLength = 0

$form.Add_Shown( { $form.Activate() } )
$drc = $form.ShowDialog()