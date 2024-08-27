using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class VisualAssigner
{
    Material material;
    string savingPath = "C:\\Users\\Public\\Documents\\1958 Files\\Cards";

    public VisualAssigner(Material material)
    {
        this.material = material;
    }

    public void AssignVisual(Card card, string imagePath, string infoPath, bool save)
    {
        Sprite mainImage = null;
        Sprite info = null;

        if (save)
        {
            mainImage = GetSpriteAt(imagePath, card.Name, true);
            info = GetSpriteAt(infoPath, card.Name, false);
        }
        else
        {
            mainImage = GetSpriteAt(savingPath + "\\Main Image\\", card.Name, true, false, ".gwi");
            info = GetSpriteAt(savingPath + "\\Info\\", card.Name, false, false, ".gwi");
        }

        if (!(mainImage is null || info is null)) card.AssignInfo(new VisualInfo(mainImage, info));
    }

    VisualInfo AssignMaterial (Texture2D tex, Sprite info)
    {
        Material m = new Material(material)
        {
            mainTexture = tex
        };
        return new VisualInfo(m, info);
    }

    Sprite GetSpriteAt(string pathWithoutName, string nameWithoutExtension, bool isMainImage, bool saveImage = true, string ext = "")
    {
        string path = pathWithoutName + "\\" + nameWithoutExtension;
        if (ext.Length==0)
        {
            ext = ".png";
            if (File.Exists(path + ".jpg")) ext = ".jpg";
            else if (File.Exists(path + ".jpeg")) ext = ".jpeg";
        }
        path += ext;

        FileStream fs = File.OpenRead(path);
        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes, 0, bytes.Length);
        fs.Close();

        Texture2D texture = new Texture2D(16, 16);
        texture.LoadImage(bytes);
        if (saveImage) SaveImage(bytes, isMainImage, nameWithoutExtension);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
    }

    void SaveImage(byte[] bytes, bool isMainImage, string name)
    {
        string path = savingPath;
        if (isMainImage) path += "\\Main Image\\";
        else path += "\\Info\\";
        path += name + ".gwi";

        File.WriteAllBytes(path, bytes);
    }
}
