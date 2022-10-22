using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public IdleAgent idleAgent;
    public Transform owner;
    public ConvGroup curGroup;
    public Transform cam;
    public request req;
    public bool ingroup;
    public GameObject okno;
    public GameObject oknoQuest;
    private static GameManager gm;
    public TMP_Text hateRateText;
    public GameObject hatePercentUI;
    public TMP_Text locationUI;
    public Transform[] destinations;
    public int curQuest = -1;
    public int starReward;

    public string[] placeNames = { "쥐라기 공원", "거북선", "게임기", "자동차", "교실", "도서관", "놀이터", "행성" };
    public static GameManager Instance
    {
        get
        {
            if (gm == null) Debug.LogError("Game Manager is null!");
            return gm;
        }
    }
    // Start is called before the first frame update
    private void Awake()
    {
        gm = this;
    }
}
