using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.InputSystem;
using System.Linq;

public class StartMenuUIController : MonoBehaviour
{
    [Serializable]
    public class Content
    {
        public Transform contentContainer;
        public Button[] contentButtons;
        public TMP_InputField contentInputField;
    }


    [SerializeField] Content[] contents;
    [Space(10)]
    [Header("Texts")]
    [SerializeField] TextMeshProUGUI gameTitle;
    [SerializeField] TextMeshProUGUI errorMessage;
    [SerializeField] TextMeshProUGUI connectingSign;
    [SerializeField] GameObject pressToStart;
    [SerializeField] CanvasGroup createJoinLobbyContent;
    [SerializeField] DustStormTransition dustStorm;
    [SerializeField] Image fader;

    public static Action OnPlayerChoseCoop;

    Content currentContent;

    EventSystem eventSystem;

    private void Awake()
    {
        FadeFromBlack();

        connectingSign.DOFade(0, 0);
        connectingSign.gameObject.SetActive(false);
        pressToStart.SetActive(true);
        createJoinLobbyContent.DOFade(0, 0);
        createJoinLobbyContent.gameObject.SetActive(false);

        eventSystem = EventSystem.current;
    }

    private void OnEnable()
    {
        MultiplayerManager.OnConnectedToLobby += ShowContent;
    }

    private void OnDisable()
    {
        MultiplayerManager.OnConnectedToLobby -= ShowContent;
    }

    void Start()
    {
        AssignButtons();
        HideContents();
        HideErrorMessage();
        PressToStart_Effect();
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (pressToStart.activeSelf)
            {
                pressToStart.SetActive(false);
                ShowContent(0);
            }
        }  

