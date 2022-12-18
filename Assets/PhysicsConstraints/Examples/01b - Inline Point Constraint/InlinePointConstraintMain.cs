/******************************************************************************/
/*
  Project - Physics Constraints
            https://github.com/TheAllenChou/unity-physics-constraints
  
  Author  - Ming-Lun "Allen" Chou
  Web     - http://AllenChou.net
  Twitter - @TheAllenChou
*/
/******************************************************************************/

using CjLib;
using UnityEngine;

public class InlinePointConstraintMain : MonoBehaviour
{
  public float Beta = 0.02f;

  public GameObject Object;
  public GameObject Target;

  private Vector3 v = Vector3.zero;

  private void Update()
  {
    if (Object == null)
      return;

    if (Target == null)
      return;

    float dt = Time.deltaTime;
    var c = Target.transform.eulerAngles - Object.transform.eulerAngles;

    v += (-Beta / dt) * c;
    v *= 0.9f; // temp magic cheat
    Object.transform.rotation = QuaternionUtil.Integrate(Object.transform.rotation, v, dt);
  }
}
