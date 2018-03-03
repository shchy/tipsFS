namespace EVM

type Crud<'M> 
    ( getID : 'M -> int
    , createFrom : int -> 'M -> 'M) =
    
    let mutable models : 'M list = List.Empty

    member this.Create (model:'M) = 
        let newID = 
            models 
            |> List.fold (fun (max:int) (x:'M) -> if max < getID x then getID x else max ) -1
            |> (+) 1
            
        let newOne = createFrom newID model
        models <- models @ [newOne]
        newOne
    member this.Get () = models
    member this.Update model =
        let xs = 
            models
            |> List.where (fun (x:'M) -> getID x = getID model)
        models <- xs @ [model]
        true            
         
    member this.Delete model =
        let xs = 
            models
            |> List.where (fun (x:'M) -> getID x = getID model)
        models <- xs
        true

    
    



type DebugDataStore () =
    let userCRUD = Crud<User>((fun x -> x.ID), (fun id x -> { ID = id; Name = x.Name }) ) 
    
    interface IDataStore with
        member this.CreateUser u = userCRUD.Create u 
        member this.GetUsers () = userCRUD.Get ()
        member this.UpdateUser u = userCRUD.Update u
        member this.DeleteUser u = userCRUD.Delete u

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

