using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("main")]
    public bool Work = true;
    public GameObject[] scenePref;
    public Transform _robot;
    public PythonParser _parser;

    [Header("Targets")] 
    public Vector3 targetRobotPos;


    private bool targetComplete = false;
    
    [Header("targetsComplete")]
    
    
    [Space(20)]
    [Header("LearnPanel")] 
    public float timeToStartPanel;
    public Transform learnPanel;
    public GameObject showLearnPanelButton;

    public float startPanelPos;
    public float endPanelPos;
    public float startScalePanel;
    public float endScalePanel;
    public float timePanelStart;


    [Header("LearnText")]
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI pagesText;


    private GameObject scene;
    private SceneData sData;

    private byte pageNow;

    void Start()
    {
        if (!Work)
            return;

        if (dataBetweenScenes.currentLevel < scenePref.Length)
            scene = Instantiate(scenePref[dataBetweenScenes.currentLevel]);

        sData = scene.transform.GetComponent<SceneData>();

        _robot.position = sData.robotPosition;
        
        SetLearnText();
        StartCoroutine(StartPanel());
    }

    void Update()
    {
        CheckTargets();
        
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            HideLearnPanel();
    }

    private void CheckTargets()
    {
        if (_robot.transform.position == targetRobotPos && _parser.codeDone)
        {
            targetComplete = true;
            sData.bulbBlink.levelComplete = true;
        }
    }

    
    
    
    public void UpdatePage(bool next)
    {
        if((pageNow + 1) < sData.learnText.Length && next)
            pageNow++;
        else if (pageNow > 0 && !next)
            pageNow--;
        
        SetLearnText();
    }
    

    private void SetLearnText()
    {
        mainText.text = sData.learnText[pageNow];

        pagesText.text = 
            (pageNow + 1).ToString("D2") + 
            "/" +
            sData.learnText.Length.ToString("D2");
    }
    
    IEnumerator StartPanel()
    {
        learnPanel.localScale = Vector3.one * startScalePanel;
        learnPanel.localPosition = new Vector3(startPanelPos,
            learnPanel.localPosition.y,
            learnPanel.localPosition.z
        );

        yield return new WaitForSeconds(timeToStartPanel);
        StartLearnPanel();
    }
    private void StartLearnPanel()
    {
        showLearnPanelButton.SetActive(false);
        
        Sequence seqPanel = DOTween.Sequence();

        seqPanel.Join(learnPanel.DOLocalMoveX(endPanelPos, timePanelStart));
        seqPanel.Join(learnPanel.DOScale(endScalePanel, timePanelStart));
    }

    private void HideLearnPanel()
    {
        Sequence seqPanel = DOTween.Sequence();

        seqPanel.Join(learnPanel.DOLocalMoveX(startPanelPos, timePanelStart));
        seqPanel.Join(learnPanel.DOScale(startScalePanel, timePanelStart))
            .OnComplete(() =>
            {
                showLearnPanelButton.SetActive(true);
            });
    }
    public void ShowLearnPanel()
    {
        StartLearnPanel();
    }

}
