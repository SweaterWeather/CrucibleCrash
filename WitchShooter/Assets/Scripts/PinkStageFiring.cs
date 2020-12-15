using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PinkStageFiring : NetworkBehaviour {

    public List<BulletMovement> quills;
    private PlayerDamage owner;
    public BulletMovement prefab;
	
    public void startup(PlayerDamage shooter)
    {
        owner = shooter;
    }
    
	public void fire (Vector3 target) {
		foreach(BulletMovement bullet in quills)
        {
            //bullet.cloneMe(target, owner);
            BulletMovement newBullet = Instantiate(prefab);
            newBullet.setTarget(bullet.transform.position, target, owner);
            NetworkServer.Spawn(newBullet.gameObject);
        }
        Destroy(this.gameObject);
	}
    [ServerCallback]
    private void Update()
    {
        transform.position = owner.transform.position;
        transform.rotation = owner.transform.rotation;
        if (quills.Count <= 0) Destroy(this.gameObject);
    }
}
