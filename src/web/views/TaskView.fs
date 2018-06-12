module EVM.Web.View.Task

open EVM
open System
open Giraffe
open Giraffe.GiraffeViewEngine


let view (task:TaskItem)  =
    [
        div [_class "container" ] [
            div [_class "row"] [
                h1 [] [rawText task.Name]
            ]

            
        ]
    ]
    |> Layout.view task.Name
let createView (message:string option) =
    [
        div [_class "container"] [
            yield h1 [_class "h3 mb-3 font-weight-normal"] [
                    rawText "create task"
                ]
            yield form [ _class "form-edit"; _action "/task/create"; _method "POST"] [
                    
                label [_for "name"; _class "sr-only"] [ rawText "name" ]
                input [_id "name";_name "name"; _class "form-control"; _placeholder "Name" ] 
                
                button [_type "submit"; _class "btn btn-lg btn-primary btn-block"] [ rawText "Submit" ]
            ]
        
            if message.IsSome then 
                yield div [_class "alert alert-warning"; attr "role" "alert" ] [ rawText message.Value ]
        ]
    ] |> Layout.view "Create Task" 