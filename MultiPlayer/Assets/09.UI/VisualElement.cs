using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TankVisualElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<TankVisualElement, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlStringAttributeDescription m_panelName =
            new UxmlStringAttributeDescription { name = "tank-name", defaultValue = "" };

        private UxmlIntAttributeDescription m_panelIndex = new UxmlIntAttributeDescription
        { name = "tank-index", defaultValue = 0 };
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var ate = ve as TankVisualElement;

            ate.tankName = m_panelName.GetValueFromBag(bag, cc);
            ate.tankIndex = m_panelIndex.GetValueFromBag(bag, cc);
        }
    }
    public string tankName { get; set; }
    public int tankIndex { get; set; }

}
