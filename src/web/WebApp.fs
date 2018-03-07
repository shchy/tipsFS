namespace EVM.Web

open System
open System.Security.Claims
open System.Threading
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks.ContextInsensitive
open Giraffe
open EVM

module WebApp =    
    
    // 認証に使用するCookieのスキーマ
    let authScheme = CookieAuthenticationDefaults.AuthenticationScheme
    
    // アクセス権がないときのハンドラ
    let accessDenied = setStatusCode 401 >=> text "Access Denied"

    // Aspnetの認証が済んでないとaccessDeniedへルーティングするHttpHandlerのラッパー
    let mustBeUser = requiresAuthentication accessDenied

    // // Aspnetの認証が済んでないorロールがAdmin出ない場合にaccessDeniedへルーティングするHttpHandlerのラッパー
    // let mustBeAdmin =
    //     requiresAuthentication accessDenied
    //     >=> requiresRole "Admin" accessDenied

    // // 認証済のClaimsPrincipalからユーザ名を見てアクセス権を判定しエラーへ流すラッパ
    // let mustBeJohn =
    //     requiresAuthentication accessDenied
    //     >=> requiresAuthPolicy (fun u -> u.HasClaim (ClaimTypes.Name, "John")) accessDenied

    let toLogin = redirectTo false "/login"
    let notFound message = RequestErrors.notFound (text message) 

    // ログインハンドラ
    let loginHandler (requestUser:LoginRequest) (next : HttpFunc) (ctx : HttpContext) =
        task {
            let dataStore = ctx.GetService<IDataStore>()

            let findUser = 
                dataStore.GetUsers()
                |> List.where (fun u -> u.AuthID = requestUser.AuthID && u.Password = requestUser.Password)
                |> List.tryHead

            match findUser with
            | None -> return! toLogin next ctx
            | Some user ->
                // JohnとしてSignInAsyncしてaspnetに認証情報を保存
                let issuer = "http://localhost:5000"
                let claims =
                    [
                        Claim(ClaimTypes.Name,      user.AuthID,  ClaimValueTypes.String, issuer)
                        Claim(ClaimTypes.Role,      "Admin", ClaimValueTypes.String, issuer)
                    ]
                let identity = ClaimsIdentity(claims, authScheme)
                let claim     = ClaimsPrincipal(identity)

                do! ctx.SignInAsync(authScheme, claim)

                return! redirectTo true "/home" next ctx
        }

    let loginUserWith (authedHandler:User -> HttpHandler) (next : HttpFunc) (ctx : HttpContext)=
        let userID = ctx.User.Identity.Name
        let dataStore = ctx.GetService<IDataStore>()
        let f = 
            dataStore.GetUsers()
            |> List.tryFind (fun u -> u.AuthID = userID)
            |> fun x -> match x with
                        | None -> toLogin
                        | Some user -> (user |> authedHandler)
        f next ctx                    
            
    let homeHandler (user:User) (next : HttpFunc) (ctx : HttpContext) =    
        let dataStore = ctx.GetService<IDataStore>()
        let projects = dataStore.GetProjects()
        (Home.view projects |> htmlView) next ctx

    // 管理者権限チェックのサンプル
    let projectHandler id (next : HttpFunc) (ctx : HttpContext) =
        let dataStore = ctx.GetService<IDataStore>()
        let f = 
            dataStore.GetProjects()
            |> List.tryFind (fun x -> x.ID = id)
            |> fun x -> match x with
                        | None -> notFound "NotFound"
                        | Some p -> text p.Name 
        f next ctx
        

    // ルーティング処理
    let webApp : HttpHandler =
        choose [
            GET >=> 
                choose [
                    route  "/"           >=> toLogin
                    route  "/login"      >=> (Login.view |> htmlView) 
                ]               
            GET >=> setHttpHeader "Cache-Control" "no-cache" >=> mustBeUser >=>
                choose [
                    route  "/logout"     >=> signOut authScheme >=> toLogin
                    route  "/home"       >=> loginUserWith homeHandler
                    routef "/project/%i"     projectHandler
                ]
            POST >=> 
                choose [
                    route  "/login"      >=> tryBindForm<LoginRequest> (fun _ -> toLogin) None loginHandler
                ]            
            notFound "Not Found"
            ]


