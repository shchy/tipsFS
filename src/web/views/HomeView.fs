namespace EVM.Web.View

open EVM
open System
open Giraffe
open Giraffe.GiraffeViewEngine

module Home =
    let view (prjects:Project list)  =
        let toLink = fun (p:Project) -> (a [_href (sprintf "/project/%d" p.ID) ] [ rawText p.Name])
        let projectLinks = List.map toLink prjects
        [
            div [_class "container" ] projectLinks
        ] |> Layout.view "Home"