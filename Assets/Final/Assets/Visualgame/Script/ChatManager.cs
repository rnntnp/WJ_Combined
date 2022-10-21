using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using TMPro;
using System;
using System.IO;
using System.Net;

public class ChatManager : MonoBehaviour
{
    public GameObject YellowArea, WhiteArea;
    public RectTransform ContentRect;
    public Scrollbar scrollbar;

    AreaScript LastArea;
    Baby_talk babyscript;

    [System.Serializable]
    public class SetTextToSpeech
    {
        public SetInput input;
        public SetVoice voice;
        public SetAudioConfig audioConfig;

    }

    [System.Serializable]
    public class SetInput
    {
        public string text;
    }

    [System.Serializable]
    public class SetVoice
    {
        public string languageCode;
        public string name;
        public string ssmlGender;
    }

    [System.Serializable]
    public class SetAudioConfig
    {
        public string audioEncoding;
        public float speakingRate;
        public int pitch;
        public int volumeGainDb;

    }

    [System.Serializable]
    public class GetContent
    {
        public string audioContent;
    }

    private string apiURL = "https://texttospeech.googleapis.com/v1/text:synthesize?key=AIzaSyCp0J9eObLvBXTgiHToRcM03x1IEgMnNfE";
    //새로운 객체 생성
    SetTextToSpeech tts = new SetTextToSpeech();
    void Start()
    {
        //babyscript=GameObject.Find("GameManager").transform.GetComponent<Baby_talk>(); //babyscript로 가서 baby목소리 가져오려는 작업
        //StartCoroutine(GetBaby("http://13.124.152.251:5000/baby"));
        Init();
    }



    // Update is called once per frame
    void Update()
    {

    }




    void Init()
    {


        SetAudioConfig sa = new SetAudioConfig();
        sa.audioEncoding = "LINEAR16";
        sa.speakingRate = 0.8f;
        sa.pitch = 0;
        sa.volumeGainDb = 0;
        tts.audioConfig = sa;

    }

    int question_count = 0;

    public void Chat(bool isSend, string text, string user)
    {
        if (text.Trim() == "") return;
        bool isBottom = scrollbar.value <= 0.00001f;

        AreaScript Area = Instantiate(isSend ? YellowArea : WhiteArea).GetComponent<AreaScript>();
        Area.transform.SetParent(ContentRect.transform, false);
        Area.BoxRect.sizeDelta = new Vector2(300, Area.BoxRect.sizeDelta.y);
        Area.TextRect.GetComponent<TMP_Text>().text = text;



        //AI가 말할때(음성도 같이나오게)
        if (!isSend)
        {
            Debug.Log("AI: " + text);
            SetInput si = new SetInput();
            SetVoice sv = new SetVoice();
            sv.languageCode = "en-US";
            sv.name = "en-US-Standard-C";
            sv.ssmlGender = "FEMALE";
            tts.voice = sv;
            si.text = text;
            tts.input = si;



            Debug.Log("여자목소리");
            CreateAudio();
        }
        //사용자가 질문할 때
        else
        {
            //StartCoroutine(GetBaby("http://13.124.152.251:5000/baby",text));
            //Debug.Log("babyscript로 넘김");
            //babyscript.baby_audio(text);
            SetInput si = new SetInput();
            si.text = text;
            SetVoice sv = new SetVoice();
            sv.languageCode = "en-US";
            sv.name = "en-US-Standard-D";
            sv.ssmlGender = "MALE";
            tts.voice = sv;
            tts.input = si;
            Debug.Log("남자목소리");
            CreateAudio();
            //사용자의 질문횟수 조사-> 질문횟수 2번이상이면 정답고를수있음

        }
        Fit(Area.BoxRect);


        // 두 줄 이상이면 크기를 줄여가면서, 한 줄이 아래로 내려가면 바로 전 크기를 대입 
        float X = Area.TextRect.sizeDelta.x + 42;
        float Y = Area.TextRect.sizeDelta.y;
        if (Y > 49)
        {
            for (int i = 0; i < 200; i++)
            {
                Area.BoxRect.sizeDelta = new Vector2(X - i * 2, Area.BoxRect.sizeDelta.y);
                Fit(Area.BoxRect);

                if (Y != Area.TextRect.sizeDelta.y) { Area.BoxRect.sizeDelta = new Vector2(X - (i * 2) + 2, Y); break; }
            }
        }

        else Area.BoxRect.sizeDelta = new Vector2(X, Y);

        Fit(Area.BoxRect);
        Fit(Area.AreaRect);
        Fit(ContentRect);
        LastArea = Area;

        if (!isSend && !isBottom) return;
        Invoke("ScrollDelay", 0.03f);



    }
    AudioSource audioSource;

    private void CreateAudio()
    {
        //HttpWebRequest 요청 후 반환된 string 값을 저장
        var str = TTS_Post(tts);

        //string 값을 JsonUtility를 사용하여 JSON 데이터 형식으로 다시 저장
        GetContent info = JsonUtility.FromJson<GetContent>(str);

        //JSON 형식으로 저장된 값을 byte array로 반환
        var bytes = Convert.FromBase64String(info.audioContent);
        //byte array를 float array로 변환
        var f = ConvertByteToFloat(bytes);
        //create audio clip
        AudioClip audioClip = AudioClip.Create("audioContent", f.Length, 1, 44100, false);

        audioClip.SetData(f, 0);


        audioSource = transform.GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private static float[] ConvertByteToFloat(byte[] array)
    {
        float[] floatArr = new float[array.Length / 2];
        for (int i = 0; i < floatArr.Length; i++)
        {
            floatArr[i] = BitConverter.ToInt16(array, i * 2) / 32768.0f;
        }
        return floatArr;
    }

    public string TTS_Post(object data)
    {
        //JsonUtility 사용.string을 보내기 위한 byte로 변환
        string str = JsonUtility.ToJson(data);
        var bytes = System.Text.Encoding.UTF8.GetBytes(str);

        //요청을 보낼 주소와 세팅
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiURL);
        request.Method = "POST";
        request.ContentType = "application/json";
        request.ContentLength = bytes.Length;

        //Stream 형식으로 데이터를 보냄 request
        try
        {

            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //StreamReader로 stream 데이터를 받음
            StreamReader reader = new StreamReader(response.GetResponseStream());
            //stream 데이터를 string로 변환
            string json = reader.ReadToEnd();

            return json;
        }
        catch (WebException e)
        {
            //log
            Debug.Log(e);
            return null;
        }

    }


    void Fit(RectTransform Rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
    void ScrollDelay() => scrollbar.value = 0;

}



