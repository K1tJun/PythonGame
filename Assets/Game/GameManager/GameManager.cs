using System.Collections;
using System;
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


    [Header("RobotInfo")]
    public Vector2 moveLimitX;
    public Vector2 moveLimitZ;
    
    [Header("Targets")]
    public float restartRobTime = 0.5f;
    
    private bool targetComplete = false;
    public bool restartingRob = false;



    [Header("targetsComplete")] 
    public RectTransform enterMenu;

    public float waitToEnterMenu;
    [Space]
    public float timeEnterMenuAnim;
    public float endMenuPos;
    public float durationMenuPosShake;
    public float timeMenuShake;
    public float endMenuScale;
    [Space]
    public TextMeshProUGUI numTime;
    public TextMeshProUGUI numCommands;
    public TextMeshProUGUI numErrors;

    
    
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

        moveLimitX = sData.limitX;
        moveLimitZ = sData.limitZ;
    }

    void Update()
    {
        CheckTargets();
        
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            HideLearnPanel();
    }

    private void CheckTargets()
    {
        if (_robot.transform.position == sData.robotTargetPosition && _parser.codeDone && !targetComplete )
        {
            targetComplete = true;
            sData.bulbBlink.levelComplete = true;

            HideLearnPanel();

            StartCoroutine(StartEnterMenu());
        }
        else if ((_robot.transform.position != sData.robotPosition 
                  || _robot.transform.eulerAngles != new Vector3(0, 0, 0)) && 
                 _parser.codeDone &&
                 _robot.transform.position != sData.robotTargetPosition && 
                 !restartingRob)
        {
            ResRob();
        }
    }

    public void ResRob()
    {
        StartCoroutine(RestartRobot());
    }


    IEnumerator RestartRobot()
    {
        restartingRob = true;

        yield return new WaitForSeconds(1f);

        _robot.transform.DOScale(0f, restartRobTime);
        yield return new WaitForSeconds(restartRobTime);
        
        _robot.transform.position = sData.robotPosition;
        _robot.transform.eulerAngles = new Vector3(0, 0, 0);

        _robot.transform.DOScale(1f, restartRobTime);
        restartingRob = false;
    }

    
    
    //Enter Menu

    IEnumerator StartEnterMenu()
    {
        yield return new WaitForSeconds(waitToEnterMenu);
        ShowEnterMenu();
    }
    private void ShowEnterMenu()
    {
        SetEnterMenuText();
        
        Sequence menuSeq = DOTween.Sequence();

        menuSeq.Join(enterMenu.DOLocalMoveY(endMenuPos + durationMenuPosShake, timeEnterMenuAnim));
        menuSeq.Join(enterMenu.DOScale(endMenuScale, timeEnterMenuAnim));
        menuSeq.Append(enterMenu.DOLocalMoveY(-durationMenuPosShake, timeMenuShake));
    }
    private void SetEnterMenuText()
    {
        numTime.text = TimeSpan.FromSeconds(Time.time).ToString(@"m\:ss");
        numCommands.text = _parser.codeLength.ToString();
        numErrors.text = _parser.errorsCount.ToString();
    }
    
    
    
    
    
    
    //Learn Panel
    
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
