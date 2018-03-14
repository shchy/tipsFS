namespace EVM.Web

open System
open EVM

[<CLIMutable>]
type LoginRequest =
    {
        AuthID : string
        Password : string
    }


[<CLIMutable>]
type CreateProjectRequest =
    {
        Name : string
    }

[<CLIMutable>]
type CreateTaskRequest =
    {
        Name : string
        Value : double
        Tags : Tag list
    }