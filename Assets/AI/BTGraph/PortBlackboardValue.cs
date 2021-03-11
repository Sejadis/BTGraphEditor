using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AI.BTGraph
{
    public class PortBlackboardValue : VisualElement
    {
        public Label Label { get; }
        public ToolbarMenu Dropdown { get; }
        public object overrideValue => isSetToManual ? inputField.text : null;
        public string Key => Label.text.Replace("(", "").Replace(")", "");
        private HashSet<BlackboardField> blackboardFields = new HashSet<BlackboardField>();

        private BlackboardField currentBlackboardField;
        private FloatField inputField;
        private readonly bool allowsManual;
        private bool isSetToManual;

        public PortBlackboardValue(bool allowsManual, string overrideValue)
        {
            style.flexDirection = FlexDirection.Row;
            Label = new Label("(none)");
            inputField = new FloatField();
            Dropdown = new ToolbarMenu();
            this.allowsManual = allowsManual;

            Add(Label);
            Add(Dropdown);
            if (!string.IsNullOrEmpty(overrideValue) && Single.TryParse(overrideValue, out var result))
            {
                inputField.value = result;
                SetToManualInput();
            }

            UpdateValues();
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
            if (allowsManual)
            {
                Dropdown.menu.AppendAction("Set manually", action => { SetToManualInput(); });
            }

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

        private void SetToManualInput()
        {
            isSetToManual = true;
            currentBlackboardField = null;
            Remove(Label);
            Insert(0, inputField);
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
            if (isSetToManual)
            {
                Remove(inputField);
                Insert(0, Label);
            }

            currentBlackboardField = blackboardField;
            Label.text = $"({blackboardField.text})";
            isSetToManual = false;
        }
    }
}