namespace EVM.Web

open EVM
open Giraffe
open Giraffe.GiraffeViewEngine

module Views =

    let loginView =
        [
            div [_class "container" ] [
                form [_action "/login"; _method "GET"] [
                    div [ _class "form-group"] [
                        label [_for "authID"] [ encodedText "ID" ]
                        input [_id "authID";_name "authID"; _class "form-control"; _placeholder "Enter User ID" ] 
                    ]
                    div [ _class "form-group"] [
                        label [_for "password"] [ encodedText "Password" ]
                        input [_type "password"; _id "password"; _name "password"; _class "form-control"; _placeholder "Enter User Password" ] 
                    ]
                    button [_type "submit"; _class "btn btn-primary"] [ encodedText "Submit" ]
                ]
            ]
        ] |> TemplateViews.layout