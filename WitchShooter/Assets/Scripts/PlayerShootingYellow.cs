using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShootingYellow : NetworkBehaviour
{
    public Transform cam;
    public Animator tome;
    public Transform tomePosition;
    public BulletMovement bulletPrefab;
    private float timeSinceShot;
    public float firingSpeed;
    public PlayerDamage damage;
    public ParticleSystem particles;

    private float timeTillShoot = 1;

    private bool fireButton = false;
    private bool pFireButton = false;

    private Vector3 aimTarget;
    private int sendCountdown = 5;

    void Update()
    {
        if (isLocalPlayer) updateInput();
        if (isServer) firing();
        pFireButton = fireButton;

        var em = particles.emission;
        em.rateOverTime = 0;
    }
    void firing()
    {
        if (fireButton && !pFireButton)
        {
            RpcChangeAnim("Deploy");
        }
        if (fireButton)
        {
            timeSinceShot = 1;
            timeTillShoot -= Time.deltaTime * firingSpeed;
            if (timeTillShoot < 0)
            {
                timeTillShoot = 1;
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
                    NetworkServer.Spawn(bullet.gameObject);
                }

            }
        }
        else timeSinceShot -= Time.deltaTime;
        if (timeSinceShot < 0) RpcChangeAnim("Withdraw");
        if (!fireButton && pFireButton)
        {
            timeTillShoot = 1;
            //bullet.setTarget(tome.transform.position, )
        }

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
