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
        Color[] colors = new Color[11];
        colors[0] = Color.white;
        colors[1] = Color.blue;
        colors[2] = Color.green;
        colors[3] = Color.red;
        colors[4] = Color.HSVToRGB(267, 74f, 59f);      //purple
        colors[5] = Color.HSVToRGB(338f, 100f, 27f); //maroon
        colors[6] = Color.cyan;
        colors[7] = Color.black;
        colors[8] = Color.grey;
        colors[9] = Color.HSVToRGB(10f, 100f, 54f); //dark red
        colors[10] = Color.yellow;

        if ((rend != null) && (num <= 10))
        {
            rend.material.shader = Shader.Find("_Color");
            rend.material.SetColor("_Color", colors[num]);
            rend.material.shader = Shader.Find("Specular");
            rend.material.SetColor("_SpecColor", colors[num]);
        }
    }
}
