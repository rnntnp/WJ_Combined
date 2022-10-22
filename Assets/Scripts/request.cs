using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class Chat
{
    public string chat;
}
public class Quest
{
    public string keywords;
}
public class request : MonoBehaviour
{
    public TMP_Text questText;
    public TMP_Text catQuestText;
    public Button QuestBtn;
    public bool ishate = false;
    public GameObject quest_panel;
    private string locationQuest;
    private string actionQuest;
    public TMP_Text currentLocation;
    private bool questIsGenerated;
    public Animator catCanJump;
    //public string[] actionKeyword = { "jump", "fly", "walk", "go", "play", "find" };
    private string[] locationKeyword = { "교실", "복도", "쥐라기 파크", "도서관", "갤러리", "옥상", "태양계" };
    public GameObject KeywordUI;
    void Start()
    {
        questIsGenerated = false;
        //QuestBtn = GameObject.FindGameObjectWithTag("NewQuestBtn").GetComponent<Button>();
        //QuestBtn.onClick.AddListener(NewQuest);
        // quest_panel.SetActive(false);
    }
    int curloc;
    int temploc;
    public void NewQuest()
    {
        temploc = curloc;
        while (curloc == temploc)
        {
            curloc = Random.Range(0, locationKeyword.Length);
        }
        locationQuest = locationKeyword[curloc];
        GameManager.Instance.curQuest = curloc;
        //actionQuest = actionKeyword[Random.Range(0, actionKeyword.Length)];
        Debug.Log("뽑힌 Keywords : " + locationQuest);
        //NOTE 여기 일단 지ㅓㅗㅁ
        StartCoroutine(UploadKeyword(locationQuest));
        questIsGenerated = true;
        GameManager.Instance.idleAgent.GetComponent<SaySomething>().say("퀘스트를 기다리는중이야!");
    }

    public void NewQuest(int locId)
    {
        locationQuest = GameManager.Instance.placeNames[locId];
        GameManager.Instance.curQuest = curloc;
        //actionQuest = actionKeyword[Random.Range(0, actionKeyword.Length)];
        Debug.Log("뽑힌 Keywords : " + locationQuest);
        //NOTE 여기 일단 지ㅓㅗㅁ
        StartCoroutine(UploadKeyword(locationQuest));
        questIsGenerated = true;
        GameManager.Instance.idleAgent.GetComponent<SaySomething>().say("퀘스트를 기다리는중이야!");
    }
    public void CheckQuestSuccess()
    {
        if (questIsGenerated == true && locationQuest == currentLocation.text)
        {
            GameManager.Instance.curQuest = -1;
            catCanJump.Play("jump");
            GameManager.Instance.idleAgent.GetComponent<SaySomething>().say("퀘스트 완료!");
            StartCoroutine(GameManager.Instance.idleAgent.GetComponent<SaySomething>().petFadeOut());

            questIsGenerated = false;
        }
    }
    // IEnumerator getRequest(string uri)
    // {
    //     UnityWebRequest uwr = UnityWebRequest.Get(uri);
    //     yield return uwr.SendWebRequest();
    //     if (uwr.result == UnityWebRequest.Result.ConnectionError)
    //     {
    //         Debug.Log("Error While Sending: " + uwr.error);
    //     }
    //     else
    //     {
    //         Debug.Log("Received: " + uwr.downloadHandler.text);
    //     }
    // }

    public IEnumerator Upload(System.Action<bool> callback, string line)
    {
        Chat body = new Chat();
        body.chat = line;
        string bodyData = JsonUtility.ToJson(body);
        //Debug.Log(bodyData);
        // var postData = System.Text.Encoding.UTF8.GetBytes(bodyData);
        var req = new UnityWebRequest("http://43.201.36.173:5000/prediction", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(bodyData);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else
        {
            Debug.Log(line + req.downloadHandler.text);
            string res = req.downloadHandler.text;
            if (res.Contains("1"))
            {
                ishate = true;
            }
            else
            {
                ishate = false;
            }
            callback(ishate);
        }
    }
    public TMP_Text keyWords;
    public IEnumerator UploadKeyword(string line)
    {
        //line -> 보낼 데이터
        Quest body = new Quest();
        body.keywords = line;
        string bodyData = JsonUtility.ToJson(body);
        //Debug.Log(bodyData);
        // var postData = System.Text.Encoding.UTF8.GetBytes(bodyData);
        var req = new UnityWebRequest("http://3.39.9.191:5000/quest_gen", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(bodyData);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else
        {
            string res = req.downloadHandler.text;
            questText.text = res + "\n(Keywords: " + line.Replace("*", " ") + ")";
            Debug.Log(res);
            string quest = res.Split('@')[0];
            string keywords = res.Split('@')[1];

            foreach (string word in keywords.Split('*'))
            {
                if (word.Length > 1 && word != "0" && word != "None")
                {
                    if (quest.IndexOf(word) >= 0)
                    {
                        quest = quest.Insert(quest.IndexOf(word), "<color=\"red\">");
                        quest = quest.Insert(quest.IndexOf(word) + word.Length, "</color>");
                    }

                }
            }

            GameManager.Instance.idleAgent.GetComponent<SaySomething>().say(quest);
            KeywordUI.SetActive(true);
            keyWords.text = "키워드: " + keywords.Replace("*", ",").Replace("0", "").Replace("None", "").Replace("n개", "").Replace("n번", "").Replace("n회", "");
            GameManager.Instance.idleAgent.Invoke("showOkNoQuest", 0.5f);
        }


    }
}
