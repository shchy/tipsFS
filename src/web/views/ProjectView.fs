namespace EVM.Web.View

open EVM
open System
open Giraffe
open Giraffe.GiraffeViewEngine

module Project =
    let view (project:Project)  =
        // let toLink = fun (p:Project) -> (a [_href (sprintf "/project/%d" p.ID) ] [ rawText p.Name])
        // let projectLinks = List.map toLink prjects
        
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