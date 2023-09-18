using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad] // �÷��� ��ư�� ������ �ʱ�ȭ �Ǵ� ��ũ��Ʈ
public class StartUpSceneLoader
{
    static StartUpSceneLoader()
    {
        EditorApplication.playModeStateChanged += LoadStartUpScene;
    }

    private static void LoadStartUpScene(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // ����������� �����ض�
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // ���� ���� ù��° ������ ������
            if (EditorSceneManager.GetActiveScene().buildIndex != 0)
            {
                EditorSceneManager.LoadScene(0);
            }
        }
    }
}
