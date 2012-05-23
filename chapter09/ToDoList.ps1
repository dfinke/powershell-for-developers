function New-ToDoList {
    param(
        [string]$ToDoListName,
        [scriptblock]$ScriptBlock
    )
    
    function New-ToDoItem {
        param (
            [string]$Priority,
            [string]$Task
        )
 
        New-Object PSObject -Property @{
            ToDoListName = $ToDoListName
            Priority = $Priority
            Task = $Task
        }
    }    

    & $ScriptBlock
}

New-ToDoList housework {
    New-ToDoItem high "Clean the house."
    New-ToDoItem medium "Wash the dishes."
    New-ToDoItem medium "Buy more soap."

    "Buy Beer", "Get Pizza", "Purchase Microsoft Stock" | 
        ForEach {
            New-ToDoItem high $_
        }
} | ft -a