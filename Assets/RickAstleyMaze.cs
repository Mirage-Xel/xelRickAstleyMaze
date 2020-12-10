using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rnd = UnityEngine.Random;
using KModkit;

public class RickAstleyMaze : MonoBehaviour {
    public KMSelectable[] arrows;
    public KMSelectable play;
    public KMSelectable submit;
    string[][] maze = new string[][]{
        new string[] {"Keep Singing","Whenever You Need Somebody","This Old House","Empty Heart","Cry For Help","I’ll Never Let You Down"},
        new string[] {"Hopelessly","Better Together","Lights Out","The Love Has Gone","The Good Old Days","Hold Me In Your Arms"},
        new string[] {"When I Fall In Love","Every Corner","Dance Walk Like A Panther","Giving Up On Love","I Need The Light"},
        new string[] {"Beautiful Life","Angels On My Side","Never Gonna Give You Up","Don't Say Goodbye","Together Forever","Slipping Away"},
        new string[] {"Shivers","Try","Take Me To Your Heart","She Wants To Dance With Me","You Move Me","She Makes Me"},
        new string[] {"Pray With Me "," No More Looking For Love","It Would Take A Strong Strong Man","Rise Up","Last Night On Earth","Chance to Dance"}
    };
    string[][] mazeInfo = new string[][]
    {
        new string[] {"1110", "1101", "1001", "1010", "1100", "1011"},
        new string[] {"0100", "1010", "1100", "0010", "0110", "1110"},
        new string[] {"0111", "0110", "0110", "0101", "0001", "0010"},
        new string[] {"1110", "0101", "0010", "1101", "1010", "0111"},
        new string[] {"0100", "1001", "0001", "1010", "0110", "1110"},
        new string[] {"0011", "1001", "1011", "0101", "0001", "0011"},
    };
    int[] staringCoordinate = new int[2];
    int[] coordinate;
    string[] alphabet = new string[]{ "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
    string movementSequence;
    string direction = "ULRD";
    int startingTime;
    string[] MazeRows = new string[6];
    public KMBombModule module;
    public KMBombInfo bomb;
    public KMAudio sound;
    int moduleId;
    static int moduleIdCounter = 1;
    bool solved;
	// Use this for initialization
	void Awake () {
        moduleId = moduleIdCounter++;
        submit.OnInteract += PressSumbit();
        play.OnInteract += PressPlay();
        for (int i = 0; i < 4; i++) arrows[i].OnInteract += PressDirection(i);
        module.OnActivate += delegate { startingTime = (int)Math.Floor(bomb.GetTime() / 60); };
        PermuteMaze();
        for (int i = 0; i < 2; i++) staringCoordinate[i] = rnd.Range(0, 6);
        staringCoordinate.CopyTo(coordinate, 0);
	}
    void PermuteMaze()
    {
        for (int i = 0; i < 6; i++) ShiftRow(bomb.GetSerialNumberNumbers().Last() + 1, i, false);
        for (int i = 0; i < 6; i++) if (i % 2 == 0) ShiftRow(bomb.GetBatteryCount() + 5, i, true);
        for (int i = 0; i < 6; i++) if (i % 2 == 1) ShiftRow(bomb.GetSolvableModuleNames().Count() + 3, i, false);
        for (int i = 0; i < 6; i++) if (i % 2 == 1) ShiftRow(bomb.GetIndicators().Count(), i, true);
        if (bomb.IsIndicatorPresent("BOB"))for (int i = 0; i < 6; i++) if (i % 2 == 1) ShiftRow(3, i, true);
        for (int i = 0; i < 6; i++) ShiftRow(startingTime + 3, i, false);
        for (int i = 0; i < 6; i++) if (i % 2 == 0) ShiftRow(Array.IndexOf(alphabet, bomb.GetSerialNumberLetters().Last()), i, true);
        for (int i = 0; i < 6; i++) if (i % 2 == 1) ShiftRow(bomb.GetSolvableModuleNames().Count() + 3, i, false);
        for (int i = 0; i < 6; i++) if (i % 2 == 1) ShiftRow(bomb.GetIndicators().Count(), i, true);
    }
	// Update is called once per frame
	void ShiftRow (int amount, int index, bool axis) {
        for (int i = 0; i < (amount % 6); i++ ){
            for (int j = 0; j < 6; j++)
            {
                if (axis)
                {
                    string temp = maze[index][j];
                    maze[index][j] = maze[index][(j + 1) % 6];
                    maze[index][(j + 1) % 6] = temp;
                }
                else
                {
                    string temp = maze[j][index];
                    maze[j][index] = maze[(j + 1) % 6][index];
                    maze[(j + 1) % 6][index] = temp;
                }
            }
        }
	}

    KMSelectable.OnInteractHandler PressDirection(int index)
    {
        return delegate
        {
            if (!solved)
            {
                arrows[index].AddInteractionPunch();
                if (mazeInfo[coordinate[0]][coordinate[1]][index] == '0')
                {
                    switch (index)
                    {
                        case 0:
                            coordinate[0]++;
                            break;
                        case 1:
                            coordinate[1]--;
                            break;
                        case 2:
                            coordinate[1]++;
                            break;
                        case 3:
                            coordinate[0]--;
                            break;

                    }
                }
            }
            return false;
        };
    }

    KMSelectable.OnInteractHandler PressPlay()
    {
        return delegate
        {
            play.AddInteractionPunch();
            if (!solved)
            {

            }
            return false;
        };
    }
     
    KMSelectable.OnInteractHandler PressSumbit()
    {
        return delegate
        {
            submit.AddInteractionPunch();
            if (!solved)
            {
                if (maze[coordinate[0]][coordinate[1]] == "Never Gonna Give You Up") 
                {
                    sound.PlaySoundAtTransform("Never Gonna Give You Up", transform);
                    module.HandlePass();
                    solved = true;
                }
            }
            return false;
        };
    }
}
