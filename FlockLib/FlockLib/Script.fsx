// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.
#load "Flocking.fs"

open FlockLib
open UnityEngine

let v1 = Vector3(1.0f, 0.5f, 0.0f)

-1.0f * v1



let v2 = Vector3(v1.x * -1.0f, v1.y * -1.0f, v1.z * -1.0f)

let addVec (a : Vector3) (b : Vector3) : Vector3 = Vector3(a.x + b.x, a.y + b.y, a.z + b.z)

Array.fold addVec Vector3.zero [|v1|]
Array.reduce (+) [|v1;v1|]