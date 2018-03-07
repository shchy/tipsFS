namespace EVM.Web.Handler


module Project =
    open System
    open Microsoft.AspNetCore.Http
    open Microsoft.Extensions.Logging
    open Giraffe
    open EVM
    open EVM.Web.View

    
    let projectHandler id =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let dataStore = ctx.GetService<IDataStore>()
            let f = 
                dataStore.GetProjects()
                |> List.tryFind (fun x -> x.ID = id)
                |> fun x -> match x with
                            | None -> Common.notFound "NotFound"
                            | Some p -> Project.view p |> htmlView
            f next ctx