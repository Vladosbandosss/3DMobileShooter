using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShellScript : MonoBehaviour
{
   private Rigidbody rb;
   
   [SerializeField] private float forceMin = 90f, forceMax = 120f;

   private float lifeTime = 3f;
   private float fadeTime = 2f;

   private Material shellmat;
   
   private void Awake()
   {
      rb = GetComponent<Rigidbody>();
   }

   private void Start()
   {
      shellmat = GetComponent<Renderer>().material;

      float force = Random.Range(forceMin, forceMax);
      rb.AddForce(transform.right*force);
      rb.AddTorque(Random.insideUnitSphere*force);

      StartCoroutine(nameof(Fade));
   }

   private IEnumerator Fade()
   {
      yield return new WaitForSeconds(lifeTime);

      float percent = 0f;
      float fadeSpeed = 1 / fadeTime;

      Color initialColor = shellmat.color;

      while (percent < 1f)
      {
         percent += Time.deltaTime * fadeSpeed;
         shellmat.color = Color.Lerp(initialColor, Color.clear, percent);
         yield return null;
      }
      
      Destroy(gameObject);
      
   }
}
