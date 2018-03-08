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

    let updateAttribute (name:string) (value:string) (attrs:XmlAttribute[]) =
        let isMarge = 
            attrs
            |> Array.exists (fun x ->   match x with 
                                        | KeyValue(n, _)    -> name = n 
                                        | _                 -> false) 

        if isMarge then
            seq {
                for attr in attrs do
                    yield 
                        match attr with 
                        | KeyValue(n, v)    -> if name = n then KeyValue(name, (v + " " + value)) else attr
                        | _                 -> attr   
            } |> Array.ofSeq
        else
            Array.append attrs [|KeyValue(name, value)|]


    let mergeAttribute (attr:XmlAttribute) (attrs:XmlAttribute[]) =
        match attr with
        | KeyValue(name, value) -> updateAttribute name value attrs 
        | _ -> (Array.append attrs [|attr|]) 


    let appendAttribute (attr:XmlAttribute) (x:XmlNode)=
        match x with
        | ParentNode((tagName, attrs), childs) -> ParentNode((tagName, (mergeAttribute attr attrs)), childs)
        | VoidElement((tagName, attrs)) -> VoidElement((tagName, (mergeAttribute attr attrs)))
        | _ -> x


    let toListView (makeGroup : XmlNode list -> XmlNode) (f:'a -> XmlNode) (items:'a list) = 
        items
        |> List.map (f >> (appendAttribute (_class "list-group-item")))
        |> makeGroup 
        |> (appendAttribute (_class "list-group"))

    let toListViewDiv (f:'a -> XmlNode) (items:'a list) =
        items
        |> toListView (fun x -> div [] x) f
    let toListViewUl (f:'a -> XmlNode) (items:'a list) =
        items
        |> toListView (fun x -> ul [] x) f    

  
        
        
    