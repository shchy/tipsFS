module Program

open EVM.Web

[<EntryPoint>]
let main _ =
    WebApp.webApp |> WebServer.Run 