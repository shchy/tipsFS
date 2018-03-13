namespace EVM.Web.View

open EVM
open System
open Giraffe
open Giraffe.GiraffeViewEngine

module Project =
    let view (project:Project)  =
        [
            div [_class "container" ] [
                div [_class "row"] [
                    h1 [] [rawText project.Name]
                ]

                div [_class "row"] [
                    div [_class "col-sm-4"] [
                        div [_class "card"] [
                            div [_class "card-header"] [
                                rawText "sprint list"
                            ]
                            project.Sprints
                                |> Layout.toListViewUl (fun x -> a [_href (sprintf "/project/sprint/%i" x.ID)] [ rawText x.Name ])
                                |> Layout.appendAttribute (_class "list-group-flush")
                        ]
                    ]
                    div [_class "col-sm-4"] [
                        div [_class "card"] [
                            div [_class "card-header"] [
                                rawText "task list"
                            ]
                            project.Tasks
                                |> Layout.toListViewUl (fun x -> a [_href (sprintf "/project/task/%i" x.ID)] [ rawText x.Name ])
                                |> Layout.appendAttribute (_class "list-group-flush")
                        ]
                        
                    ]
                    div [_class "col-sm-4"] [
                        div [_class "card"] [
                            div [_class "card-header"] [
                                rawText "user list"
                            ]
                            project.Users
                                |> Layout.toListViewUl (fun x -> a [_href (sprintf "/user/%i" x.ID)] [ rawText x.Name ])
                                |> Layout.appendAttribute (_class "list-group-flush")
                        ]
                    ]
                ]
            ]
        ]
        |> Layout.view project.Name
    let createView (message:string option) =
        [
            div [_class "container"] [
                yield h1 [_class "h3 mb-3 font-weight-normal"] [
                        rawText "create project"
                    ]
                yield form [ _class "form-edit"; _action "/project/create"; _method "POST"] [
                        
                    label [_for "name"; _class "sr-only"] [ rawText "name" ]
                    input [_id "name";_name "name"; _class "form-control"; _placeholder "Name" ] 
                    
                    button [_type "submit"; _class "btn btn-lg btn-primary btn-block"] [ rawText "Submit" ]
                ]
            
                if message.IsSome then 
                    yield div [_class "alert alert-warning"; attr "role" "alert" ] [ rawText message.Value ]
            ]
        ] |> Layout.view "Create Project" 