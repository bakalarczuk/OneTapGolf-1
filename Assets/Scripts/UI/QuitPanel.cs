using UnityEngine;
using UnityEngine.UI;
using MustHave.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class QuitPanel : UIScript
{
    [SerializeField]
    private Button confirmButton = default;
    [SerializeField]
    private Button cancelButton = default;

    protected override void Awake()
    {
        cancelButton.onClick.AddListener(Hide);
        confirmButton.onClick.AddListener(QuitApplication);
    }

    private void QuitApplication()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
