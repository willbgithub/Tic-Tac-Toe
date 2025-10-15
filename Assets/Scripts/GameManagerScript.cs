using NUnit.Framework;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

public class GameManagerScript : MonoBehaviour
{
    public bool isXTurn = true;
    public List<GameObject> buttons;
    public TMP_Text turnLabel;
    public GameObject restartButton;
    public TMP_Text xWinsLabel;
    public TMP_Text oWinsLabel;

    private bool joever = false;
    private int xWins = 0;
    private int oWins = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        restart();
    }

    public void OnCellClicked(int cellIndex)
    {
        GameObject button = buttons[cellIndex];
        TMP_Text label = button.GetComponentInChildren<TMP_Text>();
        if (joever)
        {
            print("the game is over");
            return;
        }
        if (label.text != "")
        {
            print("you clicked on an occupied tile");
            return;
        }
        if (isXTurn)
        {
            label.text = "X";
            turnLabel.text = "Turn: O";
        }
        else
        {
            label.text = "O";
            turnLabel.text = "Turn: X";
        }
        if (checkForWin(button))
        {
            joever = true;
            if (isXTurn)
            {
                turnLabel.text = "X wins";
                xWins++;
                xWinsLabel.text = "X - " + xWins;
            }
            else
            {
                turnLabel.text = "O wins";
                oWins++;
                oWinsLabel.text = oWins + " - O";
            }
            restartButton.SetActive(true);
        }
        isXTurn = !isXTurn;

    }

    public bool checkRow(int row)
    {
        string letter = (isXTurn) ? "X" : "O";
        for (int i = 0;  i < buttons.Count; i++)
        {
            GameObject button = buttons[i];
            ButtonScript buttonScript = button.GetComponent<ButtonScript>();
            if (buttonScript.row != row)
                continue;
            TMP_Text label = button.GetComponentInChildren<TMP_Text>();
            if (!label.text.Equals(letter))
            {
                return false;
            }
        }
        return true;
    }

    public bool checkColumn(int column)
    {
        string letter = (isXTurn) ? "X" : "O";
        for (int i = 0; i < buttons.Count; i++)
        {
            GameObject button = buttons[i];
            ButtonScript buttonScript = button.GetComponent<ButtonScript>();
            if (buttonScript.column != column)
                continue;
            TMP_Text label = button.GetComponentInChildren<TMP_Text>();
            if (!label.text.Equals(letter))
            {
                return false;
            }
        }
        return true;
    }

    public bool checkSlash(int slash)
    {
        string letter = (isXTurn) ? "X" : "O";
        for (int i = 0; i < buttons.Count; i++)
        {
            GameObject button = buttons[i];
            ButtonScript buttonScript = button.GetComponent<ButtonScript>();
            if (slash == 0 && !buttonScript.inNegativeSlash || slash == 1 && !buttonScript.inPositiveSlash)
                continue;
            TMP_Text label = button.GetComponentInChildren<TMP_Text>();
            if (!label.text.Equals(letter))
            {
                return false;
            }                    
        }
        return true;
    }

    public bool checkForWin(GameObject button)
    {
        bool win = false;
        ButtonScript buttonScript = button.GetComponent<ButtonScript>();
        win = checkRow(buttonScript.row) || checkColumn(buttonScript.column);
        if (buttonScript.inNegativeSlash)
            win = win || checkSlash(0);
        if (buttonScript.inPositiveSlash)
            win = win || checkSlash(1);
        return win;
    }

    public void restart()
    {
        for (int i=0; i<buttons.Count; i++)
        {
            GameObject button = buttons[i];
            TMP_Text label = button.GetComponentInChildren<TMP_Text>();
            label.text = "";
            if (isXTurn)
                turnLabel.text = "Turn: X";
            else
                turnLabel.text = "Turn: O";
            restartButton.SetActive(false);
            joever = false;
        }
    }
}
