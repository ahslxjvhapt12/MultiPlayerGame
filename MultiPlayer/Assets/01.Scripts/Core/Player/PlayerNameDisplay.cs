using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private TankPlayer _player;
    [SerializeField] private TMP_Text _displayText;

    private void Start()
    {
        // �÷��̾ �ִ� �̸��� ����Ǿ����� �����ϰ�
        HandlePlayerNameChanged(string.Empty, _player.playerName.Value);
        _player.playerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        // ����Ǹ� �� �Լ��� ����ǵ��� �Ѵ�.
        // �� �Լ��� ����Ǹ� �ؽ�Ʈ�� �����ؾ� �Ѵ�.
        _displayText.text = newName.ToString();
    }

    private void OnDestroy()
    {
        // �ı��ɶ��� �����ϰ� �̺�Ʈ�� ������� �ؾ��Ѵ�.
        _player.playerName.OnValueChanged -= HandlePlayerNameChanged;
    }
}
