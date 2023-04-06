using UnityEditor;
using UnityEngine;

//We connect the editor with the Weapon SO class
[CustomEditor(typeof(CharacterStats))]
//We need to extend the Editor
public class WeaponEditor : Editor
{
    //Here we grab a reference to our Weapon SO
    CharacterStats characterStats;

    private void OnEnable()
    {
        characterStats = target as CharacterStats;
    }

    //Here is the meat of the script
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (characterStats.characterSprite == null)
            return;

        Texture2D texture = AssetPreview.GetAssetPreview(characterStats.characterSprite);
        GUILayout.Label("", GUILayout.Height(200), GUILayout.Width(325));
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
    }
}
