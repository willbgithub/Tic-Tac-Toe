using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.HID.HID;

public class GameManagerScript : MonoBehaviour
{

    public List<GameObject> buttons;
    public GameObject restartButton;
    public TMP_Text turnLabel;
    public TMP_Text xWinsLabel;
    public TMP_Text oWinsLabel;

    private char turn = 'X';
    private bool gameIsOver = false;
    private int xWins = 0;
    private int oWins = 0;

    private bool AI = true;
    private bool AIThinking = false;
    private double AITimer = 0;
    public double AISpeed = 0.4;
    

    public void Update()
    {
        if (AIThinking)
        {
            AITimer += Time.deltaTime;
            if (AITimer > AISpeed)
            {
                AIThinking = false;
                OnButtonClicked(ChooseButton());
                restartButton.GetComponent<UnityEngine.UI.Button>().enabled = true;
                GetImage(restartButton).color = Color.white;
            }
        }
    }

    private TMP_Text GetLabel(GameObject button)
    {
        return button.GetComponentInChildren<TMP_Text>();
    }
    private ButtonScript GetScript(GameObject button)
    {
        return button.GetComponent<ButtonScript>();
    }

    private UnityEngine.UI.Image GetImage(GameObject button)
    {
        return button.GetComponent<UnityEngine.UI.Image>();
    }

    public void OnButtonClicked(GameObject button)
    {
        TMP_Text buttonLabel = GetLabel(button);

        // If the button is occupied or the game is over, return early.
        if (buttonLabel.text != "" || gameIsOver || AIThinking)
            return;

        // Adds the letter to the square
        buttonLabel.text = turn.ToString();

        // Checks for ending move
        if (IsWinningMove(button))
        {
            gameIsOver = true;
            if (turn == 'X')
            {
                xWins++;
                xWinsLabel.text = "X - " + xWins;
                turnLabel.text = "X wins!";
            }
            else
            {
                oWins++;
                oWinsLabel.text = oWins + " - O";
                turnLabel.text = "O wins!";
            }
            for (int i = 0; i<3; i++)
            {
                if (CheckRow(i))
                    GlowRow(i);
                if (CheckColumn(i))
                    GlowColumn(i);
            }
            if (CheckSlash('\\'))
                GlowSlash('\\');
            if (CheckSlash('/'))
                GlowSlash('/');
            return;
        }

        // Checks for stalemate (all tiles occupied, no winner)
        if (Stalemate())
        {
            gameIsOver = true;
            xWins++;
            xWinsLabel.text = "X - " + xWins;
            oWins++;
            oWinsLabel.text = oWins + " - O";
            turnLabel.text = "Stalemate!";
            return;
        }

        if (AI && turn == 'X')
        {
            turn = 'O';
            ActivateAI();
        }
        else
            NextTurn();
    }

    private void NextTurn()
    {
        if (turn == 'X')
            turn = 'O';
        else
            turn = 'X';
        turnLabel.text = "Turn: " + turn.ToString();
    }

    private bool IsWinningMove(GameObject button)
    {
        ButtonScript buttonScript = GetScript(button);
        return CheckRow(buttonScript.row) || CheckColumn(buttonScript.column) || CheckSlash('B');
    }

    private bool CheckRow(int row)
    {
        for (int i=0; i < buttons.Count; i++)
        {
            TMP_Text buttonLabel = GetLabel(buttons[i]);
            ButtonScript buttonScript = GetScript(buttons[i]);
            if (buttonScript.row == row && buttonLabel.text != turn.ToString())
            {
                return false;
            }
        }
        return true;
    }
    private bool CheckColumn(int column)
    {
        for (int i=0; i < buttons.Count; i++)
        {
            TMP_Text buttonLabel = GetLabel(buttons[i]);
            ButtonScript buttonScript = GetScript(buttons[i]);
            if (buttonScript.column == column && buttonLabel.text != turn.ToString())
            {
                return false;
            }
        }
        return true;
    }

    // slash can be '\', '/', or 'B' for both
    private bool CheckSlash(char slash)
    {
        bool negativeSlash = false, positiveSlash = false;
        if (slash == '\\' || slash == 'B')
        {
            negativeSlash = true;
            for (int i=0; i < buttons.Count; i++)
            {
                TMP_Text buttonLabel = GetLabel(buttons[i]);
                ButtonScript buttonScript = GetScript(buttons[i]);
                if (buttonScript.inNegativeSlash && buttonLabel.text != turn.ToString())
                {
                    negativeSlash = false;
                    break;
                }
            }
        }
        if (slash == '/' || slash == 'B')
        {
            positiveSlash = true;
            for (int i=0; i < buttons.Count; i++)
            {
                TMP_Text buttonLabel = GetLabel(buttons[i]);
                ButtonScript buttonScript = GetScript(buttons[i]);
                if (buttonScript.inPositiveSlash && buttonLabel.text != turn.ToString())
                {
                    positiveSlash = false;
                    break;
                }
            }
        }
        return negativeSlash || positiveSlash;
    }

    private void GlowRow(int row)
    {
        for (int i =0; i < buttons.Count; i++)
        {
            GameObject button = buttons[i];
            ButtonScript buttonScript = GetScript(buttons[i]);
            if (buttonScript.row == row)
                GetImage(button).color = Color.softYellow;
        }
    }
    private void GlowColumn(int column)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            GameObject button = buttons[i];
            ButtonScript buttonScript = GetScript(buttons[i]);
            if (buttonScript.column == column)
                GetImage(button).color = Color.softYellow;
        }
    }
    private void GlowSlash(char slash)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            GameObject button = buttons[i];
            ButtonScript buttonScript = GetScript(buttons[i]);
            if (buttonScript.inNegativeSlash && slash != '/')
                GetImage(button).color = Color.softYellow;
            if (buttonScript.inPositiveSlash && slash != '\\')
                GetImage(button).color = Color.softYellow;
        }
    }

    private bool Stalemate()
    {
        for (int i=0; i<buttons.Count;i++)
        {
            TMP_Text buttonLabel = GetLabel(buttons[i]);
            if (buttonLabel.text == "")
                return false;
        }
        return true;
    }

    public void Restart()
    {
        if (AIThinking)
            return;
        for (int i = 0; i < buttons.Count; i++)
        {
            GameObject button = buttons[i];
            TMP_Text buttonLabel = GetLabel(button);
            buttonLabel.text = "";
            GetImage(button).color = Color.white;
        }
        gameIsOver = false;
        if (turn == 'X' && AI)
        {
            turn = 'O';
            ActivateAI();
        }
        else
            NextTurn();
    }

    private void ActivateAI()
    {
        turnLabel.text = "Turn: O";
        restartButton.GetComponent<UnityEngine.UI.Button>().enabled = false;
        GetImage(restartButton).color = Color.grey;
        AITimer = 0f;
        AIThinking = true;
    }

    // Algorithm used by the AI to choose a button to click on.
    private GameObject ChooseButton()
    {
        List<GameObject> potentialButtons = new List<GameObject>();
        for (int i = 0; i < buttons.Count; i++)
        {
            GameObject button = buttons[i];
            TMP_Text buttonLabel = GetLabel(button);
            if (buttonLabel.text != "")
                continue;
            potentialButtons.Add(button);
        }
        int choice = UnityEngine.Random.Range(0, potentialButtons.Count);
        return potentialButtons[choice];
    }

    public void ToggleAI()
    {
        AI = !AI;
    }
}
