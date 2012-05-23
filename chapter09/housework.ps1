$houseworkXml = @"
<todo name="housework">
    <todoItem priority="high">Clean the house.</todoItem>
    <todoItem priority="medium">Wash the dishes.</todoItem>
    <todoItem priority="medium">Buy more soap.</todoItem>
</todo>
"@

$housework = [xml]$houseworkXml 
$housework.todo.todoItem