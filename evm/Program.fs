module Program

open EVM
open System

[<EntryPoint>]
let main _ =

    // 0. ユーザを定義する
    let users = List.init 5 (fun i -> 
        {
            ID=i
            Name=sprintf "User%2d" i
        })

    // 1. プロジェクトを作る
    let project00 = 
        { 
            ID = 1
            Name = "Prj01"
            Sprints = List.Empty
            Tasks = List.Empty
            Users = users 
        }

    // 2. リリースプランニング
    // プロジェクトの機能（やること）を列挙する
    // 優先順位もつける
    let tasks = List.init 10 (fun i -> 
        {
            ID = i
            Name = sprintf "Task%2d" i
            Value = (double)i
        })
    let project01 = {project00 with Tasks = tasks}

    // 3. スプリントプランニング
    // どの機能をどのスプリント（期間）でやるか考える
    let sprintRange = 10.0
    let sprintCount = 4
    let sprints = 
        List.init sprintCount (fun i -> 
        {
            ID = i
            Name = sprintf "Sprint%2d" i
            Range = {Start = DateTime.Now; End = DateTime.Now.AddDays(sprintRange)}
            Tasks = 
            (
                fun (i:int) (t:TaskItem) -> t.ID % sprintCount = i 
                >> fun p -> List.where p project01.Tasks
                <| i
            )
        })
    let project02 = {project01 with Sprints = sprints}

    // 一旦結果出す
    printfn "%O" project02

    // アサインの考え方 上記とは分けたほうが良さそう
    // 4. タスクの割当
    let assigns = 
        List.map (fun (t:TaskItem) -> { TaskID = t.ID; UserID = t.ID % 5 }) project02.Tasks

    // 5. タスク進捗の登録
    let taskRecords = 
        let f = (fun (a:TaskAssign) -> 
            {
                TaskID = a.TaskID
                Timestamp = DateTime.Now
                User = List.find (fun (u:User) -> u.ID = a.UserID) project02.Users
                Value = (List.find (fun (t:TaskItem) -> t.ID = a.TaskID) project02.Tasks).Value / 2.0
            })
        List.map f assigns
    0    

    // EVMのレポート
    // 見積もり量 と 終わった量 と 使った量 が必要
    // todo タスク単位で出すにはタスク単位で使った量の入力が要る
    // todo けどそれってすごく手間だよね。
    // todo     だって、1日の作業をタスク単位で覚えておいたり、トータル工数はみ出てないか確認したりを毎日個人がやるってクソすぎる
    // todo     全員に運用させるのは無理。管理者もチェックに追われるしチェックは機械的には出来ない
    // todo なのでEVMレポートはあくまでスプリント単位とし、使った量はその日の働いた時間だけ入力すれば良いこととする
    // todo トータルの工数管理はするしね
    // スプリント毎にEVMのレポートを生成
    // let makeEVM (sprints:Sprint list) (records:TaskRecord list) =

    //     let isInSprintTask (s:Sprint) (r:TaskRecord) = List.exists (fun (t:TaskItem) -> t.ID = r.TaskID ) s.Tasks

    //     let makeEVM (s:Sprint) = 
    //         list 
    //         |> List.where isInSprintTask s 

            // let a (xs:TaskItem list) (taskID:int) = List.exists (fun (t:TaskItem) -> t.ID = taskID) xs
            // let aa (rx:TaskRecord list) (f:int -> bool) = List.where (fun (r:TaskRecord) -> f r.TaskID) rx
            // let aaa = a >> aa
            // let aaaa = aaa records
            // aaaa


            // let recordInSprint = List.where (fun (r:TaskRecord) -> List.exists (fun (t:TaskItem) -> t.ID = r.TaskID ) s.Tasks ) records
            

        
                
 











    
    0