using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UserUI
{
    private VisualElement _root;
    private Label _nameLabel;
    private VisualElement _tankIcon;

    public UserUI(VisualElement root, string name, Sprite sprite, bool ready)
    {
        _root = root.Q<VisualElement>("user");
        _nameLabel = root.Q<Label>("user-name");
        _nameLabel.text = name;
        _tankIcon = root.Q<VisualElement>("tank-icon");
        SetTank(sprite);
        SetReady(ready);
    }

    public void SetTank(Sprite sprite)
    {
        if(sprite == null)
        {
            _tankIcon.style.backgroundImage = null;
        }
        else
        {
            _tankIcon.style.backgroundImage = new StyleBackground(sprite);
        }
    }

    public void SetReady(bool ready)
    {
        if (ready)
        {
            _root.AddToClassList("on");
        }
        else
        {
            _root.RemoveFromClassList("on");
        }
    }

    public void RemoveFromTree()
    {
        _root.RemoveFromHierarchy();
    }
}
