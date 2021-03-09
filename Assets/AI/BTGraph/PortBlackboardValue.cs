using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

namespace AI.BTGraph.Editor
{
    public class PortBlackboardValue : VisualElement
    {
        public Label Label { get; }
        public ToolbarMenu Dropdown { get; }
        public string Key => Label.text.Replace("(", "").Replace(")", "");
        private HashSet<BlackboardField> blackboardFields = new HashSet<BlackboardField>();

        private BlackboardField currentBlackboardField;

        public PortBlackboardValue()
        {
            style.flexDirection = FlexDirection.Row;
            Label = new Label("(none)");
            Dropdown = new ToolbarMenu();
            Add(Label);
            Add(Dropdown);
        }

        private void ClearDropdown()
        {
            for (var i = Dropdown.menu.MenuItems().Count - 1; i >= 0; i--)
            {
                Dropdown.menu.RemoveItemAt(i);
            }
        }

        public void UpdateValues()
        {
            ClearDropdown();
            foreach (var blackboardField in blackboardFields)
            {
                Dropdown.menu.AppendAction(blackboardField.text,
                    action => { SetCurrentFieldAndUpdateVisuals(blackboardField); });
            }

            if (blackboardFields.Contains(currentBlackboardField))
            {
                Label.text = currentBlackboardField.text;
            }
            else
            {
                Label.text = "(none)";
                currentBlackboardField = null;
            }

            // var logString = "Keys found for Port " + port.portName + ":";
            // logString = bbValues.Aggregate(logString, (current, value) => current + (" " + value));
            //
            // Debug.Log(logString);
        }

        public void AddFieldReference(BlackboardField blackboardField)
        {
            blackboardFields.Add(blackboardField);
            UpdateValues();
        }

        public void RemoveFieldReference(BlackboardField blackboardField)
        {
            blackboardFields.Remove(blackboardField);
            UpdateValues();
        }

        public void SetCurrentFieldAndUpdateVisuals(BlackboardField blackboardField)
        {
            currentBlackboardField = blackboardField;
            Label.text = $"({blackboardField.text})";
        }
    }
}