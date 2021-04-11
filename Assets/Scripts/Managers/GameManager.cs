﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(APIController))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [SerializeField]
    private bool isDebug;

    public bool IsDebug { get { return Console.isDebug; }  private set { isDebug = value; Console.isDebug = isDebug; } }

    private StateManager stateManager;

    [SerializeField]
    float timeStatement = 2;

    private APIController apiController;
    public float GetTimeStatement { get { return timeStatement; } }

    private APINumbers currentNumber;

    [SerializeField]
    Text successCount;
    [SerializeField]
    Text faultCount;

    int intSuccesCount;
    int intFaultCount;

void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        Init();
    }

    void Init()
    {
        IsDebug = isDebug;

        apiController = GetComponent<APIController>();

        stateManager = StateManager.Instance;

        stateManager.RegisterState(State.STATEMENT,new StatementState());
        stateManager.RegisterState(State.ANSWER, new AnswerState());
        
        stateManager.OnStateChange += HandleOnStateChange;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitAPI());
    }

    // Update is called once per frame
    void Update()
    {
        stateManager.Update(Time.deltaTime);
    }

    public void HandleOnStateChange()
    {

    }

    public GameObject FindInScene(string TAG)
    {

        return GameObject.FindGameObjectWithTag(TAG);
    }

    public StateManager GetStateManager()
    {
        return stateManager;
    }

    public string GetRandomLabel()
    {
        currentNumber = apiController.GetRandomNumber();

        return currentNumber.label;
    }

    public int GetValue()
    {
        return currentNumber.value;
    }

    public int GetRandomValue()
    {
        int value = -1;
        do
        {
            value = apiController.GetRandomNumber().value;
        } while (value == currentNumber.value);
        
        return value;
    }

    IEnumerator WaitAPI()
    {
        yield return new WaitForSeconds(1f);

        stateManager.ChangeTo(State.STATEMENT);
    }

    public void OnClickCheckAnswer(Button btn)
    {
        Text text = btn.GetComponentInChildren<Text>();
        string value = text.text;
        
        if(value == GetValue()+"")
        {
            var colors = btn.colors;
            colors.selectedColor = Color.green;
            text.color = Color.green;
            intSuccesCount++;
            successCount.text = intSuccesCount+"";
        }
        else
        {
            text.color = Color.red;
            intFaultCount++;
            faultCount.text = intFaultCount + "";
        }

        Console.Log("Your answer is: "+value);
    }

}
