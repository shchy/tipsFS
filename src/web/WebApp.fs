module EVM.Web.WebApp

open System
open System.Globalization
open Microsoft.AspNetCore.Http
open Giraffe
open EVM
open EVM.Web
open EVM.Web.Handler
open EVM.Web.View



let setCacheControl = setHttpHeader "Cache-Control" 
let setNoCache = setCacheControl "no-cache"


let tryBind<'T>     (parseHandler   : (string -> HttpHandler) -> (System.Globalization.CultureInfo option) -> ('T -> HttpHandler) -> HttpHandler ) 
                    (errorHandler   : string option -> HttpHandler)
                    (successHandler : 'T -> HttpHandler) : HttpHandler =
    parseHandler    
        (Some >> errorHandler)
        None 
        (successHandler)

let tryBindWithUser<'T>     (parseHandler   : (string -> HttpHandler) -> (System.Globalization.CultureInfo option) -> ('T -> HttpHandler) -> HttpHandler ) 
                            (errorHandler   : string option -> User -> HttpHandler)
                            (successHandler : 'T -> User -> HttpHandler) : HttpHandler =
    
    tryBind<'T> parseHandler
                (fun validated -> (Login.loginUserWith (errorHandler validated)))
                (successHandler >> Login.loginUserWith)

let tryBindFormWithUser<'T> = tryBindWithUser tryBindForm<'T>
    

// ルーティング処理
let webApp : HttpHandler =
    choose [
        GET >=> 
            choose [
                
                route  "/"                      >=> redirectTo false "/login" 
                route  "/login"                 >=> setNoCache >=> Common.toLogin None |> requiresAuthentication >=> redirectTo false "/home"
            ]    
        GET >=> setNoCache >=> Common.mustBeUser >=>
            choose [
                route  "/logout"                >=> setNoCache >=> Common.mustBeUser >=> signOut Common.authScheme >=> redirectTo false "/login"
                route  "/home"                  >=> setNoCache >=> Common.mustBeUser >=> Login.loginUserWith Home.homeHandler
                routef "/project/%i"            (Login.loginUserWith << Project.viewHandler)
                route  "/project/create"        >=> setNoCache >=> Common.mustBeUser >=> Login.loginUserWith (Project.createHandler None)
                route  "/task/create"           >=> setNoCache >=> Common.mustBeUser >=> Login.loginUserWith (Task.createHandler None) 
            ]
        POST >=> 
            choose [
                route  "/login"                 >=> tryBind<LoginRequest> tryBindForm<LoginRequest> Common.toLogin Login.loginHandler 
            ]                  
        
        POST >=> Common.mustBeUser >=>
            choose [
                route  "/project/create"        >=> tryBindFormWithUser<CreateProjectRequest> Project.createHandler Project.tryCreateHandler
                route  "/task/create"           >=> tryBindFormWithUser<CreateTaskRequest> Task.createHandler Task.tryCreateHandler
            ]            
        Common.notFound "Not Found"
        ]


