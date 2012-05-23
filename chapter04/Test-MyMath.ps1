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

[MyMath].GetMethods() | Where {$_.Name -like "My*"} | Out-GridView
