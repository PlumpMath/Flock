namespace FlockLib

open System
open UnityEngine

type Flocking() =
  inherit MonoBehaviour()

  [<SerializeField>]
  let mutable movable = true

  [<SerializeField>]
  let mutable others : Flocking[] = [||]

  [<SerializeField>]
  let mutable moveVec : Vector3 = Vector3.forward
    
  member this.Start() = do 
      others <- this.GetOthers()
      moveVec <- UnityEngine.Random.onUnitSphere

  member this.GetOthers() = 
      GameObject.FindObjectsOfType<Flocking>() |> 
      Array.filter(fun o -> UnityEngine.Object.op_Equality(o, this) = false)

  member this.Neighbours() =
      others

  member this.MoveVec() =
      moveVec

  member this.CalcAlign() =
      let n = Convert.ToSingle(this.Neighbours().Length)
      if n < 1.0f then Vector3.zero else
          let moveVecs = this.Neighbours() |> Array.map (fun o -> o.MoveVec())
          let moveSum = Array.reduce (+) moveVecs
          let moveAverage = moveSum / n
          moveAverage.normalized

  member this.CalcCohesion() =
      let n = Convert.ToSingle(this.Neighbours().Length)
      if n < 1.0f then Vector3.zero else
          let posVecs = this.Neighbours() |> Array.map (fun o -> o.transform.position)
          let posSum = Array.reduce (+) posVecs
          let posAverage = posSum / n
          let diff = posAverage - this.transform.position
          diff.normalized

  member this.CalcSeparation() =
      let n = Convert.ToSingle(this.Neighbours().Length)
      if n < 1.0f then Vector3.zero else
          let distVecs = this.Neighbours() |> 
                         Array.map (fun o -> o.transform.position) |> 
                         Array.map (fun other -> other - this.transform.position) |>
                         Array.filter (fun (v:Vector3) -> v.magnitude < 20.0f)
          if distVecs.Length = 0 then Vector3.zero else
              let distSum = Array.reduce (+) distVecs
              let distAverage = distSum / Convert.ToSingle(distVecs.Length)
              let distInverted = Vector3(distAverage.x * -1.0f, distAverage.y * -1.0f, distAverage.z * -1.0f)
              distInverted.normalized

  member this.Update() = do
      if movable then

        let margin = 150.0f
        let str = 1.0f

        let correction = 
            match this.transform.position with
            | v when v.x < -margin -> Vector3( str, 0.0f, 0.0f)
            | v when v.x >  margin -> Vector3(-str, 0.0f, 0.0f)
            | v when v.y < -margin -> Vector3(0.0f,  str, 0.0f)
            | v when v.y >  margin -> Vector3(0.0f, -str, 0.0f)
            | v when v.z < -margin -> Vector3(0.0f, 0.0f,  str)
            | v when v.z >  margin -> Vector3(0.0f, 0.0f, -str)
            | _ -> Vector3.zero

        let composite = this.CalcAlign() * 1.0f + this.CalcCohesion() * 1.0f + this.CalcSeparation() * 1.2f + correction

        moveVec <- Vector3.ClampMagnitude(moveVec + composite, 10.0f)
        this.transform.Translate(moveVec * Time.deltaTime * 7.0f)

  member this.OnDrawGizmos() = do
      if movable then
          let n = Convert.ToSingle(this.Neighbours().Length)
          if n < 1.0f then do () else do
            let posVecs = this.Neighbours() |> Array.map (fun o -> o.transform.position)
            let posSum = Array.reduce (+) posVecs
            let posAverage = posSum / n
            Gizmos.color <- Color.black
            Gizmos.DrawLine(this.transform.position,  posAverage)

          Gizmos.color <- Color.white
          Gizmos.DrawLine(this.transform.position, this.transform.position + this.CalcAlign() * 2.0f)
          Gizmos.color <- Color.green
          Gizmos.DrawLine(this.transform.position, this.transform.position + this.CalcCohesion() * 2.0f)
          Gizmos.color <- Color.red
          Gizmos.DrawLine(this.transform.position, this.transform.position + this.CalcSeparation() * 2.0f)

  // ALIGNMENT
  // take movement vector from all nearby birds and add up
  // divide it by the number of nearby birds to get the average vector
  // normalize that vector

  // COHESION
  // average position of neighbours

  // SEPARATION
  // average distance to neighbours
  // negate it

  // TOGETHER
  // velocity = ALIGNMENT + COHESION + SEPARATION
