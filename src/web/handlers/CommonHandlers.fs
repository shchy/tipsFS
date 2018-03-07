namespace EVM.Web.Handler


module Common =
    // open System
    open Microsoft.AspNetCore.Authentication.Cookies
    open Giraffe
    open EVM.Web
    open EVM.Web.View
    
    
    
    // 認証に使用するCookieのスキーマ
    let authScheme = CookieAuthenticationDefaults.AuthenticationScheme


    let toLogin message = Login.view (Some message) |> htmlView
    // アクセス権がないときのハンドラ
    let accessDenied = clearResponse >=> setStatusCode 401 >=>  toLogin Message.accessDenied
    // Aspnetの認証が済んでないとaccessDeniedへルーティングするHttpHandlerのラッパー
    let mustBeUser : HttpHandler = requiresAuthentication accessDenied

    // // Aspnetの認証が済んでないorロールがAdmin出ない場合にaccessDeniedへルーティングするHttpHandlerのラッパー
    // let mustBeAdmin =
    //     requiresAuthentication accessDenied
    //     >=> requiresRole "Admin" accessDenied

    // // 認証済のClaimsPrincipalからユーザ名を見てアクセス権を判定しエラーへ流すラッパ
    // let mustBeJohn =
    //     requiresAuthentication accessDenied
    //     >=> requiresAuthPolicy (fun u -> u.HasClaim (ClaimTypes.Name, "John")) accessDenied

    let notFound message = RequestErrors.notFound (text message) 
    