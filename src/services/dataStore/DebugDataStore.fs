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
    let userCRUD = Crud<User>((fun x -> x.ID), (fun id x -> 
        { 
            ID = id 
            Name = x.Name 
            AuthID = x.AuthID
            Password = x.Password
        })) 
    let projectCRUD = Crud<Project>((fun x -> x.ID), (fun id x -> 
        { 
            ID = id
            Name = x.Name
            Sprints = x.Sprints
            Tasks = x.Tasks
            Users = x.Users
        }) ) 
    let taskCRUD = Crud<TaskItem>((fun x -> x.ID), (fun id x -> 
        { 
            ID = id
            Name = x.Name
            Value = x.Value
        }) ) 
    let sprintCRUD = Crud<Sprint>((fun x -> x.ID), (fun id x -> 
        { 
            ID = id 
            Name = x.Name 
            Tasks = x.Tasks
            Range = x.Range
        }) ) 

    do 
        userCRUD.Create {
                ID = -1
                AuthID = "test"
                Name = "testUser"
                Password = "test"
        } |> ignore
        ()


    interface IDataStore with
        member this.CreateUser u = userCRUD.Create u 
        member this.GetUsers () = userCRUD.Get ()
        member this.UpdateUser u = userCRUD.Update u
        member this.DeleteUser u = userCRUD.Delete u

        member this.CreateProject x = projectCRUD.Create x
        member this.GetProjects () = projectCRUD.Get ()
        member this.UpdateProject x = projectCRUD.Update x
        member this.DeleteProject x = projectCRUD.Delete x

        member this.CreateTaskItem x = taskCRUD.Create x
        member this.GetTaskItems () = taskCRUD.Get ()
        member this.UpdateTaskItem x = taskCRUD.Update x
        member this.DeleteTaskItem x = taskCRUD.Delete x

        member this.CreateSprint x = sprintCRUD.Create x
        member this.GetSprints () = sprintCRUD.Get ()
        member this.UpdateSprint x = sprintCRUD.Update x
        member this.DeleteSprint x = sprintCRUD.Delete x

