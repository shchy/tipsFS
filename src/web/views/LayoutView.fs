namespace EVM.Web

open Giraffe
open Giraffe.GiraffeViewEngine


let layout (content: XmlNode list) =
    html [] [
        head [] [
            title []  [ encodedText "Giraffe" ]
            link [_rel "stylesheet"; _href "/css/bootstrap.min.css"; _crossorigin "anonymous" ]
            script [_src "https://code.jquery.com/jquery-3.2.1.slim.min.js"; attr "integrity" "sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN"; _crossorigin "anonymous" ] []
            script [_src "https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.9/umd/popper.min.js"; attr "integrity" "sha384-ApNbgh9B+Y1QKtv3Rn7W3mgPxhU9K/ScQsAP7hUibX39j7fakFPskvXusvfa0b4Q"; _crossorigin "anonymous" ] []
            script [_src "/js/bootstrap.min.js"; _crossorigin "anonymous" ] []
        ]
        body [] content
    ]