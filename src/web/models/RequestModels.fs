namespace EVM.Web

open System

[<CLIMutable>]
type LoginRequest =
    {
        AuthID : string
        Password : string
    }
