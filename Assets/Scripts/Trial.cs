using System;
using System.Collections.Generic;

public class Trial
{
    public bool predRender;
    public bool cageRender;
    public bool predChase;
    public bool isGreen;
    public bool EOB;
    public bool showQuestionScreen;
    public string questionText;
    public float startX;
    public float startY;
    public List<PlayerVector> positionDataList;
    public List<JoystickInput> joystickInputList;
    public int index;
    public String startTime;
    public String endTime;
    public float playerQuestionInput;
    public float trialTime;
    public float playerInLavaTime;

    public Trial(bool predRender, bool cageRender, bool predChase, bool isGreen, bool EOB, bool showQuestionScreen, string questionText, float startX, float startY)
    {
        this.predRender = predRender;
        this.cageRender = cageRender;
        this.predChase = predChase;
        this.isGreen = isGreen;
        this.EOB = EOB;
        this.showQuestionScreen = showQuestionScreen;
        this.questionText = questionText;
        this.startX = startX;
        this.startY = startY;
        this.positionDataList = null;
        this.joystickInputList = null;
        this.index = 0;
        this.startTime = null;
        this.endTime = null;
        this.playerQuestionInput = 0;
        this.playerInLavaTime = 0;
    }

    public Trial(bool predRender, bool cageRender, bool predChase, bool isGreen, bool EOB, bool showQuestionScreen, float startX, float startY)
    {
        this.predRender = predRender;
        this.cageRender = cageRender;
        this.predChase = predChase;
        this.isGreen = isGreen;
        this.EOB = EOB;
        this.showQuestionScreen = showQuestionScreen;
        this.startX = startX;
        this.startY = startY;
        this.positionDataList = null;
        this.joystickInputList = null;
        this.index = 0;
        this.startTime = null;
        this.endTime = null;
        this.playerQuestionInput = 0;
        this.playerInLavaTime = 0;
    }

    public override string ToString()
    {
        return $"PredRender: {predRender}, CageRender: {cageRender}, PredChase: {predChase}, IsGreen: {isGreen}, EOB: {EOB}, questionScreen: {showQuestionScreen}, startX: {startX}, startY: {startY}";
    }
}

//  List<PlayerVector> positionDataList, int index, DateTime startTime, DateTime endTime, int playerQuestionInput
