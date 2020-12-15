using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerDamage : NetworkBehaviour {

    public float maxHealth;
    [SyncVar]
    public float health;
    public float knockBackThreshold;

    public PlayerMovement avatar;

    public int baseDownTime;
    private float downTime = 0;
    private float iFrames = 0;

    private void Start()
    {
        health = maxHealth;
        RpcTakeDamage(health);
    }
    public void takeDamage(float damageTaken, PlayerDamage assailant)
    {
        if (iFrames > 0 || health <= 0) return;
        health -= damageTaken;
        RpcTakeDamage(health);
        if(health <= 0)
        {
            health = 0;
            RpcTakeDamage(health);
            avatar.changeAnim("Die");

            downTime = baseDownTime;
            ServerScore.updateScore(assailant);

            if (ServerScore.checkForWinner(5))
            {
                NetworkLobbyManager.singleton.ServerChangeScene("gameover");
            }
        }
        else if(damageTaken > knockBackThreshold)
        {
            avatar.changeAnim("Recoil");
        }
    }
    private void Update()
    {
        if(downTime > 0)
        {
            downTime -= Time.deltaTime;
            if(downTime <= 0)
            {
                health = maxHealth;
                RpcTakeDamage(health);

                //iFrames = baseDownTime;
                GetComponent<PlayerMovement>().Rpcrestart();
            }
        }
        if (iFrames > 0)
        {
            print("invincible");
            iFrames -= Time.deltaTime;
        }
    }
    [ClientRpc]
    void RpcTakeDamage(float newhealth)
    {
        if (!isLocalPlayer) return;
        PlayerStartup.health = newhealth;
    }
}
