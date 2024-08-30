# Unity Windows File Drag and Drop

����Unity�R���|�[�l���g�́AWindows�v���b�g�t�H�[�����Unity�A�v���P�[�V�����Ƀt�@�C���̃h���b�O�A���h�h���b�v�@�\��ǉ����܂��B

## �@�\

- Windows�v���b�g�t�H�[���ł̃t�@�C���̃h���b�O�A���h�h���b�v����
- �����t�@�C���̓����h���b�v�ɑΉ�
- UnityEvent���g�p�����C���X�y�N�^�[����̃C�x���g�ݒ�
- UnityAction���g�p�����X�N���v�g����̓��I�ȃC�x���g�w��
- �K�؂ȃ��\�[�X�Ǘ��ƃN���[���A�b�v����

## �v��

- Unity 2019.4 �ȍ~
- Windows �v���b�g�t�H�[��

## �C���X�g�[��

1. ���̃��|�W�g�����N���[�����邩�A`FileDragAndDrop.cs`�t�@�C�����_�E�����[�h���܂��B
2. �_�E�����[�h�����t�@�C����Unity�v���W�F�N�g��`Assets`�t�H���_���ɔz�u���܂��B

## �g�p���@

1. Unity Editor�ŋ��GameObject���쐬���܂��B
2. �쐬����GameObject��`FileDragAndDrop`�R���|�[�l���g���A�^�b�`���܂��B
3. �ȉ��̂����ꂩ�̕��@�Ńh���b�O�A���h�h���b�v�C�x���g���������܂��F
   - �C���X�y�N�^�[��`OnFilesDrop`�C�x���g��ݒ�
   - �X�N���v�g����`SetOnFilesDropAction`���\�b�h���g�p���ăC�x���g���w��

### �C���X�y�N�^�[�ł̐ݒ��

1. `FileDragAndDrop`�R���|�[�l���g��`OnFilesDrop`�C�x���g�ɁA�t�@�C���������s�����\�b�h���h���b�O���h���b�v�Őݒ肵�܂��B

### �X�N���v�g�ł̎g�p��

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
            // �t�@�C���̏������W�b�N�������ɋL�q
            Debug.Log($"Processing file: {file}");
        }
    }
}
```

## ���ӓ_

- ���̃R���|�[�l���g��Windows�v���b�g�t�H�[����p�ł��B
- Unity Editor�̐ݒ�ŁuAllow 'unsafe' Code�v�I�v�V������L���ɂ���K�v������܂��B
- �r���h�ݒ�Ő������^�[�Q�b�g�v���b�g�t�H�[���iWindows Standalone�j��I�����Ă��邱�Ƃ��m�F���Ă��������B

## ���C�Z���X

���̃v���W�F�N�g��MIT���C�Z���X�̉��Ō��J����Ă��܂��B�ڍׂ�[LICENSE](LICENSE)�t�@�C�����Q�Ƃ��Ă��������B

## �R���g���r���[�V����

�o�O�񍐂�@�\���N�G�X�g�́AGitHub��Issue��ʂ��Ă��肢���܂��B�v�����N�G�X�g�����}���܂��B

## ���

[tamanyo]


# Unity Windows File Drag and Drop

This Unity component adds file drag-and-drop functionality to Unity applications on the Windows platform.

## Features

- Handles file drag-and-drop operations on Windows platforms
- Supports simultaneous dropping of multiple files
- Allows event configuration from the Inspector using UnityEvent
- Enables dynamic event subscription from scripts using UnityAction
- Proper resource management and cleanup

## Requirements

- Unity 2019.4 or later
- Windows platform

## Installation

1. Clone this repository or download the `FileDragAndDrop.cs` file.
2. Place the downloaded file in the `Assets` folder of your Unity project.

## Usage

1. In the Unity Editor, create an empty GameObject.
2. Attach the `FileDragAndDrop` component to the created GameObject.
3. Handle drag-and-drop events using one of the following methods:
   - Configure the `OnFilesDrop` event in the Inspector
   - Use the `SetOnFilesDropAction` method from a script to subscribe to events

### Example of Setting in the Inspector

1. Set the method for processing files in the `OnFilesDrop` event of the `FileDragAndDrop` component using drag and drop in the Inspector.

### Example of Using in a Script

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
            // Implement file processing logic here
            Debug.Log($"Processing file: {file}");
        }
    }
}
```

## Notes

- This component is specifically for the Windows platform.
- Ensure the "Allow 'unsafe' Code" option is enabled in the Unity Editor settings.
- Verify that the correct target platform (Windows Standalone) is selected in the build settings.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## Contribution

For bug reports or feature requests, please use the GitHub Issues. Pull requests are also welcome.

## Author

[tamanyo]


