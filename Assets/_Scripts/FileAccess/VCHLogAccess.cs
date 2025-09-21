using System;
using System.IO;
using System.Threading.Tasks;

#if WINDOWS_UWP // We only have these namespaces if on an UWP device
using Windows.Storage;
//using System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions;
#endif

public static class VCHLogAccess 
{
    public static void GetVCHLogContent(Action<string> onSuccess)
    {
        Task.Run(async () => await FileLoadingTask("vchState.txt", onSuccess));
    }
    
    private static async Task FileLoadingTask(string filePath, Action<string> onSuccess)
    {
#if WINDOWS_UWP //HOLOLENSE

        // Get the Documents folder
        var folder = Windows.Storage.KnownFolders.DocumentsLibrary;
        // get a file within it
        var file = await folder.GetFileAsync(filePath);

        // read the content into a buffer
        var vchStateJSON = await Windows.Storage.FileIO.ReadTextAsync(file);
#endif
        
#if UNITY_EDITOR
        
        // as a fallback and for testing in the Editor use he normal FileIO
        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents");
        var fullFilePath = Path.Combine(folderPath, filePath);
        var vchStateJSON = await File.ReadAllTextAsync(fullFilePath);

#endif        
        // finally dispatch the callback action into the Unity main thread
        MainThreadDispatcher.Instance.DoInMainThread(() => onSuccess?.Invoke(vchStateJSON));

    }
}
