Add-Type -TypeDefinition @"
namespace Example {
    public class Test {
        public string FirstName {get; set;}
        public string LastName {get; set;}
    }
}
"@

[Example.Test] @{
    FirstName = "Donald"
    LastName  = "Knuth"
}