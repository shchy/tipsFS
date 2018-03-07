namespace EVM.Web.View

open EVM
open System
open Giraffe
open Giraffe.GiraffeViewEngine

module Project =
    let view (prject:Project)  =
        // let toLink = fun (p:Project) -> (a [_href (sprintf "/project/%d" p.ID) ] [ rawText p.Name])
        // let projectLinks = List.map toLink prjects
        
        [
            div [_class "container" ] [
                div [_class "row"] [
                    h1 [] [rawText prject.Name]
                ]
                // div [_class "row"] [
                //     li [] projectLinks
                // ]
            ]
        ]
        |> Layout.view "Home"