namespace EVM.Web.Handler


module Home =
    open System
    open Microsoft.AspNetCore.Http
    open Microsoft.Extensions.Logging
    open Giraffe
    open EVM
    open EVM.Web.View

    let homeHandler (user:User) (next : HttpFunc) (ctx : HttpContext) =    
        let dataStore = ctx.GetService<IDataStore>()
        let projects = dataStore.GetProjects()
        (Home.view projects |> htmlView) next ctx