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
        Time.timeScale = 0.0f;
    }

    public static void OpenMessageWindow(string textStr, Sprite sprite)
    {
        Instance.messageWindow.WindowOpen(textStr, sprite);
        Time.timeScale = 0.0f;
    }

    public static void CloseMessageWindow()
    {
        Instance.messageWindow.WindowClose(true);
    }

    public static void OpenKanbanWindow(string textStr)
    {
        Instance.kanbanWindow.WindowOpen(textStr);
    }

    public static void CloseKanbanWindow()
    {
        Instance.kanbanWindow.WindowClose();
    }

    public static void InitMessage(string textStr)
    {
        Instance.messageWindow.MessageInit(textStr);
    }

    public static bool isSending()
    {
        return Instance.messageWindow.isSending;
    }

    public static void FinishMessage()
    {
        Instance.messageWindow.MessageFinish();
    }
}
