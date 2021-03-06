module Farmer.Helpers

open Farmer
open System

let sanitise filters maxLength (resourceName:ResourceName) =
    resourceName.Value.ToLower()
    |> Seq.filter(fun c -> Seq.exists(fun filter -> filter c) filters)
    |> Seq.truncate maxLength
    |> Seq.toArray
    |> System.String
let sanitiseStorage = sanitise [ Char.IsLetterOrDigit ] 16
let sanitiseSearch = sanitise [ Char.IsLetterOrDigit; (=) '-' ] 60
let santitiseDb = sanitise [ Char.IsLetterOrDigit ] 100 >> fun r -> r.ToLower()
let tryMergeResource<'T when 'T :> IResource> resourceName (merge:'T -> 'T) (existingResources:IResource list) =
    existingResources
    |> List.filter(fun g -> g.ResourceName = resourceName)
    |> List.tryPick(function :? 'T as resource -> Some resource | _ -> None)
    |> Option.map merge
    |> Option.defaultWith (fun () -> failwithf "could not locate %O" resourceName)