namespace EVM.Web

open EVM
open Giraffe
open Giraffe.GiraffeViewEngine

module Views =

    let loginView () =
        [
            div [_class "container" ] [
                form [] [
                    div [ _class "form-group"] [
                        label [_for "userid"] [ encodedText "ID" ]
                        input [_id "userid"; _class "form-control"; _placeholder "Enter User ID" ] 
                    ]
                    div [ _class "form-group"] [
                        label [_for "password"] [ encodedText "Password" ]
                        input [_type "password"; _id "password"; _class "form-control"; _placeholder "Enter User Password" ] 
                    ]
                    button [_type "submit"; _class "btn btn-primary"] [ encodedText "Submit" ]
                ]
            ]
        ] 