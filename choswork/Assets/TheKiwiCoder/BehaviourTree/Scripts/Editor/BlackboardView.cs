using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace TheKiwiCoder {
    [UxmlElement]
    public partial class BlackboardView : VisualElement {

        public BlackboardView() {

        }

        internal void Bind(SerializedBehaviourTree serializer) {
            Clear();

            var blackboardProperty = serializer.Blackboard;

            blackboardProperty.isExpanded = true;

            // Property field
            PropertyField field = new PropertyField();
            field.BindProperty(blackboardProperty);
            Add(field);
        }
    }
}