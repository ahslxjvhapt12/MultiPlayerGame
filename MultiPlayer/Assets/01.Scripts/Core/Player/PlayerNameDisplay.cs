using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private TankPlayer _player;
    [SerializeField] private TMP_Text _displayText;

    private void Start()
    {
        // 플레이어에 있는 이름을 변경되었는지 구독하고
        HandlePlayerNameChanged(string.Empty, _player.playerName.Value);
        _player.playerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        // 변경되면 이 함수가 실행되도록 한다.
        // 이 함수가 실행되면 텍스트를 변경해야 한다.
        _displayText.text = newName.ToString();
    }

    private void OnDestroy()
    {
        // 파괴될때는 안전하게 이벤트를 구독취소 해야한다.
        _player.playerName.OnValueChanged -= HandlePlayerNameChanged;
    }
}
