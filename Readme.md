# Unity Windows File Drag and Drop

このUnityコンポーネントは、Windowsプラットフォーム上でUnityアプリケーションにファイルのドラッグアンドドロップ機能を追加します。

## 機能

- Windowsプラットフォームでのファイルのドラッグアンドドロップ処理
- 複数ファイルの同時ドロップに対応
- UnityEventを使用したインスペクターからのイベント設定
- UnityActionを使用したスクリプトからの動的なイベント購読
- 適切なリソース管理とクリーンアップ処理

## 要件

- Unity 2019.4 以降
- Windows プラットフォーム

## インストール

1. このリポジトリをクローンするか、`FileDragAndDrop.cs`ファイルをダウンロードします。
2. ダウンロードしたファイルをUnityプロジェクトの`Assets`フォルダ内に配置します。

## 使用方法

1. Unity Editorで空のGameObjectを作成します。
2. 作成したGameObjectに`FileDragAndDrop`コンポーネントをアタッチします。
3. 以下のいずれかの方法でドラッグアンドドロップイベントを処理します：
   - インスペクターで`OnFilesDrop`イベントを設定
   - スクリプトから`SetOnFilesDropAction`メソッドを使用してイベントを購読

### インスペクターでの設定例

1. `FileDragAndDrop`コンポーネントの`OnFilesDrop`イベントに、ファイル処理を行うメソッドをドラッグ＆ドロップで設定します。

### スクリプトでの使用例

```csharp
public class FileProcessor : MonoBehaviour
{
    private FileDragAndDrop fileDragAndDrop;

    void Start()
    {
        fileDragAndDrop = GetComponent<FileDragAndDrop>();
        fileDragAndDrop.SetOnFilesDropAction(ProcessDroppedFiles);
    }

    void OnDisable()
    {
        if (fileDragAndDrop != null)
        {
            fileDragAndDrop.RemoveOnFilesDropAction();
        }
    }

    private void ProcessDroppedFiles(List<string> files)
    {
        Debug.Log($"Processing {files.Count} dropped files...");
        foreach (string file in files)
        {
            // ファイルの処理ロジックをここに記述
            Debug.Log($"Processing file: {file}");
        }
    }
}
```

## 注意点

- このコンポーネントはWindowsプラットフォーム専用です。
- Unity Editorの設定で「Allow 'unsafe' Code」オプションを有効にする必要があります。
- ビルド設定で正しいターゲットプラットフォーム（Windows Standalone）を選択していることを確認してください。

## ライセンス

このプロジェクトはMITライセンスの下で公開されています。詳細は[LICENSE](LICENSE)ファイルを参照してください。

## コントリビューション

バグ報告や機能リクエストは、GitHubのIssueを通じてお願いします。プルリクエストも歓迎します。

## 作者

[tamanyo]

