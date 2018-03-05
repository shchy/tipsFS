namespace EVM

open System

[<CLIMutable>]
type LoginRequest =
    {
        AuthID : string
        Password : string
    }
