namespace EVM

type IDataStore =
    abstract member CreateUser : User -> User
    abstract member GetUsers : unit -> User list
    abstract member UpdateUser : User -> bool
    abstract member DeleteUser : User -> bool

    // abstract member CreateProject : Project -> Project
    // abstract member GetProjects : unit -> Project list
    // abstract member UpdateProject : Project -> bool
    // abstract member DeleteProject : Project -> bool

    // abstract member CreateTaskItem : TaskItem -> TaskItem
    // abstract member GetTaskItems : unit -> TaskItem list
    // abstract member UpdateTaskItem : TaskItem -> bool
    // abstract member DeleteTaskItem : TaskItem -> bool

    // abstract member CreateSprint : Sprint -> Sprint
    // abstract member GetSprints : unit -> Sprint list
    // abstract member UpdateSprint : Sprint -> bool
    // abstract member DeleteSprint : Sprint -> bool



