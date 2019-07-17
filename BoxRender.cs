using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxRender : MonoBehaviour
{
    Renderer rend;

    //gameobject position
    void Awake()
    {
        rend = this.GetComponent<Renderer>();

        rend.material.shader = Shader.Find("_Color");
        rend.material.SetColor("_Color", Color.grey);

        rend.material.shader = Shader.Find("Specular");
        rend.material.SetColor("_SpecColor", Color.grey);
    }

    //  This function changes the cube color
    public void SetColor(int num)
    {
        switch (num)
        {
            case 0:
                rend.material.shader = Shader.Find("_Color");               // 0 is white
                rend.material.SetColor("_Color", Color.white);
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.white);
                break;
            case 1:
                rend.material.shader = Shader.Find("_Color");               // 1 is blue
                rend.material.SetColor("_Color", Color.blue);
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.blue);
                break;
            case 2:                                                         // 2 is green
                rend.material.shader = Shader.Find("_Color");
                rend.material.SetColor("_Color", Color.green);
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.green);
                break;
            case 3:
                rend.material.shader = Shader.Find("_Color");               // 3 is red
                rend.material.SetColor("_Color", Color.red);
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.red);
                break;
            case 4:
                rend.material.shader = Shader.Find("_Color");               // 4 is purple
                rend.material.SetColor("_Color", Color.HSVToRGB(267, 74f, 59f)); 
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.HSVToRGB(267, 74f, 59f));
                break;
            case 5:
                rend.material.shader = Shader.Find("_Color");               // 5 is maroon
                rend.material.SetColor("_Color", Color.HSVToRGB(338f, 100f, 27f)); 
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.HSVToRGB(338f, 100f, 27f));
                break;
            case 6:
                rend.material.shader = Shader.Find("_Color");               // 6 is turquoise 
                rend.material.SetColor("_Color", Color.cyan);
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.cyan);
                break;
            case 7:
                rend.material.shader = Shader.Find("_Color");               // 7 is black 
                rend.material.SetColor("_Color", Color.black);
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.black);
                break;
            case 8:                                                         // 8 is grey
                rend.material.shader = Shader.Find("_Color");
                rend.material.SetColor("_Color", Color.grey);
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.grey);
                break;
            case 9: //end game                                              //mines are dark read but look black 
                rend.material.shader = Shader.Find("_Color");
                rend.material.SetColor("_Color", Color.HSVToRGB(10f, 100f, 54f));   
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.HSVToRGB(10f, 100f, 54f));
                break;
            case 10:
                //do nothing
                break;
            case 11: //start game                                           // start block is yellow
                rend.material.shader = Shader.Find("_Color");
                rend.material.SetColor("_Color", Color.yellow); 
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.yellow);
                break;

        }
    }
}