using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour {
    
    public Animator p1;
    public Animator p2;
    public Animator p3;
    public Animator p4;

    public PlayerShooting magShoot;
    public PlayerShootingCyan cyaShoot;
    public PlayerShootingPink pinkShoot;
    public PlayerShootingYellow yellShoot;

    public Transform cam;
    public Animator tome;
    public CharacterController controller;
    [SyncVar]
    public float stamina = 5;
    public float topStamina = 5;

    //public Rigidbody body;
    private Animator avatar;
    private bool grounded = true;
    private float dashing = 0;
    [SyncVar]
    private float jumpCharge = 0;

    public int speed;

    public float airResistance;
    public int jump;
    public int jumpChargeRate;
    public float jumpChargeMax;
    private Vector3 velocity = new Vector3();

    public int dashSpeed;
    public float baseDashCost;

    public float camSpeed; 

    private Vector3 pPos = new Vector3();
    private float pmouseX;
    private float pmouseY;

    public ParticleSystem particles;
    public int particleDensity;
    private bool isTransitioning = false;

    private float vertAxis = 0;
    private float horzAxis = 0;
    private bool jumpButton = false;
    private bool pJumpButton = false;
    private bool dashButton = false;
    private bool pDashButton = false;

    public Camera cameraToKill;
    public Text username;

    //////*REPLACE THIS*//////
    [SyncVar]
    private int chosenCharacter = 0;
    [SyncVar]
    private string nickName = "";
    private bool shouldSkin = true;

    // Use this for initialization
    //[ClientCallback]
    void Start () {
        if (isLocalPlayer)
        {
            pmouseX = Input.mousePosition.x;
            pmouseY = Input.mousePosition.y;

            CmdSetSkin(PlayerStartup.choseCharacter, PlayerStartup.username);
        }
        if (!isLocalPlayer) cameraToKill.gameObject.SetActive(false);
	}

    [Command]
    void CmdSetSkin(int skin, string username)
    {
        print(skin + "" + username);
        chosenCharacter = skin;
        nickName = username;
        ServerScore.setSkin(chosenCharacter, nickName, this);
    }
    
    void setSkin()
    {
        switch (chosenCharacter)
        {
            case 1:
                avatar = p1;
                p2.gameObject.SetActive(false);
                p3.gameObject.SetActive(false);
                p4.gameObject.SetActive(false);

                magShoot.enabled = true;
                cyaShoot.enabled = false;
                pinkShoot.enabled = false;
                yellShoot.enabled = false;
                break;
            case 2:
                avatar = p2;
                p1.gameObject.SetActive(false);
                p3.gameObject.SetActive(false);
                p4.gameObject.SetActive(false);

                magShoot.enabled = false;
                cyaShoot.enabled = false;
                pinkShoot.enabled = true;
                yellShoot.enabled = false;
                break;
            case 3:
                avatar = p3;
                p2.gameObject.SetActive(false);
                p1.gameObject.SetActive(false);
                p4.gameObject.SetActive(false);

                magShoot.enabled = false;
                cyaShoot.enabled = true;
                pinkShoot.enabled = false;
                yellShoot.enabled = false;
                break;
            case 4:
                avatar = p4;
                p2.gameObject.SetActive(false);
                p3.gameObject.SetActive(false);
                p1.gameObject.SetActive(false);

                magShoot.enabled = false;
                cyaShoot.enabled = false;
                pinkShoot.enabled = false;
                yellShoot.enabled = true;
                break;
        }
        shouldSkin = false;

        if (avatar) {
            if (isLocalPlayer) avatar.GetComponentInChildren<TextMesh>().gameObject.SetActive(false);
            else if (nickName != "") avatar.GetComponentInChildren<TextMesh>().text = nickName;

            //CmdSetUpForGameOver();
        }
        /*foreach (Transform child in avatar.transform)
        {
            if (avatar && isLocalPlayer)
            {
                if (child.gameObject.layer == 10) child.gameObject.SetActive(false);
            }
        }*/
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (shouldSkin && chosenCharacter != 0) setSkin();
        if (!avatar) return;

        var em = particles.emission;
        em.rateOverTime = jumpCharge * particleDensity;

        if (isLocalPlayer) {
            updateInput();
            updateCam();
        }
        if (isServer)
        {
            if (dashing > 0) dashMovement();
            else if (!grounded) airMovement();
            else groundMovement();
        }
        //if (velocity.y < Physics.gravity.y) velocity.y = Physics.gravity.y;
    }
    private void FixedUpdate()
    {
        grounded = false;
    }
    [ServerCallback]
    void dashMovement()
    {
        jumpCharge = 0;
        if (stamina < 0) dashing = 0;
        changeAnim("Dash");
        if (!dashButton) dashing -= Time.deltaTime;
        else stamina -= Time.deltaTime;
        //controller.Move(transform.forward * dashSpeed * Time.deltaTime);float vert = Input.GetAxis("Vertical");

        float vert = vertAxis;
        float horz = horzAxis;
        float rad = transform.eulerAngles.y * Mathf.Deg2Rad;
        Vector3 vect = new Vector3();

        if (vert != 0 || horz != 0)
        {
            vect = transform.forward * vert;
            vect += transform.right * horz;

            vect.Normalize();

            vect *= dashSpeed;

            //avatar
        }
        else vect = transform.forward * dashSpeed;
        controller.Move(vect * Time.deltaTime);

        pDashButton = dashButton;

        velocity = vect;
        RpcSetStamina(stamina);
    }
    [ServerCallback]
    void groundMovement()
    {
        avatar.transform.localEulerAngles = Vector3.zero;

        velocity.y = 0;
        float vert = vertAxis;
        float horz = horzAxis;
        float rad = transform.eulerAngles.y * Mathf.Deg2Rad;
        Vector3 vect = new Vector3();

        if (vert != 0 || horz != 0)
        {
            vect = transform.forward * vert;
            vect += transform.right * horz;

            vect.Normalize();

            vect *= speed;

            if (vert > 0) changeAnim("Run Forward");
            else if (vert < 0) changeAnim("Run Backward");
            else if (horz < 0) changeAnim("Run Left");
            else if (horz > 0) changeAnim("Run Right");
        }
        else changeAnim("Idle");

        pPos = transform.position;
        //vect.y = body.velocity.y;
        if (dashButton && !pDashButton && stamina >= baseDashCost)
        {
            dashing = .25f;
            stamina -= baseDashCost;
        }

        //vect += velocity;
        controller.SimpleMove(vect);
        velocity = vect;
        if (!jumpButton && pJumpButton)
        {
            velocity += new Vector3(0, jump + jump * jumpCharge);
            grounded = false;
            changeAnim("Jump");
        }
        else if (jumpButton)
        {
            jumpCharge += Time.deltaTime * jumpChargeRate;
            if (jumpCharge > jumpChargeMax) jumpCharge = jumpChargeMax;
        }
        else jumpCharge = 0;
        transform.position += new Vector3(0, velocity.y * Time.deltaTime);

        pJumpButton = jumpButton;
        pDashButton = dashButton;

        stamina += Time.deltaTime;
        if (stamina > topStamina) stamina = topStamina;
        RpcSetStamina(stamina);
    }
    [ServerCallback]
    void airMovement()
    {
        stamina += Time.deltaTime * .025f;
        jumpCharge = 0;
        avatar.transform.localEulerAngles = Vector3.zero;

        if (controller.velocity.y < 0) changeAnim("Fall");
        else changeAnim("Ascend");
        float vert = vertAxis;
        float horz = horzAxis;
        float rad = transform.eulerAngles.y * Mathf.Deg2Rad;
        Vector3 vect = new Vector3();

        if (vert != 0 || horz != 0)
        {
            vect = transform.forward * vert;
            vect += transform.right * horz;

            vect.Normalize();

            vect *= speed * airResistance;
        }
        if (dashButton && !pDashButton && stamina >= baseDashCost)
        {
            dashing = .25f;
            stamina -= baseDashCost;
        }

        pPos = transform.position;

        pDashButton = dashButton;

        //vect.y = body.velocity.y;

        //vect += velocity;
        controller.Move(velocity * Time.deltaTime);
        //transform.position += new Vector3(0, velocity.y * Time.deltaTime);
        velocity += Physics.gravity * Time.deltaTime;
    }
    void updateCam()
    {
        if (avatar.GetCurrentAnimatorStateInfo(0).IsName("Recoil")) return;
        if (avatar.GetCurrentAnimatorStateInfo(0).IsName("Die")) return;
        Vector3 vectVert = new Vector3(pmouseY - Input.mousePosition.y, 0);
        Vector3 vectHorz = new Vector3(0, pmouseX - Input.mousePosition.x);

        cam.transform.localEulerAngles += vectVert * camSpeed * 9;
        Vector3 newVect = cam.transform.localEulerAngles;

        if (newVect.x > 45 && newVect.x < 180) cam.transform.localEulerAngles = new Vector3 (45, newVect.y, newVect.z);
        else if (newVect.x < 360-45 && newVect.x > 180) cam.transform.localEulerAngles = new Vector3 (360-45, newVect.y, newVect.z);
        transform.eulerAngles += vectHorz * -camSpeed * 16;

        if (vectHorz.y != 0) CmdUpdateRotate(transform.eulerAngles);

        pmouseX = Input.mousePosition.x;
        pmouseY = Input.mousePosition.y;
    }
    [Command]
    void CmdUpdateRotate(Vector3 newRot)
    {
        transform.eulerAngles = newRot;
    }
    void updateInput()
    {
        if (avatar.GetCurrentAnimatorStateInfo(0).IsName("Die")) return;
        if (avatar.GetCurrentAnimatorStateInfo(0).IsName("Recoil")) return;
        bool shouldUpdate = false;
        if (vertAxis != Input.GetAxis("Vertical") || horzAxis != Input.GetAxis("Horizontal") ||
            dashButton != Input.GetButton("Dash") || jumpButton != Input.GetButton("Jump")) shouldUpdate = true;

        vertAxis = Input.GetAxis("Vertical");
        horzAxis = Input.GetAxis("Horizontal");

        dashButton = Input.GetButton("Dash");
        jumpButton = Input.GetButton("Jump");

        if (shouldUpdate) CmdInput(vertAxis, horzAxis, dashButton, jumpButton);
    }
    [Command]
    void CmdInput(float vertInput, float horzInput, bool dashInput, bool jumpInput)
    {
        this.vertAxis = vertInput;
        horzAxis = horzInput;
        dashButton = dashInput;
        jumpButton = jumpInput;
    }
    //[ClientRpc]
    public void changeAnim(string anim)
    {
        if (!avatar || isTransitioning || avatar.IsInTransition(0) || avatar.GetCurrentAnimatorStateInfo(0).IsName(anim)) return;
        if (avatar.GetCurrentAnimatorStateInfo(0).IsName("Die")) return;
        if (avatar.GetCurrentAnimatorStateInfo(0).IsName("Recoil") && anim != "Die") return;
        RpcChangeAnim(anim);
    }
    [ClientRpc]
    void RpcChangeAnim(string anim)
    {
        if (avatar.GetCurrentAnimatorStateInfo(0).IsName("Jump")) return;
        avatar.CrossFade(anim, .1f);
        isTransitioning = true;
        StartCoroutine(playAnim(anim));
    }
    IEnumerator playAnim(string anim)
    {
        yield return new WaitForSeconds(.1f);
        avatar.Play(anim);
        isTransitioning = false;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.layer == 8) grounded = true;
        else if (collision.collider.gameObject.layer == 9) transform.position = new Vector3(transform.position.x, 5, transform.position.z);
    }
    [ClientRpc]
    public void Rpcrestart()
    {
        avatar.Play("Idle");
        transform.position = new Vector3(0, 25, 0);
    }
    [ClientRpc]
    void RpcSetStamina(float stam)
    {
        if (!isLocalPlayer) return;
        PlayerStartup.stamina = stam;
    }
}
