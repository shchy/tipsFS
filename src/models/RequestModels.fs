namespace EVM.Web

open System
open Giraffe

[<CLIMutable>]
type LoginRequest =
    {
        AuthID : string
        Password : string
    }
