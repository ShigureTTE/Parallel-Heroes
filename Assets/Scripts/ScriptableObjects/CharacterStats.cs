using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Character", menuName = "Characters/New Character")]
public class CharacterStats : ScriptableObject {
    [Header("Character Information")]
    public string characterName;
    public Gender gender;
    public string role;
    public string weapon;

    [Header("Base Stats")]
    public int maximumHealth;
    public int maximumMP;
    public int attack;
    public int defense;
    public int resistance;
    public int skill;
    public int speed;

    [Header("Attacks")]
    public Attack normalAttack;
    public Attack[] spells = new Attack[2];
    public List<Attack> combos;
}

[System.Serializable]
public class Attack {
    public AttackType type;
    public string attackName;
    public Lane preferredLane;
    public int mPCost;
    public int basePower;
    public CharacterStats comboCharacter;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Attack), true)]
public class AttackDrawerUIE : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        var positionAdjust = 0f;

        EditorGUI.PropertyField(new Rect(position.x, position.y + positionAdjust, position.width, 16),
            property.FindPropertyRelative("type"));
        positionAdjust += 20f;

        EditorGUI.PropertyField(new Rect(position.x, position.y + positionAdjust, position.width, 16),
            property.FindPropertyRelative("basePower"));
        positionAdjust += 20f;

        EditorGUI.PropertyField(new Rect(position.x, position.y + positionAdjust, position.width, 16),
            property.FindPropertyRelative("preferredLane"));
        positionAdjust += 20f;

        SerializedProperty field = property.FindPropertyRelative("type");
        AttackType type = (AttackType)field.enumValueIndex;

        switch (type) {
            case AttackType.Normal:

                break;
            case AttackType.Spell:
                EditorGUI.PropertyField(new Rect(position.x, position.y + positionAdjust, position.width, 16),
                    property.FindPropertyRelative("attackName"));
                positionAdjust += 20f;
                EditorGUI.PropertyField(new Rect(position.x, position.y + positionAdjust, position.width, 16),
                    property.FindPropertyRelative("mPCost"));
                positionAdjust += 20f;
                break;
            case AttackType.Combo:
                EditorGUI.PropertyField(new Rect(position.x, position.y + positionAdjust, position.width, 16),
                    property.FindPropertyRelative("attackName"));
                positionAdjust += 20f;
                EditorGUI.PropertyField(new Rect(position.x, position.y + positionAdjust, position.width, 16),
                    property.FindPropertyRelative("mPCost"));
                positionAdjust += 20f;
                EditorGUI.PropertyField(new Rect(position.x, position.y + positionAdjust, position.width, 16),
                    property.FindPropertyRelative("comboCharacter"));
                positionAdjust += 20f;
                break;
            default:
                break;
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return base.GetPropertyHeight(property, label) + 100;
    }
}
#endif