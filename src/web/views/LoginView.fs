namespace EVM.Web.View

open EVM
open Giraffe
open Giraffe.GiraffeViewEngine

module Login =
    let view (message:string option) =
        [
            div [_class "container" ] [
                yield form [_action "/login"; _method "POST"] [
                        div [ _class "form-group"] [
                            label [_for "authID"] [ rawText "ID" ]
                            input [_id "authID";_name "authID"; _class "form-control"; _placeholder "Enter User ID" ] 
                        ]
                        div [ _class "form-group"] [
                            label [_for "password"] [ rawText "Password" ]
                            input [_type "password"; _id "password"; _name "password"; _class "form-control"; _placeholder "Enter User Password" ] 
                        ]
                        button [_type "submit"; _class "btn btn-primary"] [ rawText "Submit" ]
                    ]
                if message.IsSome then 
                    yield div [_class "alert alert-warning"; attr "role" "alert" ] [ rawText message.Value ]
            ] 
        ] |> Layout.view "Login"