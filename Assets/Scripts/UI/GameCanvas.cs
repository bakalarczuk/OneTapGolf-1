using System;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class GameCanvas : UIBehaviour
{
    [SerializeField]
    private new Camera camera = default;
    [SerializeField]
    private SpriteRenderer backgroundSprite = default;
    [SerializeField]
    private GameScreen gameScreen = default;
    [SerializeField]
    private QuitPanel quitPanel = default;
    [SerializeField]
    private Vector2 cameraToBackgroundRatio = new Vector2(1.75f, 1f);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameScreen.IsResultPanelVisible && EventSystem.current && EventSystem.current.enabled)
        {
            quitPanel.Show();
        }
    }

    protected override void OnRectTransformDimensionsChange()
    {
#if UNITY_EDITOR
        if (this && camera && backgroundSprite)
#endif
        {
            Vector2 bgSpriteExtents = backgroundSprite.bounds.extents;
            bgSpriteExtents.x *= cameraToBackgroundRatio.x;

            if (bgSpriteExtents.x > 0.1f && bgSpriteExtents.y > 0.1f)
            {
                SetCameraSize(bgSpriteExtents);
            }
        }
    }

    private void SetCameraSize(Vector2 bgSpriteExtents)
    {
        if (camera && backgroundSprite)
        {
            if (camera.orthographic)
            {
                float bgAspect = bgSpriteExtents.x / bgSpriteExtents.y;
                float screenAspect = 1f * Screen.width / Screen.height;
                if (bgAspect > screenAspect)
                {
                    camera.orthographicSize = bgSpriteExtents.x / camera.aspect;
                }
                else
                {
                    camera.orthographicSize = bgSpriteExtents.y;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    private void SetGameScreenAnchors(Vector2 bgSize)
    {
        if (camera)
        {
            float cameraViewWidth = 2f * camera.orthographicSize * camera.aspect;
            gameScreen.SetAnchors(camera, new Vector2(cameraViewWidth, bgSize.y));
        }
    }
}
