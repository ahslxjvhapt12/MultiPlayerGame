using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TankUI
{
    public VisualElement root;
    private TankVisualElement _tankPanel;
    private VisualElement _tankBody;
    private VisualElement _tankTurret;
    private Label _tankName;

    public TankUI(VisualElement root, TankDataSO tankData)
    {
        this.root = root;
        _tankPanel = root.Q<TankVisualElement>("tank-panel");
        _tankBody = root.Q<VisualElement>("body");
        _tankBody.style.backgroundImage = new StyleBackground(tankData.bodySprite);
        _tankTurret = root.Q<VisualElement>("turret");
        _tankTurret.style.backgroundImage = new StyleBackground(tankData.basicTurretSprite);
        _tankName = root.Q<Label>("tank-name");
        _tankName.text = tankData.tankName;

        _tankPanel.tankName = tankData.tankName;
        _tankPanel.tankIndex = tankData.tankID;
    }
}
