module Program

open EVM
open EVM.Web




[<EntryPoint>]
let main _ =
    WebServer.Run WebApp.webApp