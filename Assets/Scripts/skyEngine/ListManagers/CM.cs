using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CM
{
    FadeScreenController _fadeController;
    WaitController _waitControllerl;

    private static CM instance = null;
    public static void NewGame()
    {
        if (CM.instance != null)
            CM.instance = null;

        CM.instance = new CM();

        GameObject parent = GameObject.Find("EventControllers");

        CM.instance._fadeController = parent.GetComponentInChildren<FadeScreenController>();
        CM.instance._waitControllerl = parent.GetComponentInChildren<WaitController>();

        /* Dialogue find
        parent = GameObject.Find("MainMenu");

        CM.instance._dialogueController = parent.GetComponentInChildren<DialogueController>();
        CM.instance._dialogueController.gameObject.SetActive(false);
        */
    }

    public static FadeScreenController FadeController
    {
        get { return CM.instance._fadeController; }
    }

    public static WaitController WaitController
    {
        get { return CM.instance._waitControllerl; }
    }
}