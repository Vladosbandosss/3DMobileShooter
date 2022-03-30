using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    public enum FireMode
    {
        Auto,
        Burst,
        Single
    };
    
    private FireMode fireMode;

    [SerializeField]private Transform[] bulletSpawn;
    [SerializeField]private Bullet bullet;

    private float msBetweenShoots;
    [SerializeField]private float bulletVelocity = 35f;

    [SerializeField]private int burstCount = 3;
    [SerializeField]private int bulletPerMag = 10;

    [SerializeField]private float reloadTime = 0.3f;

    [Header("Effects")] 
    [SerializeField]private Transform shell;
    [SerializeField]private Transform shellEjector;
    private MuzzleFlash muzzleFlash;
    private float nextShootTime;
    [SerializeField]private AudioClip shootAudio;
    [SerializeField]private AudioClip reloadAudio;

    [Header("Recoil")] 
    [SerializeField]private Vector2 kickMinMax = new Vector2(0.05f, 0.2f);
    [SerializeField]private Vector2 recoilAngleMinMax = new Vector2(3f, 5f);
    [SerializeField]private float recoilMoveSettleTime = 0.1f;
    [SerializeField]private float recoilRotationSettleTime = 0.1f;

    private bool triggerReleasedSinceLastShot;
    private int shootsRemainingBirst;
    private int bulletsRemainingInMag;
    private bool isReloading;

    private Vector3 recoilSmoothDampVelocity;

    private float recoilAngle;
    private float minRecoilAngle = 0f;
    private float maxRecoilAngle = 30f;
    private float recoilRotSmoothDampVelocity;

    private void Awake()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shootsRemainingBirst = burstCount;
        bulletsRemainingInMag = bulletPerMag;
    }

    private void LateUpdate()
    {
        transform.localPosition=Vector3.SmoothDamp(transform.localPosition,Vector3.zero,
            ref recoilSmoothDampVelocity,recoilRotationSettleTime);

        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0,
            ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);

        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if (!isReloading && bulletsRemainingInMag == 0)
        {
            Reload();
        }
    }

    private void Shoot()
    {
        if (!isReloading && Time.time > nextShootTime && bulletsRemainingInMag > 0)
        {
            if (fireMode == FireMode.Burst)
            {
                if (shootsRemainingBirst == 0)
                {
                    return;
                }
                else
                {
                    shootsRemainingBirst--;
                }
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            for (int i = 0; i < bulletSpawn.Length; i++)
            {
                if (bulletsRemainingInMag == 0)
                {
                    break;
                }

                bulletsRemainingInMag--;

                nextShootTime = Time.time + msBetweenShoots / 1000f;

                Bullet newBullet = Instantiate(bullet, bulletSpawn[i].position, bulletSpawn[i].rotation);
                
                newBullet.SetSpeed(bulletVelocity);
            }

            Instantiate(shell, shellEjector.position, shellEjector.rotation);
            muzzleFlash.Activate();
            
            transform.localPosition=Vector3.forward *Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, minRecoilAngle, maxRecoilAngle);
            
            AudioManager.instance.PlaySound(shootAudio,transform.position);
        }
    }

    public void Reload()
    {
        if (bulletsRemainingInMag != bulletPerMag && !isReloading)
        {
            StartCoroutine(nameof(AnimateReload));
            AudioManager.instance.PlaySound(reloadAudio,transform.position);
        }
    }

    private IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(0.2f);

        float reloasSpeed = 1 / reloadTime;
        float percent = 0;

        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30f;

        while (percent<=1)
        {
            percent += Time.deltaTime * reloasSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0f, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

            yield return null;
        }

        isReloading = false;
        bulletsRemainingInMag = bulletPerMag;

    }

    public void Aim(Vector3 aimPoint)
    {
        if (!isReloading)
        {
            transform.LookAt(aimPoint);
        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }
    
    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shootsRemainingBirst = burstCount;
    }
}
