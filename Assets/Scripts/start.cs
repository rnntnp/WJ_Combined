using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class start : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void func()
    {
        Debug.Log("��ư����");


    }
    public void SceneChange()
    {
        SceneManager.LoadScene("Game");
    }
}
