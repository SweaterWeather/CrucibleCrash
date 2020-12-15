using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShootingPink : NetworkBehaviour
{
    public Transform cam;
    public Animator tome;
    public PinkStageFiring stage1;
    public PinkStageFiring stage2;
    public PinkStageFiring stage3;
    private List<PinkStageFiring> stages = new List<PinkStageFiring>();
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

            switch (stages.Count)
            {
                case 0:
                    if (charge > maxCharge * .3f)
                    {
                        PinkStageFiring stage = Instantiate(stage1);
                        stage.startup(damage);
                        stages.Add(stage);
                        NetworkServer.Spawn(stage.gameObject);
                    }
                    break;
                case 1:
                    if (charge > maxCharge * .6f)
                    {
                        PinkStageFiring stage = Instantiate(stage2);
                        stage.startup(damage);
                        stages.Add(stage);
                        NetworkServer.Spawn(stage.gameObject);
                    }
                    break;
                case 2:
                    if (charge == maxCharge)
                    {
                        PinkStageFiring stage = Instantiate(stage3);
                        stage.startup(damage);
                        stages.Add(stage);
                        NetworkServer.Spawn(stage.gameObject);
                    }
                    break;
            }
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

                foreach(PinkStageFiring stage in stages)
                {
                    stage.fire(hit.point);
                }
                stages = new List<PinkStageFiring>();
            }
            charge = 0;

            
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
