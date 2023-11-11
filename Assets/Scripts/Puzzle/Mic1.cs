using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mic1 : MonoBehaviour
{
    private AudioClip aud;
    int sampleRate = 44100;
    private float[] samples;
    public float rmsValue;
    public float modulate;
    public int resultValue;
    public int cutValue;

    public Button recordButton;

    private readonly int duration = 5;

    public GameObject myLight;
    private UnityEngine.Rendering.Universal.Light2D myLightScript;
    [SerializeField] private float rangeIncrease = 0.005f;
    [SerializeField] private float minRange = 0f;
    [SerializeField] private float maxRangeOuter = 4f;
    [SerializeField] private float maxRangeInner = 2f;
    [SerializeField] private Dropdown dropdown;

    void Start()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
        #else
        foreach (var device in Microphone.devices)
        {
            dropdown.options.Add(new Dropdown.OptionData(device));
        }

        recordButton.onClick.AddListener(StartRecording);
        dropdown.onValueChanged.AddListener(ChangeMicrophone);
            
        var index = PlayerPrefs.GetInt("user-mic-device-index");
        dropdown.SetValueWithoutNotify(index);
        #endif

        samples = new float[sampleRate];
        myLightScript = myLight.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
    }

    private void ChangeMicrophone(int index)
    {
        PlayerPrefs.SetInt("user-mic-device-index", index);
    }

    private void StartRecording()
    {
        //isRecording = true;
        recordButton.enabled = false;

        var index = PlayerPrefs.GetInt("user-mic-device-index");
        
        #if !UNITY_WEBGL
        aud = Microphone.Start(dropdown.options[index].text, false, duration, 44100);
        #endif
    }

    public void Update()
    {
        aud.GetData(samples,0); //녹음 데이터를 실수형 배열로 가져오기 (-1f ~ 1f), 시작위치 -> 0으로 두기
        float sum = 0;

        //데이터의 평균값 구하기
        //실수형 배열이기 때문에 그냥 평균 구하면 0에 가까워지므로 제곱 평균 제곱근 rms로 구하기
        //배열의 값 제곱한 후 더해주고 배열의 크기만큼 나눠주기
        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }
        //Mathf.Sqrt 이용하여 제곱근 구하기
        rmsValue = Mathf.Sqrt(sum / samples.Length);

        //그냥 사용하면 소수점으로 값이 나오기 때문에 큰 수 곱하기
        //Clamp 사용해서 0~100사이의 숫자만 나오도록 설정하고 RoundToInt사용해서 소수점 더해주기
        rmsValue = rmsValue * modulate;
        rmsValue = Mathf.Clamp(rmsValue, 0, 100);
        resultValue = Mathf.RoundToInt(rmsValue);

        //오버되는 결과값 잘라주기
        if(resultValue < cutValue)
            resultValue = 0;
        
        if(resultValue > 70)
            Debug.Log("GameOver");
        
        if(myLightScript != null)
        {
            if(resultValue > 30)
            {
                myLightScript.pointLightInnerRadius = Mathf.Clamp(myLightScript.pointLightInnerRadius + rangeIncrease, minRange, maxRangeInner);
                myLightScript.pointLightOuterRadius = Mathf.Clamp(myLightScript.pointLightOuterRadius + rangeIncrease, minRange, maxRangeOuter);
            }
            else
            {
                myLightScript.pointLightInnerRadius = Mathf.Clamp(myLightScript.pointLightInnerRadius - rangeIncrease, minRange, maxRangeInner);
                myLightScript.pointLightOuterRadius = Mathf.Clamp(myLightScript.pointLightOuterRadius - rangeIncrease, minRange, maxRangeOuter);
            }
        }
    }
}