using TMPro;
using UnityEngine;

namespace FulcrumGames.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMatchObjectName : MonoBehaviour
    {
        [SerializeField]
        private GameObject _targetObject;

        [SerializeField]
        private TextMeshProUGUI _label;

        private void OnValidate()
        {
            if (!_label)
            {
                _label = GetComponent<TextMeshProUGUI>();
                if (!_label)
                    return;
            }

            var targetObject = _targetObject;
            if (!targetObject)
            {
                targetObject = gameObject;
                if (!targetObject)
                    return;
            }

            _label.text = targetObject.name;
        }
    }
}
