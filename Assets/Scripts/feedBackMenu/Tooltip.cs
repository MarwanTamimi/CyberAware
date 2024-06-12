using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public int characterWrapLimit;
    public RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string content, string header = "")
    {
        if (headerField != null && contentField != null)
        {
            // Activate header field only if header is not empty and set the text
            if (!string.IsNullOrEmpty(header))
            {
                headerField.gameObject.SetActive(true);
                headerField.text = header;
                Debug.Log($"[Tooltip] Header set to: {header}"); // Log header setting
            }
            else
            {
                headerField.gameObject.SetActive(false);
                Debug.Log("[Tooltip] No header provided, hiding header field."); // Log hiding header field
            }

            // Set the content text
            contentField.text = content;
            Debug.Log($"[Tooltip] Content set to: {content}"); // Log content setting

            // Determine if layout element should be enabled based on the character wrap limit
            int headerLength = headerField.text.Length;
            int contentLength = contentField.text.Length;
            layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit);

            // Optionally, log the layout element's enabled state for debugging purposes
            Debug.Log($"[Tooltip] Layout Element Enabled: {layoutElement.enabled}");
        }
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            Vector2 position = Input.mousePosition;
            float pivotX = position.x / Screen.width;
            float pivotY = position.y / Screen.height;
            rectTransform.pivot = new Vector2(pivotX, pivotY);
            transform.position = position;
        }
    }
}
