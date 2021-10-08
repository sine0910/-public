using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public List<ChatArea> chat_List = new List<ChatArea>();

    public RectTransform messagePanel;
    public ScrollRect scrollRect;
    public Scrollbar scrollBar;

    public void ChatSort()
    {
        scrollBar.value = 0.00001f;

        Fit(chat_List[chat_List.Count - 1].BoxRect);
        Fit(chat_List[chat_List.Count - 1].AreaRect);
        chat_List[chat_List.Count - 1].AreaRect.sizeDelta = new Vector2(messagePanel.rect.width - 10, 100);
        Fit(messagePanel);
    }

    void Fit(RectTransform Rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
}
