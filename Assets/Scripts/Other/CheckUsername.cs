using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckUsername : MonoBehaviour
{
    private bool launcherLogin = false;

    void Start()
    {
        if (Environment.GetCommandLineArgs().Length > 1)
        {
            if (!String.IsNullOrEmpty(Environment.GetCommandLineArgs()[1]))
            {
                launcherLogin = Environment.GetCommandLineArgs()[1].ToString() == "LoggedIn";
            }
        }
        if (!launcherLogin)
        {
            Application.Quit();
        }
    }
}
