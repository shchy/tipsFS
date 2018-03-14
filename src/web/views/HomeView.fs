namespace EVM.Web.View

open EVM
open System
open Giraffe
open Giraffe.GiraffeViewEngine

module Home =
    let view (user:User) (projects:Project list)  =
        [
            div [_class "container" ] [
                div [_class "row"] [
                    h1 [] [rawText user.Name]
                ]

                div [_class "row"] [
                    div [_class "col-sm-4"] [
                        [
                            ("/project/create","create project")
                            ("/task/create", "create task")
                            ("/sprint/create", "create sprint")
                        ]
                            |> Layout.toListViewUl (fun x -> (a [_href (fst x) ] [ rawText (snd x)]) )
                            |> Layout.appendAttribute (_class "list-group-flush")
                            |> Layout.toCard "menu" 
                    ]
                    div [_class "col-sm-4"] [
                        projects
                            |> Layout.toListViewUl (fun p -> (a [_href (sprintf "/project/%d" p.ID) ] [ rawText p.Name]) )
                            |> Layout.appendAttribute (_class "list-group-flush")
                            |> Layout.toCard "project list" 
                    ]
                ]
            ]
        ]
        |> Layout.view "Home"