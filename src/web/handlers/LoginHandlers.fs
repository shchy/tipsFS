namespace EVM.Web.Handler


module Login =
    open System
    open System.Security.Claims
    open System.Threading
    open Microsoft.AspNetCore.Http
    open Microsoft.AspNetCore.Authentication
    open FSharp.Control.Tasks.ContextInsensitive
    open Giraffe
    open EVM
    open EVM.Web
    open EVM.Web.View
    
    

    // ログインハンドラ
    let loginHandler (requestUser:LoginRequest) (next : HttpFunc) (ctx : HttpContext) =
        let dataStore = ctx.GetService<IDataStore>()
        
        dataStore.GetUsers()
        |> List.where (fun u -> u.AuthID = requestUser.AuthID && u.Password = requestUser.Password)
        |> List.tryHead
        |> fun x -> match x with
                    | None -> Common.toLogin Msg.loginError next ctx
                    | Some user -> 
                    task {
                        // JohnとしてSignInAsyncしてaspnetに認証情報を保存
                        let issuer = "http://localhost:5000"
                        let claims =
                            [
                                Claim(ClaimTypes.Name,      user.AuthID,  ClaimValueTypes.String, issuer)
                                Claim(ClaimTypes.Role,      "Admin", ClaimValueTypes.String, issuer)
                            ]
                        let identity = ClaimsIdentity(claims, Common.authScheme)
                        let claim    = ClaimsPrincipal(identity)

                        do! ctx.SignInAsync(Common.authScheme, claim)

                        let toPath = 
                            ctx.Request.Headers.["Referer"].ToArray()
                            |> Array.tryHead
                            |> fun x -> match x with
                                        | None -> "/home"
                                        | Some path -> path
                            

                        return! redirectTo false toPath next ctx
                    }

    let loginUserWith (authedHandler:User -> HttpHandler) (next : HttpFunc) (ctx : HttpContext)=
        let userID = ctx.User.Identity.Name
        let dataStore = ctx.GetService<IDataStore>()
        let f = 
            dataStore.GetUsers()
            |> List.tryFind (fun u -> u.AuthID = userID)
            |> fun x -> match x with
                        | None -> Common.accessDenied
                        | Some user -> (user |> authedHandler)
        f next ctx      