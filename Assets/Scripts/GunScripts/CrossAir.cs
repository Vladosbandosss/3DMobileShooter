using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossAir : MonoBehaviour
{
   [SerializeField]private float rotateSpeed = 50f;
   public LayerMask targetMask;
   
   private Color hightLightDotColor=Color.red;
   private Color orginalDotColor;

   [SerializeField]private SpriteRenderer dot;

   private void Start()
   {
      Cursor.visible = false;
      orginalDotColor = dot.color;
   }

   private void Update()
   {
      transform.Rotate(Vector3.forward*rotateSpeed*Time.deltaTime);
   }

   public void DetectTargets(Ray ray)
   {
      if (Physics.Raycast(ray, 100f, targetMask))
      {
         dot.color = hightLightDotColor;
      }
      else
      {
         dot.color = orginalDotColor;
      }
   }
}
