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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCellClicked(int cellIndex)
    {
        GameObject button = buttons[cellIndex];
        TMP_Text label = button.GetComponentInChildren<TMP_Text>();
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
            turnLabel.text = "";
            if (isXTurn)
            {
                print("X wins");
            }
            else
            {
                print("O wins");
            }
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
}
