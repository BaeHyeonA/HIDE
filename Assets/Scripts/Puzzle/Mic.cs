using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.SceneManagement;

public class Mic : MonoBehaviour
{
    private AudioClip aud;
    int sampleRate = 44100;
    private float[] samples;
    public float rmsValue;
    public float modulate;
    public int resultValue;
    public int cutValue;

    public Slider slider;

    public Button recordButton;
    private bool micCheck = false;
    private int index;

    public GameObject myLight;
    private UnityEngine.Rendering.Universal.Light2D myLightScript;
    [SerializeField] private float rangeIncrease = 0.005f;
    [SerializeField] private float minRange = 0f;
    [SerializeField] private float maxRangeOuter = 4f;
    [SerializeField] private float maxRangeInner = 2f;
    [SerializeField] private Dropdown dropdown;

    void Start()
    {
        foreach (var device in Microphone.devices)
        {
            dropdown.options.Add(new Dropdown.OptionData(device));
        }

        index = PlayerPrefs.GetInt("user-mic-device-index");

        recordButton.onClick.AddListener(StartRecording);
        dropdown.onValueChanged.AddListener(ChangeMicrophone);

        dropdown.SetValueWithoutNotify(index);

        samples = new float[sampleRate];
        myLightScript = myLight.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        slider.value = resultValue;
    }

    private void ChangeMicrophone(int index)
    {
        PlayerPrefs.SetInt("user-mic-device-index", index);
    }

    private void StartRecording()
    {
        index = PlayerPrefs.GetInt("user-mic-device-index");

        aud = Microphone.Start(dropdown.options[index].text.ToString(), true, 1, sampleRate);

        if (dropdown.options[index].text != null) micCheck = true;
    }

    public void Update()
    {
        if (micCheck)
        {
            if (aud == null)
            {
                micCheck = false;
                return;
            }

            if (slider.gameObject.activeSelf == false)
                slider.gameObject.SetActive(true);

            aud.GetData(samples, 0); //녹음 데이터를 실수형 배열로 가져오기 (-1f ~ 1f), 시작위치 -> 0으로 두기
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

            slider.value = resultValue;

            //오버되는 결과값 잘라주기
            if (resultValue < cutValue)
                resultValue = 0;


            if (resultValue > 70)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            if (myLightScript != null)
            {
                if (resultValue > 40)
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
}