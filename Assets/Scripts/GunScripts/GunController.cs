using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
   [SerializeField]private Transform weaponHold;

   [SerializeField]private Gun[] allGuns;
   private int gunIndex;

   private Gun _equpidGun;

   private void Start()
   {
      EquipGun(allGuns[gunIndex]);
   }
   
   public void EquipGun(Gun gunToEquip)
   {
      if (_equpidGun != null)
      {
         Destroy(_equpidGun.gameObject);
      }

      _equpidGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation);
      _equpidGun.transform.parent = weaponHold;
   }

   public void OntriggerHold()
   {
      if (_equpidGun != null)
      {
         _equpidGun.OnTriggerHold();
      }
   }
   
   public void OntriggerRelease()
   {
      if (_equpidGun != null)
      {
         _equpidGun.OnTriggerRelease();
      }
   }


   public float GunHeigth()
   {
      return weaponHold.position.y;
   }

   public void Aim(Vector3 aimPoint)
   {
      if (_equpidGun )
      {
         _equpidGun.Aim(aimPoint);
      }
   }

   public void Reload()
   {
      if (_equpidGun)
      {
         _equpidGun.Reload();
      }
   }

   public void ChangeGun()
   {
      gunIndex++;
      if (gunIndex == allGuns.Length)
      {
         gunIndex = 0;
      }
      
      EquipGun(allGuns[gunIndex]);
   }
}
