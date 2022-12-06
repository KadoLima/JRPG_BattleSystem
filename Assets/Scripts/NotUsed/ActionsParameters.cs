using UnityEditor;

[CustomEditor(typeof(CombatAction))]
public class ActionsParameters : Editor
{
    #region serializedProperties
    SerializedProperty actionType;
    SerializedProperty actionName;
    SerializedProperty description;
    SerializedProperty mpCost;
    SerializedProperty goToTarget;
    SerializedProperty isAreaOfEffect;
    SerializedProperty damageMultiplier;
    SerializedProperty animationCycle;
    #endregion

    private void OnEnable()
    {
        actionType = serializedObject.FindProperty("actionType");
        actionName = serializedObject.FindProperty("actionName");
        description = serializedObject.FindProperty("description");
        mpCost = serializedObject.FindProperty("mpCost");
        goToTarget = serializedObject.FindProperty("goToTarget");
        isAreaOfEffect = serializedObject.FindProperty("isAreaOfEffect");
        damageMultiplier = serializedObject.FindProperty("damageMultiplier");
        animationCycle = serializedObject.FindProperty("animationCycle");
    }



    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
