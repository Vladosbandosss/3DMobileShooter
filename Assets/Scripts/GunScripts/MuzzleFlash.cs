using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MuzzleFlash : MonoBehaviour
{
   [SerializeField] private GameObject flashHolder;
   
   [SerializeField] private Sprite[] flashSprites;
   [SerializeField] private SpriteRenderer[] spriteRenderers;
   [SerializeField] private float flashTime = 0.0f;
   
   private void Start()
   {
      Deactivate();
   }
   private void Deactivate()
   {
      flashHolder.SetActive(false);
   }

   public void Activate()
   {
      flashHolder.SetActive(true);

      int flashSpriteIndex = Random.Range(0, flashSprites.Length);

      for (int i = 0; i < spriteRenderers.Length; i++)
      {
         spriteRenderers[i].sprite = flashSprites[flashSpriteIndex];
      }
      
      Invoke(nameof(Deactivate),flashTime);
   }
}
