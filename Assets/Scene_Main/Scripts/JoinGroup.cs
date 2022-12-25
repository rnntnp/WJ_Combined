using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinGroup : MonoBehaviour
{
    public Transform pet;

    public Transform T;

    Vector3 removY(Vector3 vec)
    {
        return new Vector3(vec.x, 0, vec.z);
    }

    public void warn()
    {
        GameManager.Instance.idleAgent.say();
    }
    public void join()
    {
        Transform t = GameManager.Instance.curGroup.transform.GetChild(0);
        Transform inGroupUser1 = GameManager.Instance.curGroup.transform.GetChild(0).GetChild(0);
        Transform inGroupUser2 = GameManager.Instance.curGroup.transform.GetChild(1).GetChild(0);
        Transform inGroupUser3 = GameManager.Instance.curGroup.transform.GetChild(2).GetChild(0);
        if (!GameManager.Instance.curGroup.isbad)
        {
            // GameManager.Instance.idleAgent.endObst();
            Physics.IgnoreCollision(pet.GetComponent<Collider>(), t.parent.GetComponent<Collider>(), true);
            GameManager.Instance.idleAgent.enterGroup();
        }
        GameManager.Instance.ingroup = true;
        transform.SetParent(t.parent);
        transform.rotation = Quaternion.LookRotation(removY(-transform.position + t.position));
        Physics.IgnoreCollision(GetComponent<Collider>(), t.parent.GetComponent<Collider>(), true);
        GetComponent<Rigidbody>().AddForce(transform.forward * 3000, ForceMode.Impulse);
        //t.parent.GetComponent<ConvGroup>().hideSphere();
        t.parent.GetComponent<ConvGroup>().join();
        inGroupUser1.gameObject.layer = 5;
        inGroupUser2.gameObject.layer = 5;
        inGroupUser3.gameObject.layer = 5;

        // Physics.IgnoreCollision(GetComponent<Collider>(), t.parent.GetComponent<Collider>(), false);
    }

    public void sep()
    {
        if (GameManager.Instance.idleAgent.state == IdleAgent.States.say)
            GameManager.Instance.idleAgent.endSay();
        transform.SetParent(GameManager.Instance.transform);
        GameManager.Instance.ingroup = false;
        GameManager.Instance.hatePercentUI.SetActive(false);
    }

}
