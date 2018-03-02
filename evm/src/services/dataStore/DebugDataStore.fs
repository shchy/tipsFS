namespace EVM


type DebugDataStore ()=
    let mutable users : User list = List.Empty
    interface IDataStore with
        member this.CreateUser u = 
            let newUser = 
                {
                    ID = (List.fold (fun (max:int) (x:User) -> if max < x.ID then x.ID else max ) -1 users) + 1
                    Name = u.Name
                }
            users <- users @ [newUser]
            newUser
        member this.GetUsers () = users
        member this.UpdateUser u =
            let xs = 
                users
                |> List.where (fun (x:User) -> x.ID = u.ID)
            users <- xs @ [u]
            true            
             
        member this.DeleteUser u =
            let xs = 
                users
                |> List.where (fun (x:User) -> x.ID = u.ID)
            users <- xs
            true

        // member this.CreateProject : Project -> Project
        // member this.GetProjects : unit -> Project list
        // member this.UpdateProject : Project -> bool
        // member this.DeleteProject : Project -> bool

        // member this.CreateTaskItem : TaskItem -> TaskItem
        // member this.GetTaskItems : unit -> TaskItem list
        // member this.UpdateTaskItem : TaskItem -> bool
        // member this.DeleteTaskItem : TaskItem -> bool

        // member this.CreateSprint : Sprint -> Sprint
        // member this.GetSprints : unit -> Sprint list
        // member this.UpdateSprint : Sprint -> bool
        // member this.DeleteSprint : Sprint -> bool

