using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TILTrial 
{
   //Trial Components:
    public int preyPosX;
    public int preyPosY;
    public int threatState;
    public int predatorState; // attacking or not
    public float attackingSteps;
    public int stimType; //toQuestion, noQuestion, 

    //data to record
    public float trialOnsetTime;
    public float trialOffsetTime;
    public string playerState;  //captured, free
    public PositionHandler endPosition; 
    public List<PositionHandler> playerPos; 
    public List<PositionHandler> predatorPos; 
    public List<PositionHandler> flashingPos; 
    public List<JoystickInputHandler> firstJoystickInput;
    public List<JoystickInputHandler> finalMovementVector;
    public List<PresentationEvent> presentationEvents;
    public List<LightEvent> lightEvent;
    public float captureTime;
    public float attackInitiatedTime;
    public float playerQuestionResponse;


}
