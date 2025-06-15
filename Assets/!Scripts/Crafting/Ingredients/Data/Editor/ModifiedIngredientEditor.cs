using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ModifiedIngredient))]
public class ModifiedIngredientEditor : PropertyDrawer
{
    private SerializedProperty _ingredientProp;
    private Editor _ingredientEditor;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float h = EditorGUIUtility.singleLineHeight;

        var prop = property.FindPropertyRelative("BaseIngredient");
        if (prop != null)
        {
            EnsureEditor(prop);
            //h += EditorGUIUtility.singleLineHeight * 4.0f;
        }

        return h + EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var prop = property.FindPropertyRelative("BaseIngredient");

        Rect fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(fieldRect, prop, new GUIContent(property.displayName));

        if (prop.objectReferenceValue != null)
        {
            EnsureEditor(prop);

            float y = fieldRect.yMax + EditorGUIUtility.standardVerticalSpacing;
            Rect box = new Rect(position.x + 15, y, position.width - 15, position.height - (y - position.y));

            using (new EditorGUI.IndentLevelScope(1))
            {
                _ingredientEditor.OnInspectorGUI();
            }
        }

        EditorGUI.EndProperty();
    }

    void EnsureEditor(SerializedProperty property)
    {
        if (_ingredientEditor == null || _ingredientEditor.target != property.objectReferenceValue)
            _ingredientEditor = Editor.CreateEditor(property.objectReferenceValue);
    }
}
