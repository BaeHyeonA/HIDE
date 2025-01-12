using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DemianDiaryInteraction : MonoBehaviour
{
    public BookController bookController;
    public HelenDiaryInteraction1 helen1;
    public MiaDiaryInteraction mia;
    public TomDiaryInteraction1 tom1;
    public LucyDiaryInteraction lucy;
    public HelenDiaryInteraction2 helen2;
    public TomDiaryInteraction2 tom2;

    [SerializeField]
    Button nextButton;
    [SerializeField]
    Button previousButton;
    [SerializeField]
    Button closeButton;
    [SerializeField]
    Image bookImage;
    [SerializeField]
    Sprite bookTexture;

    public GameObject[] pages;

    public int currentPage;

    public TMP_Text DiaryText;
    public GameObject DialogPanel;

    public GameObject DemianDiary;

    public GameObject NextBtn;
    public GameObject PreviousBtn;
    public GameObject Closebtn;

    bool Demian = false;


    //독백 오브젝트
    public GameObject player;
    public GameObject textpanel;
    public GameObject characterText;
    public GameObject characterName;
    public GameObject characterImage;
    public GameObject mapChange;
    public GameObject mapChangePanel;
    bool read = false;
    bool readAgain = true; //다시 뜨는 거 막기 위해 사용
    public GameObject minigame;

    void Start()
    {
        UpdatePage();

        nextButton.onClick.AddListener(NextPage);
        previousButton.onClick.AddListener(PreviousPage);
        closeButton.onClick.AddListener(Close);

        NextBtn.SetActive(false);
        PreviousBtn.SetActive(false);
        Closebtn.SetActive(false);
    }

    void NextPage()
    {
        bookController.NextPage();
        currentPage = Mathf.Min(++currentPage, pages.Length - 1);
        StartCoroutine(UpdatePageDelayed());
    }

    void PreviousPage()
    {
        bookController.PreviousPage();
        currentPage = Mathf.Max(--currentPage, 0);
        StartCoroutine(UpdatePageDelayed());
    }

    void Close()
    {
        DemianDiary.SetActive(false);
        NextBtn.SetActive(false);
        PreviousBtn.SetActive(false);
        Closebtn.SetActive(false);
    }

    IEnumerator UpdatePageDelayed()
    {
        yield return new WaitForEndOfFrame();
        UpdatePage();
    }

    public void UpdatePage()
    {
        Array.ForEach(pages, c => { c.SetActive(false); });
        pages[currentPage].SetActive(true);
        nextButton.gameObject.SetActive(currentPage < pages.Length - 1);
        previousButton.gameObject.SetActive(currentPage > 0);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            DialogPanel.SetActive(true);
            DiaryText.text = "It is Demian's diary. Should I read it?";
            Demian = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            DialogPanel.SetActive(false);
            DemianDiary.SetActive(false);
            NextBtn.SetActive(false);
            PreviousBtn.SetActive(false);
            Closebtn.SetActive(false);
            Demian = false;

            if(read && readAgain)
            {
                this.GetComponent<AudioSource>().Play();
                StartCoroutine(ShowPanel());
                readAgain = false;
            }
        }
    }

    IEnumerator ShowPanel()
    {
        characterText.GetComponent<TypingEffect>()._dialog[0] = "I hear someone approaching. Is Tom still in the mansion? I need to hide quickly. I’ll pile up some items to conceal myself.";
        characterText.GetComponent<TMP_Text>().text = characterText.GetComponent<TypingEffect>()._dialog[0];
        textpanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        characterImage.SetActive(true);
        yield return new WaitForSeconds(1f);
        characterName.SetActive(true);
        yield return new WaitForSeconds(1f);
        characterText.SetActive(true);
        minigame.SetActive(true);
        mapChange.SetActive(false);
        mapChangePanel.SetActive(true);
    }

    public void ShowDemian()
    {
        if (Demian)
        {
            currentPage = 0;   //if player activate the diary, player can see fist page of the diary
            mia.currentPage = 0;
            helen1.currentPage = 0;
            tom1.currentPage = 0;
            lucy.currentPage = 0;
            helen2.currentPage = 0;
            tom2.currentPage = 0;
            UpdatePage();
            DemianDiary.SetActive(true);
            NextBtn.SetActive(true);
            Closebtn.SetActive(true);
            DialogPanel.SetActive(false);
            read = true;
        }
    }

    public void NoBtn()
    {
        DialogPanel.SetActive(false);
    }
}
