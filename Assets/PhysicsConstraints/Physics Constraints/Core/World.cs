﻿/******************************************************************************/
/*
  Project - Physics Constraints
            https://github.com/TheAllenChou/unity-physics-constraints
  
  Author  - Ming-Lun "Allen" Chou
  Web     - http://AllenChou.net
  Twitter - @TheAllenChou
*/
/******************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace PhysicsConstraints
{
  public class World : MonoBehaviour
  {
    private static int s_velocityIterations = 10;
    public static int VelocityIterations
    {
      get { return s_velocityIterations; }
      set { s_velocityIterations = Mathf.Max(1, value); }
    }

    public static Vector3 Gravity = Vector3.zero;

    private static IBroadphase m_broadphase = new NSquared();
    public static IBroadphase Broadphase
    {
      get { return m_broadphase; }
      set
      {
        if (value != null)
          m_broadphase = value;
        else
          m_broadphase = new NSquared();
      }
    }

    // constraints
    private static ICollection<IPhysicsConstraint> s_constraints;
    public static void Register(IPhysicsConstraint c)
    {
      ValidateWorld();
      s_constraints.Add(c);
    }
    public static void Unregister(IPhysicsConstraint c)
    {
      if (s_constraints == null)
        return;

      s_constraints.Remove(c);
    }

    // contacts
    private static ICollection<Contact> s_contacts;
    public static void Register(Contact contact)
    {
      ValidateWorld();
      s_contacts.Add(contact);
    }
    // no need to unregister, as contacts are cleared every time step

    // physics bodies
    private static ICollection<PhysicsBody> s_bodies;
    public static void Register(PhysicsBody b)
    {
      ValidateWorld();
      s_bodies.Add(b);
    }
    public static void Unregister(PhysicsBody b)
    {
      if (s_bodies == null)
        return;

      s_bodies.Remove(b);
    }

    // colliders
    private static HashSet<PhysicsCollider> s_colliders;
    public static void Register(PhysicsCollider collider)
    {
      ValidateWorld();
      s_colliders.Add(collider);
    }
    public static void Unregister(PhysicsCollider collider)
    {
      s_colliders.Remove(collider);
    }

    private static GameObject s_world;
    private static void ValidateWorld()
    {
      
      if (s_world != null)
        return;

      s_constraints = new HashSet<IPhysicsConstraint>();
      s_contacts = new List<Contact>();
      s_bodies = new HashSet<PhysicsBody>();
      s_colliders = new HashSet<PhysicsCollider>();

      s_world = new GameObject("World (Physics Constraints)");
      s_world.AddComponent<World>();
    }

    private void FixedUpdate()
    {
      if (s_world != gameObject)
      {
        Destroy(gameObject);
        return;
      }

      float dt = Mathf.Max(ConstraintUtil.Epsilon, Time.fixedDeltaTime);
      Step(Time.fixedDeltaTime);
    }

    public static void Step(float dt)
    {
      // inertia
      foreach (var body in s_bodies)
      {
        body.UpdateInertiaWs();
      }

      // collision detection
      {
        var colliderPairs = Broadphase.GenerateColliderPairs(s_colliders);
        foreach (var colliderPair in colliderPairs)
        {
          Contact contact = null;
          if (!Collision.DetectCollision(colliderPair.ColliderA, colliderPair.ColliderB, ref contact))
            continue;

          Register(contact);
        }
      }

      // gravity
      {
        Vector3 gravityImpulse = Gravity * dt;
        foreach (var body in s_bodies)
        {
          body.LinearVelocity += gravityImpulse * body.GravityScale;
        }
      }

      // init constraints
      foreach (var contact in s_constraints)
      {
        contact.InitVelocityConstraint(dt);
      }
      foreach (var contact in s_contacts)
      {
        contact.InitVelocityConstraint(dt);
      }

      // solve constraints
      for (int i = 0; i < s_velocityIterations; ++i)
      {
        foreach (var constraint in s_constraints)
        {
          constraint.SolveVelocityConstraint(dt);
        }
        foreach (var contact in s_contacts)
        {
          contact.SolveVelocityConstraint(dt);
        }
      }

      // clear contacts
      foreach (var contact in s_contacts)
      {
        Pool<Contact>.Store(contact);
      }
      s_contacts.Clear();

      // integrate
      foreach (var body in s_bodies)
      {
        body.Integrate(dt);
      }

      // drag
      foreach (var body in s_bodies)
      {
        body.LinearVelocity *= Mathf.Pow(1.0f - body.LinearDrag, dt);
        body.AngularVelocity *= Mathf.Pow(1.0f - body.AngularDrag, dt);
      }
    }
  }
}
