using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;
using UnityEngine.UI;

public class MessageManager : SingletonMonoBehaviour<MessageManager>
{
    [SerializeField]
    private MessageWindow messageWindow;

    [SerializeField]
    private MessageWindow kanbanWindow;

    public static void OpenMessageWindow(string textStr)
    {
        Instance.messageWindow.WindowOpen(textStr);
    }

    public static void OpenMessageWindow(string textStr, Image image)
    {
        Instance.messageWindow.WindowOpen(textStr, image);
    }

    public static void CloseMessageWindow()
    {
        Instance.messageWindow.WindowClose();
    }

    public static void OpenKanbanWindow(string textStr)
    {
        Instance.kanbanWindow.WindowOpen(textStr);
    }

    public static void CloseKanbanWindow()
    {
        Instance.kanbanWindow.WindowClose();
    }
}
