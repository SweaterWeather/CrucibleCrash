using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAnimator : MonoBehaviour {

    public Animator option1;
    public Animator option2;
    public Animator option3;
    public Animator option4;

    public void setAnim(int skin, string anim)
    {
        option1.Play(anim);
        option2.Play(anim);
        option3.Play(anim);
        option4.Play(anim);

        switch (skin)
        {
            case 0:
                Destroy(this.gameObject);
                break;
            case 1:
                option1.gameObject.SetActive(true);
                option2.gameObject.SetActive(false);
                option3.gameObject.SetActive(false);
                option4.gameObject.SetActive(false);
                break;
            case 2:
                option1.gameObject.SetActive(false);
                option2.gameObject.SetActive(true);
                option3.gameObject.SetActive(false);
                option4.gameObject.SetActive(false);
                break;
            case 3:
                option1.gameObject.SetActive(false);
                option2.gameObject.SetActive(false);
                option3.gameObject.SetActive(true);
                option4.gameObject.SetActive(false);
                break;
            case 4:
                option1.gameObject.SetActive(false);
                option2.gameObject.SetActive(false);
                option3.gameObject.SetActive(false);
                option4.gameObject.SetActive(true);
                break;
        }
    }
}
