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
                    route  "/"                      >=> redirectTo false "/login" 
                    route  "/login"                 >=> setNoCache >=> requiresAuthentication (Common.toLogin None) >=> redirectTo false "/home"
                ]    
            POST >=> 
                choose [
                    route  "/login"                 >=> tryBindForm<LoginRequest> (fun _ -> Common.toLogin (Some Message.loginError)) None Login.loginHandler
                ]                  
            GET >=> setNoCache >=> Common.mustBeUser >=>
                choose [
                    route  "/logout"                >=> signOut Common.authScheme >=> redirectTo false "/login"
                    route  "/home"                  >=> Login.loginUserWith Home.homeHandler
                    routef "/project/%i"            (Project.projectHandler >> Login.loginUserWith)
                    route  "/project/create"        >=> Login.loginUserWith (Project.createProjectHandler None)
                ]
            POST >=> Common.mustBeUser >=>
                choose [
                    route  "/project/create"        >=> tryBindForm<CreateProjectRequest> (fun _ -> Common.toLogin (Some Message.loginError)) None (Project.tryCreateProjectHandler >> Login.loginUserWith)
                ]            
            Common.notFound "Not Found"
            ]