        if (Input.GetKeyDown(KeyCode.U) && currentContent == contents[4])
        {
            Debug.LogWarning("Force starting game. Players connected = " + MultiplayerManager.instance.ConnectedPlayersCount());
            FadeToDustAndLoadScene(2);
        }
    }

    public void OnMenus_Back(InputValue value)
    {
        if (currentContent != contents[0] && currentContent != contents[4])
        {
            //Debug.LogWarning("yes");
            MultiplayerManager.ForceDisconnect();
            HideContents();
            HideErrorMessage();

            Sequence _sequence = DOTween.Sequence();
            _sequence.AppendInterval(.2f);
            _sequence.OnComplete(() => PressToStart_Effect());
        }
    }

    void FadeFromBlack()
    {
        fader.gameObject.SetActive(true);
        Sequence _fadeFromBlack = DOTween.Sequence();
        _fadeFromBlack.Append(fader.DOFade(1f, 0f));
        _fadeFromBlack.AppendInterval(.25f);
        _fadeFromBlack.Append(fader.DOFade(0f, 2f)).OnComplete(() => fader.gameObject.SetActive(false));
    }

    void HideContents(bool hideGameTitle = false)
    {
        foreach (var c in contents)
            c.contentContainer.gameObject.SetActive(false);
    }

    void ShowContent(int index)
    {
        StartCoroutine(ShowContent_Coroutine(index));
    }

    IEnumerator ShowContent_Coroutine(int index)
    {
        float _delay = 0;

        if (index == 1)
        {
            connectingSign.DOFade(0, .2f).OnComplete(() => connectingSign.gameObject.SetActive(false));
            _delay = .2f;
        }

        yield return new WaitForSeconds(_delay);
        DisableContentButtons(index);
        HideButtonArrowsAndDeselectHighlightedButtons(index);
        yield return new WaitForSeconds(.05f);
        ActivateContentContainer(index);
        SelectFirstButton(index);
        SetCurrentContent(index);
        StartCoroutine(EnableButtonsDelayed(index));
    }

    void DisableContentButtons(int index)
    {
        if (contents[index].contentButtons.Length == 0)
            return;

        foreach (Button button in contents[index].contentButtons)
        {
            button.interactable = false;
        }
    }

    void ActivateContentContainer(int index)
    {
        foreach (Content c in contents)
        {
            c.contentContainer.gameObject.SetActive(c == contents[index]);
        }

        contents[index].contentContainer.GetComponent<CanvasGroup>().alpha = 1;
    }

    void SetCurrentContent(int index) => currentContent = contents[index];
    void SelectFirstButton(int index) 
    {
        if (contents[index].contentButtons.Length == 0) 
            return;

        eventSystem.SetSelectedGameObject(contents[index].contentButtons[0].gameObject);
    } 

    void HideButtonArrowsAndDeselectHighlightedButtons(int index)
    {
        foreach (Button button in contents[index].contentButtons.Skip(1))
        {
            //Debug.LogWarning(button.name);
            GameObject _buttonArrow = button.transform.GetChild(1).gameObject;
            _buttonArrow.SetActive(false);

            HighlightButton _highlightButton = button.GetComponent<HighlightButton>();
            if (_highlightButton != null)
                _highlightButton.Deselected();
            
        }
    }

    IEnumerator EnableButtonsDelayed(int index)
    {
        yield return new WaitForSeconds(.1f);
        foreach (Button b in contents[index].contentButtons)
        {
            b.interactable = true;
        }
    }

    void PressToStart_Effect()
    {
        pressToStart.SetActive(true);
        TextMeshProUGUI _pressToStartText = pressToStart.GetComponent<TextMeshProUGUI>();

        Sequence _pulsingEffect = DOTween.Sequence();
        _pulsingEffect.Append(_pressToStartText.DOFade(1, .75f));
        _pulsingEffect.Append(_pressToStartText.DOFade(0, .75f));
        _pulsingEffect.SetLoops(-1);
    }

    void AssignButtons()
    {
        contents[0].contentButtons[0].onClick.AddListener(delegate { FadeToDustAndLoadScene(1); });
        contents[0].contentButtons[1].onClick.AddListener(ShowCoopMenu);

        contents[1].contentButtons[0].onClick.AddListener(ShowCreateNewLobbyContent);
        contents[1].contentButtons[1].onClick.AddListener(ShowJoinLobbyContent);

        contents[2].contentButtons[0].onClick.AddListener(FinishCreatingLobby); 
        
        contents[3].contentButtons[0].onClick.AddListener(FinishJoiningLobby);

        //contents[4].contentButtons[0].onClick.AddListener(delegate { FadeToDustAndLoadScene(1); });

    }

    public void FadeToDustAndLoadScene(int sceneIndex)
    {
        gameTitle.DOFade(0, 0.5f);
        Sequence fadeAndLoadSequence = DOTween.Sequence();
        fadeAndLoadSequence.Append(currentContent.contentContainer.GetComponent<CanvasGroup>().DOFade(0, .5f));
        dustStorm.PlayParticles();
        fadeAndLoadSequence.AppendInterval(2f);
        fadeAndLoadSequence.OnComplete(() => SceneManager.LoadScene(sceneIndex));
    }

    void ShowCoopMenu()
    {
        Sequence _hideMainContent = DOTween.Sequence();
        _hideMainContent.Append(currentContent.contentContainer.GetComponent<CanvasGroup>().DOFade(0, .2f));
        _hideMainContent.OnComplete(() => currentContent.contentContainer.gameObject.SetActive(false));

        connectingSign.gameObject.SetActive(true);
        Sequence _changeToCoopMenu = DOTween.Sequence();
        _changeToCoopMenu.Append(connectingSign.DOFade(1, 0.2f));
        _changeToCoopMenu.OnComplete(() => MultiplayerManager.instance.InitializeConnectionToServer());
    }

    public void ShowCreateNewLobbyContent()
    {
        currentContent.contentContainer.gameObject.SetActive(false);
        contents[2].contentContainer.gameObject.SetActive(true);
        currentContent = contents[2];

        eventSystem.SetSelectedGameObject(contents[2].contentInputField.gameObject);
    }

    public void ShowJoinLobbyContent()
    {
        currentContent.contentContainer.gameObject.SetActive(false);
        contents[3].contentContainer.gameObject.SetActive(true);
        currentContent = contents[3];

        eventSystem.SetSelectedGameObject(contents[3].contentInputField.gameObject);
    }

    public void OnEndCreatingLobbyName()
    {
        eventSystem.SetSelectedGameObject(contents[2].contentButtons[0].gameObject);
    }

    public void OnEndJoinLobbyName()
    {
        eventSystem.SetSelectedGameObject(contents[3].contentButtons[0].gameObject);
    }

    public void FinishCreatingLobby()
    {
        if (string.IsNullOrEmpty(contents[2].contentInputField.text))
        {
            Debug.LogWarning("Input field is empty!");
            eventSystem.SetSelectedGameObject(contents[2].contentInputField.gameObject);
            ShowErrorMessage("Invalid name.");
            return;
        }

        MultiplayerManager.instance.CreateRoom(contents[2].contentInputField.text);
        StartCoroutine(WaitForPlayersAndLoadGameScene());
    }

    IEnumerator WaitForPlayersAndLoadGameScene()
    {
        ShowContent(4);
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() => MultiplayerManager.instance.ConnectedPlayersCount() > 1);
        yield return new WaitForSeconds(.5f);
        FadeToDustAndLoadScene(2);
    }

    public void FinishJoiningLobby()
    {
        if (string.IsNullOrEmpty(contents[3].contentInputField.text))
        {
            Debug.LogWarning("Input field is empty!");
            eventSystem.SetSelectedGameObject(contents[3].contentInputField.gameObject);
            ShowErrorMessage("Invalid name.");
            return;
        }

        MultiplayerManager.instance.JoinRoom(contents[3].contentInputField.text);
        StartCoroutine(WaitForPlayersAndLoadGameScene());
    }

    public void ShowErrorMessage(string text)
    {
        errorMessage.gameObject.SetActive(true);
        errorMessage.text = text;
    }

    public void HideErrorMessage() => errorMessage.gameObject.SetActive(false);

}
