using UnityEngine;
using UnityEngine.Events;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

[Serializable]
public class StringListEvent : UnityEvent<List<string>> { }

public class FileDragAndDropHandler : MonoBehaviour
{
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern uint DragQueryFile(IntPtr hDrop, uint iFile, [Out] StringBuilder lpszFile, uint cch);

    [DllImport("shell32.dll")]
    private static extern void DragFinish(IntPtr hDrop);

    private const int WM_DROPFILES = 0x0233;
    private const int GWL_WNDPROC = -4;

    private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    private delegate void DragAcceptFilesDelegate(IntPtr hWnd, bool fAccept);

    private IntPtr oldWndProcPtr;
    private WndProcDelegate newWndProc;
    private IntPtr shell32Module;
    private DragAcceptFilesDelegate dragAcceptFilesFunc;
    private IntPtr unityHWnd;
    private bool isInitialized = false;

    // ドロップされたファイルを保存するリスト
    private List<string> droppedFiles = new List<string>();

    // UnityEvent for drag and drop
    public StringListEvent OnFilesDrop;

    // UnityAction for drag and drop
    private UnityAction<List<string>> onFilesDropAction;

    void Start()
    {
        if (!Application.isEditor && !Application.platform.ToString().Contains("Windows"))
        {
            Debug.LogWarning("FileDragAndDrop is only supported on Windows platforms.");
            return;
        }

        try
        {
            InitializeDragAndDrop();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize drag and drop: {e.Message}\n{e.StackTrace}");
        }
    }

    private void InitializeDragAndDrop()
    {
        shell32Module = LoadLibrary("shell32.dll");
        if (shell32Module == IntPtr.Zero)
        {
            throw new Exception($"Failed to load shell32.dll. Error code: {Marshal.GetLastWin32Error()}");
        }

        IntPtr dragAcceptFilesPtr = GetProcAddress(shell32Module, "DragAcceptFiles");
        if (dragAcceptFilesPtr == IntPtr.Zero)
        {
            throw new Exception($"Failed to get address of DragAcceptFiles. Error code: {Marshal.GetLastWin32Error()}");
        }

        dragAcceptFilesFunc = Marshal.GetDelegateForFunctionPointer<DragAcceptFilesDelegate>(dragAcceptFilesPtr);

        unityHWnd = GetCorrectWindowHandle();
        Debug.Log($"Unity window handle: {unityHWnd}");

        if (!IsWindow(unityHWnd))
        {
            throw new Exception($"Invalid window handle: {unityHWnd}");
        }

        newWndProc = new WndProcDelegate(WndProc);
        IntPtr newWndProcPtr = Marshal.GetFunctionPointerForDelegate(newWndProc);

        oldWndProcPtr = SetWindowLongPtr(unityHWnd, GWL_WNDPROC, newWndProcPtr);
        if (oldWndProcPtr == IntPtr.Zero)
        {
            throw new Exception($"Failed to set window procedure. Error code: {Marshal.GetLastWin32Error()}");
        }

        Debug.Log("Window procedure set successfully");

        dragAcceptFilesFunc(unityHWnd, true);
        int dragAcceptError = Marshal.GetLastWin32Error();
        if (dragAcceptError != 0)
        {
            throw new Exception($"Failed to enable drag and drop. Error code: {dragAcceptError}");
        }

        Debug.Log("Drag and drop enabled successfully");
        isInitialized = true;
    }

    private IntPtr GetCorrectWindowHandle()
    {
        IntPtr handle = GetActiveWindow();
        if (handle != IntPtr.Zero)
        {
            int processId;
            GetWindowThreadProcessId(handle, out processId);
            if (processId == System.Diagnostics.Process.GetCurrentProcess().Id)
            {
                return handle;
            }
        }

        // フォールバック: Unity の組み込み関数を使用
        return System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
    }

    void OnDisable()
    {
        CleanUp();
    }

    void OnDestroy()
    {
        CleanUp();
    }

    void OnApplicationQuit()
    {
        CleanUp();
    }

    private void CleanUp()
    {
        if (!isInitialized) return;

        try
        {
            Debug.Log("Cleaning up FileDragAndDrop...");

            if (unityHWnd != IntPtr.Zero && oldWndProcPtr != IntPtr.Zero)
            {
                SetWindowLongPtr(unityHWnd, GWL_WNDPROC, oldWndProcPtr);
                Debug.Log("Restored original window procedure");
            }

            if (dragAcceptFilesFunc != null && unityHWnd != IntPtr.Zero)
            {
                dragAcceptFilesFunc(unityHWnd, false);
                Debug.Log("Disabled drag and drop");
            }

            if (shell32Module != IntPtr.Zero)
            {
                if (FreeLibrary(shell32Module))
                {
                    Debug.Log("Freed shell32.dll");
                }
                else
                {
                    Debug.LogError($"Failed to free shell32.dll. Error code: {Marshal.GetLastWin32Error()}");
                }
            }

            isInitialized = false;
            Debug.Log("FileDragAndDrop cleanup completed");
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception during cleanup: {e.Message}\n{e.StackTrace}");
        }
    }

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        try
        {
            switch (msg)
            {
                case WM_DROPFILES:
                    Debug.Log("WM_DROPFILES message received");
                    string[] files = GetDroppedFiles(wParam);
                    ProcessDroppedFiles(files);
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception in WndProc: {e.Message}\n{e.StackTrace}");
        }

        return CallWindowProc(oldWndProcPtr, hWnd, msg, wParam, lParam);
    }

    private string[] GetDroppedFiles(IntPtr hDrop)
    {
        uint num = DragQueryFile(hDrop, 0xFFFFFFFF, null, 0);
        string[] files = new string[num];

        for (uint i = 0; i < num; i++)
        {
            uint size = DragQueryFile(hDrop, i, null, 0);
            StringBuilder sb = new StringBuilder((int)size + 1);
            DragQueryFile(hDrop, i, sb, size + 1);
            files[i] = sb.ToString();
        }

        DragFinish(hDrop);
        return files;
    }

    private void ProcessDroppedFiles(string[] files)
    {
        droppedFiles.Clear(); // 以前のファイルリストをクリア

        Debug.Log($"Number of files dropped: {files.Length}");
        foreach (string file in files)
        {
            Debug.Log($"Dropped file: {file}");
            droppedFiles.Add(file);
        }

        // 例: 最初の5つのファイルのみを表示（ファイルが多い場合）
        int displayCount = Mathf.Min(5, droppedFiles.Count);
        string fileList = string.Join("\n", droppedFiles.GetRange(0, displayCount));
        if (droppedFiles.Count > 5)
        {
            fileList += $"\n... and {droppedFiles.Count - 5} more file(s)";
        }
        Debug.Log($"Processed files:\n{fileList}");

        // UnityEventを呼び出す
        OnFilesDrop?.Invoke(droppedFiles);

        // UnityActionを呼び出す
        onFilesDropAction?.Invoke(droppedFiles);
    }

    // ドロップされたファイルのリストを取得するパブリックメソッド
    public List<string> GetDroppedFilesList()
    {
        return new List<string>(droppedFiles); // リストのコピーを返す
    }

    // UnityActionを設定するメソッド
    public void SetOnFilesDropAction(UnityAction<List<string>> action)
    {
        onFilesDropAction = action;
    }

    // UnityActionを削除するメソッド
    public void RemoveOnFilesDropAction()
    {
        onFilesDropAction = null;
    }
}