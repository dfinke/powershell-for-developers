$code = @"
    public class MyMath
    {
        public int  MyAdd(int n1, int n2)      { return n1 + n2; }
        public int  MySubtract(int n1, int n2) { return n1 - n2; }
        public int  MyMultiply(int n1, int n2) { return n1 * n2; }
        public int  MyDivide(int n1, int n2)   { return n1 / n2; }
        public void MyTest() {System.Console.WriteLine("Test");}
    }
"@

Add-Type -TypeDefinition $code

function Get-Parameter ($target) {
    ($target.GetParameters() |
        ForEach {
            "`$$($_.Name)"
        }
    ) -join ", "
}

@"
`$MyMath = New-Object MyMath
$([MyMath].GetMethods() | Where {$_.Name -like "My*"} | ForEach {

    $params = Get-Parameter $_

@"

function Invoke-$($_.Name) ($params) {`$MyMath.$($_.Name)($($params))}
"@
})

"@