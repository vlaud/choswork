using UnityEngine;
using System;
#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(CustomHeaderAttribute))]
    public class CustomHeaderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CustomHeaderAttribute tempHeader = attribute as CustomHeaderAttribute;
            float headerHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            Rect headerRect = new Rect(position.x, position.y, position.width, headerHeight);
            Rect propertyRect = new Rect(position.x, position.y + headerHeight, position.width, position.height - headerHeight);
            // 헤더 스타일 (굵게 + 한글 지원 + 줄바꿈)
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                //normal = { textColor = Color.red }, // 필요시 색상 변경
                wordWrap = true                     // 긴 한글도 줄바꿈
            };
            EditorGUI.LabelField(headerRect, tempHeader.header, headerStyle);

            // Draw the property field below the header with their real name
            EditorGUI.PropertyField(propertyRect, property, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float headerHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            float propertyHeight = EditorGUI.GetPropertyHeight(property, label, true);
            return headerHeight + propertyHeight;
        }
    }
}
#endif

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class CustomHeaderAttribute : PropertyAttribute
{
    //
    // Summary:
    //     The header text.
    public readonly string header;

    //
    // Summary:
    //     Add a header above some fields in the Inspector.
    //
    // Parameters:
    //   header:
    //     The header text.
    public CustomHeaderAttribute(string header)
        : base(applyToCollection: true)
    {
        this.header = header;
    }
}