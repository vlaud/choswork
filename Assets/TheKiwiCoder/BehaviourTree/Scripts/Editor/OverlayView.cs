using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace TheKiwiCoder
{
    [UxmlElement]
    public partial class OverlayView : VisualElement
    {

        public System.Action<BehaviourTree> OnTreeSelected;

        Button openButton;
        Button createButton;
        DropdownField assetSelector;
        TextField treeNameField;
        TextField locationPathField;

        public void Show()
        {
            // Hidden in UIBuilder while editing..
            style.visibility = Visibility.Visible;

            // Configure fields
            openButton = this.Q<Button>("OpenButton");
            assetSelector = this.Q<DropdownField>();
            createButton = this.Q<Button>("CreateButton");
            treeNameField = this.Q<TextField>("TreeName");
            locationPathField = this.Q<TextField>("LocationPath");

            // Configure asset selection dropdown menu
            var behaviourTrees = EditorUtility.GetAssetPaths<BehaviourTree>();
            assetSelector.choices = new List<string>();
            behaviourTrees.ForEach(treePath =>
            {
                assetSelector.choices.Add(ToMenuFormat(treePath));
            });

            // Configure open asset button
            openButton.clicked -= OnOpenAsset;
            openButton.clicked += OnOpenAsset;

            // Configure create asset button
            createButton.clicked -= OnCreateAsset;
            createButton.clicked += OnCreateAsset;
        }

        public void Hide()
        {
            style.visibility = Visibility.Hidden;
        }

        public string ToMenuFormat(string one)
        {
            // Using the slash creates submenus...
            return one.Replace("/", "|");
        }

        public string ToAssetFormat(string one)
        {
            // Using the slash creates submenus...
            return one.Replace("|", "/");
        }

        void OnOpenAsset()
        {
            string path = ToAssetFormat(assetSelector.text);
            BehaviourTree tree = AssetDatabase.LoadAssetAtPath<BehaviourTree>(path);
            if (tree)
            {
                TreeSelected(tree);
                style.visibility = Visibility.Hidden;
            }
        }

        void OnCreateAsset()
        {
            BehaviourTree tree = EditorUtility.CreateNewTree(treeNameField.text, locationPathField.text);
            if (tree)
            {
                TreeSelected(tree);
                style.visibility = Visibility.Hidden;
            }
        }

        void TreeSelected(BehaviourTree tree)
        {
            OnTreeSelected.Invoke(tree);
        }
    }
}
