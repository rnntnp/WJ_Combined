using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showSphere : MonoBehaviour
{
    public GameObject Sphere;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Sphere.SetActive(false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (GameManager.Instance.ingroup)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Transform inGroupUser1 = GameManager.Instance.curGroup.transform.GetChild(0).GetChild(0);
                Transform inGroupUser2 = GameManager.Instance.curGroup.transform.GetChild(1).GetChild(0);
                Transform inGroupUser3 = GameManager.Instance.curGroup.transform.GetChild(2).GetChild(0);
                Sphere.SetActive(true);
                Physics.IgnoreCollision(transform.parent.GetComponent<Collider>(), GameManager.Instance.owner.GetComponent<Collider>(), false);
                GameManager.Instance.owner.GetComponent<JoinGroup>().sep();
                inGroupUser1.gameObject.layer = 3;
                inGroupUser2.gameObject.layer = 3;
                inGroupUser3.gameObject.layer = 3;
            }
        }
        else
        {
            if (other.gameObject.CompareTag("agent"))
            {
                Physics.IgnoreCollision(transform.parent.GetComponent<Collider>(), GameManager.Instance.idleAgent.GetComponent<Collider>(), false);
            }
        }
    }
}
