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
                    projects
                    |> Layout.toListViewUl (fun p -> (a [_href (sprintf "/project/%d" p.ID) ] [ rawText p.Name]) )
                ]
            ]
        ]
        |> Layout.view "Home"