namespace EVM

open System
open Giraffe

[<CLIMutable>]
type User =
    {
        ID : int
        Name : string
        AuthID : string
        Password : string
    }

[<CLIMutable>]
type Tag = string

[<CLIMutable>]
type TaskItem =
    {
        ID : int
        Name : string
        Value : double
        Tags : Tag list
    }

[<CLIMutable>]
type Range<'T> = 
    {
        Start : 'T
        End : 'T
    }

[<CLIMutable>]
type Sprint =
    {
        ID : int
        Name : string
        Tasks : TaskItem list
        Range : Range<DateTime> /// todo 逆にスプリント内の営業日が変化するのを受け入れるほうがわかりやすい？
    }

[<CLIMutable>]
type Project =
    {
        ID : int
        Name : string
        Sprints : Sprint list
        Tasks : TaskItem list   /// todo 優先度の概念がある
        Users : User list
    }

/// タスクの割当
[<CLIMutable>]
type TaskAssign =
    {
        TaskID : int
        UserID : int
    }

/// ---------こっからEVM色が強いよ

/// 勤怠入力のための登録番号的な位置づけ
[<CLIMutable>]
type WorkUnit =
    {
        ID : int
        Name : string
        ProjectID : int
        Range : Range<DateTime>
    }

/// TaskItemの進捗履歴
[<CLIMutable>]
type TaskRecord =
    {
        TaskID : int
        Value : double
        Timestamp : DateTime
        User : User
    }

[<CLIMutable>]
type WorkRecord =
    {
        TimeStamp : DateTime
        User : User
        Value : double
    }


 [<CLIMutable>]
type Car =
    {
        Name   : string
        Make   : string
        Wheels : int
        Built  : DateTime
    }
    interface IModelValidation<Car> with
        member this.Validate() =
            if this.Wheels > 1 && this.Wheels <= 6 then Ok this
            else Error (RequestErrors.BAD_REQUEST "Wheels must be a value between 2 and 6.")


