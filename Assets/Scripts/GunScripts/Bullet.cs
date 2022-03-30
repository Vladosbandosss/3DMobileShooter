using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
   [SerializeField]private LayerMask collisionMask;

   [SerializeField]private float speed = 10f;
   [SerializeField]private float destroyAfter = 2f;
   [SerializeField]private float damage = 1f;
   [SerializeField]private float skinWidth = 1f;

   [SerializeField]private Color trailColor;

   [SerializeField]private TrailRenderer trailRenderer;

   private void Start()
   {
      Destroy(gameObject,destroyAfter);

      Collider[] initialCollision = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
      if (initialCollision.Length > 0)
      {
         OnHitObject(initialCollision[0],transform.position);
      }
      
      trailRenderer.material.SetColor("_TintColor",trailColor);
   }

   private void Update()
   {
      CheckCollisions(speed * Time.deltaTime);
      transform.Translate(Vector3.forward*Time.deltaTime*speed);
   }

   public void SetSpeed(float newSpeed)
   {
      speed = newSpeed;
   }

   private void CheckCollisions(float moveDistance)
   {
      Ray ray = new Ray(transform.position, transform.forward);
      RaycastHit hit;

      if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, 
         collisionMask, QueryTriggerInteraction.Collide))
      {
         OnHitObject(hit.collider,hit.point);
      }
   }

   private void OnHitObject(Collider c, Vector3 hitPoint)
   {
      IDamageable damageableObject = c.GetComponent<IDamageable>();

      if (damageableObject != null)
      {
         damageableObject.TakeHit(damage,hitPoint,transform.forward);
      }
      
      Destroy(gameObject);
   }
}
