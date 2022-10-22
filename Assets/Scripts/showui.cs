using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showui : MonoBehaviour
{
    public GameObject ButtonCanv;
    public GameObject UdangCanv;
    public GameObject GuardBot;

    public GameObject sphere;
    Animation anim;
    List<string> animArray;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.curGroup = GetComponent<ConvGroup>();
            ButtonCanv.transform.rotation = Quaternion.LookRotation(ButtonCanv.transform.position - other.transform.GetComponentInChildren<Camera>().transform.position);
            // GuardBot.transform.position = new Vector3(other.transform.position.x,other.transform.position.y+1.5f,other.transform.position.z);
            // GuardBot.transform.rotation = Quaternion.LookRotation(other.transform.GetComponentInChildren<Camera>().transform.position-GuardBot.transform.position);
            // anim = GuardBot.GetComponent<Animation>();
            // animArray = new List<string>();
            // AnimationArray();
            // anim.Stop("Default");
            // anim.Play("Angry");
            //anim.wrapMode = WrapMode.Once;
            ButtonCanv.SetActive(true);
            if (!sphere.activeSelf)
            {
                sphere.SetActive(true);
            }
        }
    }
    public void AnimationArray() 
    { 
        // foreach (AnimationState state in anim) 
        // {
        //     animArray.Add(state.name); 
        // }
    }


    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ButtonCanv.SetActive(false);
            // anim.Stop("Angry");
            // anim.Play("Default");
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
