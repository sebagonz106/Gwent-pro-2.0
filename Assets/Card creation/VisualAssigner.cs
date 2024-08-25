using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class VisualAssigner
{
    Shader shader;
    string savingPath = "C:\\Users\\Public\\Documents\\1958 Files\\Cards";

    public VisualAssigner(Shader shader)
    {
        this.shader = shader;
    }
    public void AssignVisual(Card card, string imagePath, string infoPath, bool save)
    {
        Material m = new Material(shader);
        Sprite s = null;
        try
        {
            if (save) AssignNew(ref m, ref s, imagePath, infoPath, card.Name);
            else AssignPreLoaded(ref m, ref s, imagePath, infoPath, card.Name);

            card.AssignInfo(new VisualInfo(m, s));
        }
        catch { }
    }
    void AssignPreLoaded(ref Material m, ref Sprite s, string imagePath, string infoPath, string name)
    {
        m = AssetDatabase.LoadAssetAtPath<Material>(imagePath + "\\" + name + ".mat");
        s = AssetDatabase.LoadAssetAtPath<Sprite>(infoPath + "\\" + name + ".png");
    }
    void AssignNew(ref Material m, ref Sprite s, string imagePath, string infoPath, string name)
    {
        Sprite texture = GetSpriteAt(imagePath + "\\" + name);
        m.mainTexture = texture.texture;
        s = GetSpriteAt(infoPath + "\\" + name);

        AssetDatabase.CreateAsset(m, savingPath + "\\Main image\\" + name + ".mat");
        AssetDatabase.CreateAsset(s, savingPath + "\\Info\\" + name + ".png");
    }
    Sprite GetSpriteAt(string path)
    {
        string ext = ".png";
        if (File.Exists(path + ".jpg")) ext = ".jpg";
        else if (File.Exists(path + ".jpeg")) ext = ".jpeg";

        return AssetDatabase.LoadAssetAtPath<Sprite>(path + ext);
    }
}
