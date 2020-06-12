using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MessageManager : SingletonMonoBehaviour<MessageManager>
{
    [SerializeField]
    private MessageWindow messageWindow;

    [SerializeField]
    private MessageWindow kanbanWindow;

    private void Start()
    {
        //シーンが切り替わったときに閉じる
        SceneManager.sceneLoaded += Close;
    }

    public static void OpenMessageWindow(string textStr)
    {
        Instance.messageWindow.gameObject.SetActive(true);
        Instance.messageWindow.WindowOpen(textStr);
        // Time.timeScale = 0.0f; // 変更 tada
    }

    public static void OpenMessageWindow(string textStr, Sprite sprite)
    {
        Instance.messageWindow.gameObject.SetActive(true);
        Instance.messageWindow.WindowOpen(textStr, sprite);
        // Time.timeScale = 0.0f; // 変更 tada
    }

    public static void CloseMessageWindow()
    {
        Instance.messageWindow.WindowClose(true);
    }

    public static void OpenKanbanWindow(string textStr)
    {
        Instance.kanbanWindow.gameObject.SetActive(true);
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

    void Close(Scene nextScene, LoadSceneMode mode)
    {
        CloseMessageWindow();
        CloseKanbanWindow();
    }
}
