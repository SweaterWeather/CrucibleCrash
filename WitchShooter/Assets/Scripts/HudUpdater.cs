using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudUpdater : MonoBehaviour {

    public Text text;
    public Image healthBar;
    public Image staminaBar;

	void Update () {
        Vector3 vect = healthBar.transform.localScale;

        vect.x = PlayerStartup.health / PlayerStartup.maxHealth;
        healthBar.transform.localScale = vect;

        vect = staminaBar.transform.localScale;

        vect.x = PlayerStartup.stamina / PlayerStartup.maxStamina;
        staminaBar.transform.localScale = vect;
	}
}
