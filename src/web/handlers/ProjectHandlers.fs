module EVM.Web.Handler.Project

open System
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Giraffe
open EVM
open EVM.Web
open EVM.Web.View


let viewHandler id (user:User)=
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let dataStore = ctx.GetService<IDataStore>()
        let f = 
            dataStore.GetProjects()
            |> List.tryFind (fun x -> x.ID = id)
            |> fun x -> match x with
                        | None -> Common.notFound "NotFound"
                        | Some p -> Project.view p |> htmlView
        f next ctx

let createHandler (message:string option) (user:User) =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        (message |> Project.createView |> htmlView) next ctx

let tryCreateHandler (req:CreateProjectRequest) (user:User) =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let dataStore = ctx.GetService<IDataStore>()
        let createOne = dataStore.CreateProject {
            ID = -1
            Name = req.Name
            Sprints = List.Empty
            Tasks = List.Empty
            Users = [user]
        }
        redirectTo false "/home" next ctx 
