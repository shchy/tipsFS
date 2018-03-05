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

    // // Aspnetの認証が済んでないとaccessDeniedへルーティングするHttpHandlerのラッパー
    // let mustBeUser = requiresAuthentication accessDenied

    // Aspnetの認証が済んでないorロールがAdmin出ない場合にaccessDeniedへルーティングするHttpHandlerのラッパー
    let mustBeAdmin =
        requiresAuthentication accessDenied
        >=> requiresRole "Admin" accessDenied

    // // 認証済のClaimsPrincipalからユーザ名を見てアクセス権を判定しエラーへ流すラッパ
    // let mustBeJohn =
    //     requiresAuthentication accessDenied
    //     >=> requiresAuthPolicy (fun u -> u.HasClaim (ClaimTypes.Name, "John")) accessDenied

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
                    | None -> return! redirectTo true "/" next ctx
                    | Some user ->
                        // JohnとしてSignInAsyncしてaspnetに認証情報を保存
                        let issuer = "http://localhost:5000"
                        let claims =
                            [
                                Claim(ClaimTypes.Name,      user.Name,  ClaimValueTypes.String, issuer)
                                Claim(ClaimTypes.NameIdentifier,   user.AuthID,   ClaimValueTypes.String, issuer)
                                Claim(ClaimTypes.Role,      "Admin", ClaimValueTypes.String, issuer)
                            ]
                        let identity = ClaimsIdentity(claims, authScheme)
                        let claim     = ClaimsPrincipal(identity)

                        do! ctx.SignInAsync(authScheme, claim)

                        return! redirectTo true "/home" next ctx
            }

    // ログイン中のユーザ情報を表示するハンドラ
    let userHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            text ctx.User.Identity.Name next ctx

    // 管理者権限チェックのサンプル
    let showUserHandler id =
        mustBeAdmin >=>
        text (sprintf "User ID: %i" id)


    // キャッシュの使われ具合のテスト用？
    let time() = System.DateTime.Now.ToString()

    // モデルマッパーのエラーハンドラ
    let parsingErrorHandler err = RequestErrors.BAD_REQUEST err

    // ルーティング処理
    let webApp : HttpHandler =
        choose [
            GET >=> 
                choose [
                    route  "/"           >=> (Views.loginView |> htmlView) 
                    route  "/login"      >=> tryBindQuery<LoginRequest> (fun _ -> redirectTo true "/") None loginHandler
                    route  "/home"       >=> text "home"


                    route  "/logout"     >=> signOut authScheme >=> text "Successfully logged out."
                    routef "/user/%i"    showUserHandler
                    route  "/everytime"  >=> warbler (fun _ -> (time() |> text))
                    
                ]
            // POST >=> 
            //     choose [
            //         route  "/login"      >=> setHttpHeader "Cache-Control" "no-cache" >=> tryBindForm<LoginRequest> (fun _ -> redirectTo true "/") None loginHandler                    
            //     ]            
            route "/car"  >=> bindModel<Car> None json
            route "/car2" >=> tryBindQuery<Car> parsingErrorHandler None (validateModel xml)
            RequestErrors.notFound (text "Not Found") ]


