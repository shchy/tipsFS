namespace EVM.Web

open Giraffe
open EVM
open EVM.Web
open EVM.Web.Handler
open EVM.Web.View

module WebApp =

    let setCacheControl = setHttpHeader "Cache-Control" 
    let setNoCache = setCacheControl "no-cache"

    // ルーティング処理
    let webApp : HttpHandler =
        choose [
            GET >=> 
                choose [
                    route  "/"           >=> redirectTo false "/login" 
                    route  "/login"      >=> setNoCache >=> requiresAuthentication ((Login.view None |> htmlView)) >=> redirectTo false "/home"
                ]               
            GET >=> setNoCache >=> Common.mustBeUser >=>
                choose [
                    route  "/logout"      >=> signOut Common.authScheme >=> redirectTo false "/login"
                    route  "/home"        >=> Login.loginUserWith Home.homeHandler
                    routef "/project/%i"      Project.projectHandler 
                ]
            POST >=> 
                choose [
                    route  "/login"      >=> tryBindForm<LoginRequest> (fun _ -> Common.toLogin Message.loginError) None Login.loginHandler
                ]
            Common.notFound "Not Found"
            ]


