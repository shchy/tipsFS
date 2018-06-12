module EVM.Web.View.Login

open EVM
open Giraffe
open Giraffe.GiraffeViewEngine

let view (message:string option) =
    [
        div [_class "text-center"] [
            yield form [ _class "form-signin"; _action "/login"; _method "POST"] [
                h1 [_class "h3 mb-3 font-weight-normal"] [
                    rawText "sign in"
                ]    
                label [_for "authID"; _class "sr-only"] [ rawText "ID" ]
                input [_id "authID";_name "authID"; _class "form-control"; _placeholder "Enter User ID" ] 
                label [_for "password"; _class "sr-only"] [ rawText "Password" ]
                input [_type "password"; _id "password"; _name "password"; _class "form-control"; _placeholder "Enter User Password" ] 
            
                button [_type "submit"; _class "btn btn-lg btn-primary btn-block"] [ rawText "Submit" ]
            ]
        
            if message.IsSome then 
                yield div [_class "alert alert-warning"; attr "role" "alert" ] [ rawText message.Value ]
        ]
    ] |> Layout.viewWithHeaders "Login" [ link [_rel "stylesheet"; _href "/css/signin.css"; _crossorigin "anonymous" ] ]