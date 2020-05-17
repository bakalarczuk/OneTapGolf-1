using UnityEngine;
using UnityEngine.EventSystems;

namespace MustHave.UI
{
    public class UIScript : UIBehaviour
    {
        public bool IsShown { get { return gameObject.activeSelf; } }

        public RectTransform RectTransform => transform as RectTransform;

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}