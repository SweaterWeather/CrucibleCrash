using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletMovement : NetworkBehaviour {

    private Vector3 velocity;
    public float speed;
    public float lifeTime;
    public PlayerDamage shooter;
    public float power;
    [SyncVar]
    public bool fired = false;
    public BulletMovement clone;

	public void setTarget (Vector3 start, Vector3 target, PlayerDamage damage) {
        fired = true;
        shooter = damage;
        transform.position = start;
        transform.LookAt(target);
	}
	
	void Update () {
        if (!fired) return;
        transform.position += transform.forward * speed * Time.deltaTime;
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0) Destroy(this.gameObject);
	}
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (!fired) return;
        if (other.GetComponent<BulletMovement>()) return;
        if (other.GetComponent<PlayerDamage>())
        {
            PlayerDamage hit = other.GetComponent<PlayerDamage>();
            if (hit == shooter) return;
            else
            {
                hit.takeDamage(power, shooter);
                Destroy(this.gameObject);
            }
        }
        else Destroy(this.gameObject);
        //Destroy(other.gameObject);
    }
    /*public void cloneMe(Vector3 target, PlayerDamage damage)
    {
        print("new bullet!");
        BulletMovement bullet = Instantiate(clone);
        NetworkServer.Spawn(clone.gameObject);
        clone.setTarget(transform.position, target, damage);
        clone.fired = true;
        clone.shooter = damage;
        Destroy(this.gameObject);
    }*/
    [ClientRpc]
    public void RpcResize(float size)
    {
        transform.localScale = new Vector3(size, size, size);
    }
}
