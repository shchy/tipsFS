module EVM.Web.WebServer
// アプリにあんまり依存しないWebServerの処理

open System
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open EVM



// 認証に使用するCookieのスキーマ
let authScheme = CookieAuthenticationDefaults.AuthenticationScheme


// エラーハンドラ
let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message


// 基本設定
let configureApp (app : IApplicationBuilder) (routing:HttpFunc -> HttpContext -> HttpFuncResult) =
    app
        // Giraffeの拡張：エラーハンドラの定義
        .UseGiraffeErrorHandler(errorHandler)
        // AspNet:静的ファイルを使う 
        .UseStaticFiles()
        // AspNet:認証使う
        .UseAuthentication()
        // Giraffeの拡張：ルーティング
        .UseGiraffe routing


// Cookie認証の設定
let cookieAuth (o : CookieAuthenticationOptions) =
    do
        o.Cookie.HttpOnly     <- true
        o.Cookie.SecurePolicy <- CookieSecurePolicy.SameAsRequest
        o.SlidingExpiration   <- true
        o.ExpireTimeSpan      <- TimeSpan.FromDays 7.0


// サービスの追加設定
let configureServices (services : IServiceCollection) =
    services
        // giraffe:giraffeの登録
        .AddGiraffe()
        // aspnet:認証サービスの登録
        .AddAuthentication(authScheme)
        // aspnet:認証用のCookieサービスの登録
        .AddCookie(cookieAuth)   |> ignore
    // aspnet:データ保護？        
    services.AddDataProtection() |> ignore
    // DBサービス追加
    services.AddSingleton<IDataStore, DebugDataStore>() |> ignore
    


// ログ機能の設定
let configureLogging (loggerBuilder : ILoggingBuilder) =
    loggerBuilder
        // レベルフィルタ
        .AddFilter(fun lvl -> lvl.Equals LogLevel.Error)
        // コンソールへの出力を追加
        .AddConsole()
        // Debugレベルの追加？
        .AddDebug() |> ignore

// 起動
let Run (routing:HttpFunc -> HttpContext -> HttpFuncResult) =
    // AspNetのWebServer生成処理
    WebHost.CreateDefaultBuilder()
        // 基本設定
        .Configure(fun (app:IApplicationBuilder) -> configureApp app routing)
        // サービスの追加
        .ConfigureServices(configureServices)
        // ログ機能の設定
        .ConfigureLogging(configureLogging)
        // 上記構成でビルド
        .Build()
        // 実行
        .Run()
    0
