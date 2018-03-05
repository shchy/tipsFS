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

    // ---------------------------------
    // Auth
    // ---------------------------------
    
    // 認証に使用するCookieのスキーマ
    let authScheme = CookieAuthenticationDefaults.AuthenticationScheme
    
    // アクセス権がないときのハンドラ
    let accessDenied = setStatusCode 401 >=> text "Access Denied"

    // Aspnetの認証が済んでないとaccessDeniedへルーティングするHttpHandlerのラッパー
    let mustBeUser = requiresAuthentication accessDenied

    // Aspnetの認証が済んでないorロールがAdmin出ない場合にaccessDeniedへルーティングするHttpHandlerのラッパー
    let mustBeAdmin =
        requiresAuthentication accessDenied
        >=> requiresRole "Admin" accessDenied

    // // 認証済のClaimsPrincipalからユーザ名を見てアクセス権を判定しエラーへ流すラッパ
    // let mustBeJohn =
    //     requiresAuthentication accessDenied
    //     >=> requiresAuthPolicy (fun u -> u.HasClaim (ClaimTypes.Name, "John")) accessDenied

    let toLogin = redirectTo false "/"

    // ログインハンドラ
    let loginHandler (requestUser:LoginRequest) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
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

    let loginUserWith (authedHandler:User -> HttpHandler) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let userID = ctx.User.Identity.Name
            let dataStore = ctx.GetService<IDataStore>()

            let findUser = 
                dataStore.GetUsers()
                |> List.where (fun u -> u.AuthID = userID)
                |> List.tryHead

            match findUser with
            | None -> toLogin next ctx
            | Some user -> (user |> authedHandler) next ctx
        

    let homeHandler (user:User) =
            fun (next : HttpFunc) (ctx : HttpContext) ->
                let dataStore = ctx.GetService<IDataStore>()
                let projects = dataStore.GetProjects()
                (Home.view projects |> htmlView) next ctx
            


    // 管理者権限チェックのサンプル
    let showUserHandler id =
        mustBeAdmin >=>
        text (sprintf "User ID: %i" id)

    // ルーティング処理
    let webApp : HttpHandler =
        choose [
            GET >=> setHttpHeader "Cache-Control" "no-cache" >=>
                choose [
                    route  "/"           >=> (Login.view |> htmlView) 
                    route  "/login"      >=> tryBindQuery<LoginRequest> (fun _ -> toLogin) None loginHandler
                ]   
            GET >=> setHttpHeader "Cache-Control" "no-cache" >=> mustBeUser >=>
                choose [
                    route  "/home"       >=> loginUserWith homeHandler

                    route  "/logout"     >=> signOut authScheme >=> text "Successfully logged out."
                    routef "/user/%i"    showUserHandler
                    
                ]
            RequestErrors.notFound (text "Not Found") ]


