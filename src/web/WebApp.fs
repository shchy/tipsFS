namespace EVM.Web

open System
open System.Security.Claims
open System.Threading
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.Extensions.Configuration
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

    // 認証済のClaimsPrincipalからユーザ名を見てアクセス権を判定しエラーへ流すラッパ
    let mustBeJohn =
        requiresAuthentication accessDenied
        >=> requiresAuthPolicy (fun u -> u.HasClaim (ClaimTypes.Name, "John")) accessDenied

    // ログインハンドラ
    let loginHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                // JohnとしてSignInAsyncしてaspnetに認証情報を保存
                let issuer = "http://localhost:5000"
                let claims =
                    [
                        Claim(ClaimTypes.Name,      "John",  ClaimValueTypes.String, issuer)
                        Claim(ClaimTypes.Surname,   "Doe",   ClaimValueTypes.String, issuer)
                        Claim(ClaimTypes.Role,      "Admin", ClaimValueTypes.String, issuer)
                    ]
                let identity = ClaimsIdentity(claims, authScheme)
                let user     = ClaimsPrincipal(identity)

                do! ctx.SignInAsync(authScheme, user)

                return! text "Successfully logged in" next ctx
            }

    // ログイン中のユーザ情報を表示するハンドラ
    let userHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            text ctx.User.Identity.Name next ctx

    // 管理者権限チェックのサンプル
    let showUserHandler id =
        mustBeAdmin >=>
        text (sprintf "User ID: %i" id)

    // DIのサンプル
    let configuredHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let configuration = ctx.GetService<IConfiguration>()
            text configuration.["HelloMessage"] next ctx

    // キャッシュの使われ具合のテスト用？
    let time() = System.DateTime.Now.ToString()

    // モデルマッパーのエラーハンドラ
    let parsingErrorHandler err = RequestErrors.BAD_REQUEST err

    // ルーティング処理
    let webApp : HttpHandler =
        choose [
            GET >=>
                choose [
                    route  "/"           >=> text "index"
                    route  "/ping"       >=> text "pong"
                    route  "/error"      >=> (fun _ _ -> failwith "Something went wrong!")
                    route  "/login"      >=> loginHandler
                    route  "/logout"     >=> signOut authScheme >=> text "Successfully logged out."
                    route  "/user"       >=> mustBeUser >=> userHandler
                    route  "/john-only"  >=> mustBeJohn >=> userHandler
                    routef "/user/%i"    showUserHandler
                    route  "/person"     >=> (Views.personView { Name = "Html Node" } |> htmlView)
                    route  "/once"       >=> (time() |> text)
                    route  "/everytime"  >=> warbler (fun _ -> (time() |> text))
                    route  "/configured" >=> configuredHandler
                ]
            route "/car"  >=> bindModel<Car> None json
            route "/car2" >=> tryBindQuery<Car> parsingErrorHandler None (validateModel xml)
            RequestErrors.notFound (text "Not Found") ]


