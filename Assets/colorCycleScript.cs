using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

//thank you Obvious for helping me make this, and by that I mean telling everything I need to put in to make the module work

public class colorCycleScript : MonoBehaviour {

    public KMAudio Audio;
    public AudioClip[] sounds;
    public KMBombInfo BombInfo;
    public KMSelectable[] Buttons;
    public GameObject LED;
    public KMBombModule Module;

    private int Index = 1;
    private int TargetTime;
    private int TargetIndex;

    static int _moduleIdCounter = 1;
    int _moduleID = 0;

    void Awake()
    {
        _moduleID = _moduleIdCounter++;
        Buttons[0].OnInteract += delegate{Buttons[0].AddInteractionPunch(); Index = (Index + 7) % 8; UpdateColors(); Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, base.transform); ; return false; };
        Buttons[1].OnInteract += delegate{Buttons[1].AddInteractionPunch(); Index = (Index + 1) % 8; UpdateColors(); Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, base.transform); return false; };
        Buttons[2].OnInteract += delegate{Buttons[2].AddInteractionPunch(); Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, base.transform); SubmitCheck(); return false;}; 
         }

    // hi person that's in this code. these are the if statements lol

    void Start () {
        if (BombInfo.IsPortPresent(Port.Serial) && BombInfo.GetIndicators().Count() >= 2)
        {
            Debug.LogFormat("[Color-Cycle Button #{0}] Rule #1 is TRUE. Expected Color: BLUE, Expected Time: 5.", _moduleID);
            TargetIndex = 2;
            TargetTime = 5;
        }
        else if (BombInfo.GetBatteryCount() == 0)
        {
            Debug.LogFormat("[Color-Cycle Button #{0}] Rule #2 is TRUE. Expected Color: GREEN, Expected Time: 2.", _moduleID);
            TargetIndex = 1;
            TargetTime = 2;
        }
        else if (BombInfo.GetBatteryCount() == 5 && BombInfo.GetPortPlates().Where(x => x.Length == 0).Count() > 0)
        {
            Debug.LogFormat("[Color-Cycle Button #{0}] Rule #3 is TRUE. Expected Color: RED, Expected Time: 6.", _moduleID);
            TargetIndex = 0;
            TargetTime = 6;
        }
        else if (BombInfo.GetPortCount() == 0)
        {
            Debug.LogFormat("[Color-Cycle Button #{0}] Rule #4 is TRUE. Expected Color: WHITE, Expected Time: 4.", _moduleID);
            TargetIndex = 6;
            TargetTime = 4;
        }
        else if (BombInfo.GetSerialNumber().Contains('7') || BombInfo.GetSerialNumber().Contains('E')) 
        {
            Debug.LogFormat("[Color-Cycle Button #{0}] Rule #5 is TRUE. Expected Color: YELLOW, Expected Time: 1.", _moduleID);
            TargetIndex = 4;
            TargetTime = 1;
        }
        else if (BombInfo.IsIndicatorPresent("FRK"))
        {
            Debug.LogFormat("[Color-Cycle Button #{0}] Rule #6 is TRUE. Expected Color: BLACK, Expected Time: 3.", _moduleID);
            TargetIndex = 7;
            TargetTime = 3;
        }
        else if (BombInfo.IsIndicatorPresent("MSA"))
        {
            Debug.LogFormat("[Color-Cycle Button #{0}] Rule #7 is TRUE. Expected Color: CYAN, Expected Time: 5.", _moduleID);
            TargetIndex = 3;
            TargetTime = 5;
        }
        else if (BombInfo.GetIndicators().Count() == 0)
        {
            Debug.LogFormat("[Color-Cycle Button #{0}] Rule #8 is TRUE. Expected Color: MAGENTA, Expected Time: 4.", _moduleID);
            TargetIndex = 5;
            TargetTime = 4;
        }
        else if (BombInfo.GetBatteryCount() == 4 && BombInfo.GetBatteryHolderCount() == 2 && BombInfo.IsPortPresent(Port.Parallel))
        {
            Debug.LogFormat("[Color-Cycle Button #{0}] Rule #9 is TRUE. Expected Color: RED, Expected Time: 9.", _moduleID);
            TargetIndex = 0;
            TargetTime = 9;
        }
        else if (BombInfo.GetBatteryCount() == 2 && BombInfo.GetBatteryHolderCount() == 1)
            
        {
            Debug.LogFormat("[Color-Cycle Button #{0}] Rule #10 is TRUE. Expected Color: CYAN, Expected Time: 6.", _moduleID);
            TargetIndex = 3;
            TargetTime = 6;
        }
        else
        {
            Debug.LogFormat("[Color-Cycle Button #{0}] Rule #11 is TRUE. Expected Color: GREEN, Expected Time: 6.", _moduleID);
            TargetIndex = 1;
            TargetTime = 6;
        }
	}
    //how do i upload source code again


	// //hi person looking at this, now this is the thing that actually makes the "color" part make sense
	private void UpdateColors()
    {
        int[] Reds = {1, 0, 0, 0, 1, 1, 1, 0};
        int[] Greens = {0, 1, 0, 1, 1, 0, 1, 0};
        int[] Blues = {0, 0, 1, 1, 0, 1, 1, 0};
        LED.GetComponent<MeshRenderer>().material.color = new Color(Reds[Index], Greens[Index], Blues[Index]);

        //i have no idea how this works i just did the logging
    }
    private void SubmitCheck()
    {
        if (Index == TargetIndex && (TargetTime == -1 || BombInfo.GetFormattedTime().Contains(TargetTime.ToString()))){
            string[] colornames = { "RED", "GREEN", "BLUE", "CYAN", "YELLOW", "MAGENTA", "WHITE", "BLACK" };
            Module.HandlePass();
            Audio.PlaySoundAtTransform("Solved", transform);
            Debug.LogFormat("[Color-Cycle Button #{0}] You selected {1} as your color. That is CORRECT.", _moduleID, colornames[Index], colornames[TargetIndex]);
            Debug.LogFormat("[Color-Cycle Button #{0}] You submitted when the timer had a {1} in any position. That is CORRECT.", _moduleID, TargetTime);
            Debug.LogFormat("[Color-Cycle Button #{0}] Module Solved!", _moduleID);


        }
        else
        {
            Module.HandleStrike();
            Audio.PlaySoundAtTransform("Strike", transform);
            string[] colornames = { "RED", "GREEN", "BLUE", "CYAN", "YELLOW", "MAGENTA", "WHITE", "BLACK"};
            if (Index != TargetIndex)
            {
                Debug.LogFormat("[Color-Cycle Button #{0}] You selected {1} as your color, but I expected {2}. Strike!", _moduleID, colornames[Index], colornames[TargetIndex]);
            }
            else
            {
                Debug.LogFormat("[Color-Cycle Button #{0}] The timer did not contain a {1} in any position. Strike!", _moduleID, TargetTime);
            }
        }

     

    }

}



