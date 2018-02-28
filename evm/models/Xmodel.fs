namespace EVM

// module Model =
    open System

    type User =
        {
            ID : int
            Name : string
        }

    type Tag = string


    type TaskItem =
        {
            ID : int
            Name : string
            Value : double
        }

    type Range<'T> = 
        {
            Start : 'T
            End : 'T
        }

    type Sprint =
        {
            ID : int
            Name : string
            Tasks : TaskItem list
            Range : Range<DateTime> /// todo 逆にスプリント内の営業日が変化するのを受け入れるほうがわかりやすい？
        }

    type Project =
        {
            ID : int
            Name : string
            Sprints : Sprint list
            Tasks : TaskItem list   /// todo 優先度の概念がある
            Users : User list
        }


    /// ---------こっからEVM色が強いよ



    /// TaskItemの進捗履歴
    type TaskRecord =
        {
            TaskID : int
            Value : double
            Timestamp : DateTime
            User : User
        }



    /// タスクの割当
    type TaskAssign =
        {
            TaskID : int
            UserID : int
        }

    type WorkRecord =
        {
            TimeStamp : DateTime
            User : User
            Value : double
        }