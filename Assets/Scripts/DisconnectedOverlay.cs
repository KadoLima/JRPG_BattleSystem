using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisconnectedOverlay : MonoBehaviour
{
    [SerializeField] Button okButton;
    [SerializeField] GameObject screen;

    private void OnEnable()
    {
        GameManager.OnPlayerDisconnectedFromGame += ShowOverlay;
       
    }

    private void OnDisable()
    {
        GameManager.OnPlayerDisconnectedFromGame -= ShowOverlay;
    }

    void Start()
    {
        screen.SetActive(false);
        okButton.onClick.AddListener(GameManager.instance.LoadMenuScene);
    }

    public void ShowOverlay()
    {
        screen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(okButton.gameObject);
    }
}
