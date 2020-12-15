using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShootingCyan : NetworkBehaviour
{
    public Transform cam;
    public Animator tome;
    public Transform tomePosition;
    public BulletMovement bulletPrefab;
    private float timeSinceShot;
    public float chargeRate;
    public float maxCharge;
    public int particleDensity;
    public ParticleSystem particles;
    public PlayerDamage damage;

    [SyncVar]
    private float charge;

    private bool fireButton = false;
    private bool pFireButton = false;

    private Vector3 aimTarget;
    private int sendCountdown = 5;

    void Update()
    {
        var em = particles.emission;
        em.rateOverTime = charge * particleDensity;

        if (isLocalPlayer) updateInput();
        if (isServer) firing();
        pFireButton = fireButton;
    }
    void firing()
    {
        if (fireButton && !pFireButton)
        {
            RpcChangeAnim("Deploy");
            charge = 0;
        }
        if (fireButton)
        {
            charge += chargeRate * Time.deltaTime;
            if (charge > maxCharge) charge = maxCharge;

            timeSinceShot = 1;
        }
        if (!fireButton && pFireButton)
        {

            timeSinceShot = 1;
            RpcChangeAnim("Fire");
            //Ray ray = new Ray(cam.transform.position, cam.transform.eulerAngles);
            //Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //print("casted");
                Debug.DrawLine(ray.origin, hit.point, Color.red, 1000);

                BulletMovement bullet;

                bullet = Instantiate(bulletPrefab);
                bullet.setTarget(tomePosition.position, hit.point, damage);
                bullet.power = charge * .5f;
                bullet.transform.localScale = new Vector3(charge * .5f, charge * .5f, charge * .5f);
                NetworkServer.Spawn(bullet.gameObject);
                bullet.RpcResize(charge * .5f);
                charge = 0;
            }

            
        }
        else timeSinceShot -= Time.deltaTime;
        if (timeSinceShot < 0) RpcChangeAnim("Withdraw");
        
        

    }
    void updateInput()
    {
        bool shouldUpdate = false;
        sendCountdown--;
        if (sendCountdown < 0)
        {
            sendCountdown = 5;
            shouldUpdate = true;
        }

        if (fireButton != Input.GetButton("Fire1")) shouldUpdate = true;

        fireButton = Input.GetButton("Fire1");
        if (PlayerStartup.health <= 0) fireButton = false;
        if (shouldUpdate) CmdInput(fireButton, cam.position, cam.eulerAngles);
    }
    [Command]
    void CmdInput(bool fireInput, Vector3 camPos, Vector3 camRot)
    {
        cam.position = camPos;
        cam.eulerAngles = camRot;
        fireButton = fireInput;
    }
    [ClientRpc]
    void RpcChangeAnim(string anim)
    {
        changeAnim(anim);
    }
    void changeAnim(string anim)
    {
        if (tome.GetCurrentAnimatorStateInfo(0).IsName("Charging") && anim == "Deploy") return;
        if (tome.GetCurrentAnimatorStateInfo(0).IsName("Fire") && anim == "Deploy") return;
        if (tome.GetCurrentAnimatorStateInfo(0).IsName("Idle") && anim == "Withdraw") return;
        if (tome.IsInTransition(0)) return;
        tome.CrossFade(anim, 0);
    }
}
