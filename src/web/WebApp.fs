namespace EVM.Web

open Giraffe
open EVM
open EVM.Web
open EVM.Web.Handler
open EVM.Web.View

module WebApp =    
    
    // ルーティング処理
    let webApp : HttpHandler =
        choose [
            GET >=> 
                choose [
                    route  "/"           >=> setHttpHeader "Cache-Control" "no-cache" >=> requiresAuthentication (redirectTo false "/login") >=> redirectTo false "/home"
                    route  "/login"      >=> (Login.view None |> htmlView) 
                ]               
            GET >=> setHttpHeader "Cache-Control" "no-cache" >=> Common.mustBeUser >=>
                choose [
                    route  "/logout"     >=> signOut Common.authScheme >=> redirectTo false "/login"
                    route  "/home"       >=> Login.loginUserWith Home.homeHandler
                    routef "/project/%i"     Project.projectHandler
                ]
            POST >=> 
                choose [
                    route  "/login"      >=> tryBindForm<LoginRequest> (fun _ -> Common.toLogin Msg.loginError) None Login.loginHandler
                ]
            Common.notFound "Not Found"
            ]


