namespace EVM.Web.View

open Giraffe
open Giraffe.GiraffeViewEngine

module Layout =
    let viewWithHeaders (titleHeader:string) (headers: XmlNode list) (content: XmlNode list)  =
        html [] [
            head [] (
                [
                    meta [ _charset "UTF-8" ]
                    meta [ _name "viewport"; _content "width=device-width, initial-scale=1, shrink-to-fit=no" ]
                    
                    title []  [ rawText titleHeader ]

                    link [_rel "stylesheet"; _href "/css/bootstrap.min.css"; _crossorigin "anonymous" ]
                    script [_src "https://code.jquery.com/jquery-3.2.1.slim.min.js"; attr "integrity" "sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN"; _crossorigin "anonymous" ] []
                    script [_src "https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.9/umd/popper.min.js"; attr "integrity" "sha384-ApNbgh9B+Y1QKtv3Rn7W3mgPxhU9K/ScQsAP7hUibX39j7fakFPskvXusvfa0b4Q"; _crossorigin "anonymous" ] []
                    script [_src "/js/bootstrap.min.js"; _crossorigin "anonymous" ] []
                ] @ headers)
            body [] content
        ]

    let view (titleHeader:string) (content: XmlNode list) = 
        viewWithHeaders titleHeader List.Empty content

    