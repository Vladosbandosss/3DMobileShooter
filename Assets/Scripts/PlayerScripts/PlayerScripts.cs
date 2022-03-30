using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScripts : LivingEntity
{
   public float moveSpeed = 4.5f;

   private PlayerController _playerController;
   private Camera viewCamera;

   private Vector3 moveInput, moveVelocity;

   [SerializeField] private FixedJoystick _joystick;

   private Ray lookRay;

   private GunController gunController;

   private Plane groundPlane;

   [SerializeField]private AudioClip playerDeadClip;

   [SerializeField]private CrossAir crossAir;
   
   private void Awake()
   {
      _playerController = GetComponent<PlayerController>();
      viewCamera = Camera.main;

      gunController = GetComponent<GunController>();
   }

   protected override void Start()
   {
      base.Start();

      groundPlane = new Plane(Vector3.up, Vector3.up*gunController.GunHeigth());
   }

   private void Update()
   {
     
      HandleMovement();

      lookRay = viewCamera.ScreenPointToRay(Input.mousePosition);

      float rayDistance;
      
      if (groundPlane.Raycast(lookRay, out rayDistance))
      {
        
         Vector3 point = lookRay.GetPoint(rayDistance);
         _playerController.LookAt(point);

         crossAir.transform.position = point;
         crossAir.DetectTargets(lookRay);
         
         gunController.Aim(point);
      }
      
      if (Input.GetMouseButton(0)&&_joystick.Horizontal==0&&_joystick.Vertical==0)
         { 
            gunController.OntriggerHold();
         }

         if (Input.GetMouseButtonUp(0))
         {
            gunController.OntriggerRelease();
            
         }

         if (transform.position.y < -10f)
         {
            TakeDamage(health);
         }
      }
   

   public override void Die()
   {
      AudioManager.instance.PlaySound(playerDeadClip,transform.position);
      base.Die();
   }

   public void ReloadMobile()//отключил!
   {
      gunController.Reload();
   }

   public void ChangeGun()
   {
      gunController.ChangeGun();
   }

   private void HandleMovement()
   {
      moveInput = new Vector3(_joystick.Horizontal, 0f, _joystick.Vertical);

      moveVelocity = moveInput.normalized * moveSpeed;
      
      _playerController.Move(moveVelocity);
      
   }
   
}
