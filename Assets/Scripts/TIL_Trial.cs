using System;
using System.Collections.Generic;

public class TIL_Trial
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
    public List<PlayerVector> chaserPositionList;
    public List<JoystickInput> joystickInputList;
    public int index;
    public float startTime;
    public float endTime;
    public float playerShowTimestamp;
    public float cageShowTimestamp;
    public float predShowTimestamp;
    public float playerMoveTimestamp;
    public float questionScreenTimestamp;
    public float playerQuestionInput;
    public float trialTime;
    public float playerInLavaTime;

    public TIL_Trial(bool predRender, bool cageRender, bool predChase, bool isGreen, bool EOB, bool showQuestionScreen, string questionText, float startX, float startY)
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
        this.chaserPositionList = null;
        this.joystickInputList = null;
        this.index = 0;
        this.startTime = -1;
        this.endTime = -1;
        this.playerShowTimestamp=-1;
        this.cageShowTimestamp=-1;
        this.predShowTimestamp=-1;
        this.playerMoveTimestamp=-1;
        this.questionScreenTimestamp=-1;
        this.playerShowTimestamp = -1;
        this.playerQuestionInput = 0;
        this.playerInLavaTime = 0;
    }

    public TIL_Trial(bool predRender, bool cageRender, bool predChase, bool isGreen, bool EOB, bool showQuestionScreen, float startX, float startY)
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
        this.chaserPositionList = null;
        this.joystickInputList = null;
        this.index = 0;
        this.startTime = -1;
        this.endTime = -1;
        this.playerShowTimestamp=-1;
        this.cageShowTimestamp=-1;
        this.predShowTimestamp=-1;
        this.playerMoveTimestamp=-1;
        this.questionScreenTimestamp=-1;
        this.playerQuestionInput = 0;
        this.playerInLavaTime = 0;
    }

    public override string ToString()
    {
        return $"PredRender: {predRender}, CageRender: {cageRender}, PredChase: {predChase}, IsGreen: {isGreen}, EOB: {EOB}, questionScreen: {showQuestionScreen}, startX: {startX}, startY: {startY}";
    }
}

