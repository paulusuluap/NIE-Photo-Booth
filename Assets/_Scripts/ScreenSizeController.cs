using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSizeController : MonoBehaviour
{
    int index = 1;

    void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D))
        {
            //switching between windowed 1920x1080, windowed 1536x864, and fullsceen
            switch(index)
            {
                case 0:
                    Screen.SetResolution(1920, 1080, false);
                    index++;
                    break;
                case 1:
                    Screen.SetResolution(1536, 864, false);
                    index++;
                    break;
                case 2:
                    Screen.SetResolution(1920, 1080, true);
                    index = 0;
                    break;
            }
        
        }
    }
}
