using System;
using System.IO;
using System.Threading.Tasks;

#if WINDOWS_UWP // We only have these namespaces if on an UWP device
using Windows.Storage;
//using System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions;
#endif

public static class VCHLogAccess 
{
#if WINDOWS_UWP
    private static Windows.Storage.StorageFile _cachedFile;
#endif
    
#if UNITY_EDITOR
    private static string _cachedFilePath;
#endif
    
    private static bool _isInitialized = false;
    private static readonly object _lock = new object();
    
    /// <summary>
    /// Initialize the file reference once. Call this before using GetVCHLogContent.
    /// </summary>
    public static async Task InitializeAsync(string filePath = "vchState.txt")
    {
        lock (_lock)
        {
            if (_isInitialized) return;
        }
        
#if WINDOWS_UWP
        var folder = Windows.Storage.KnownFolders.DocumentsLibrary;
        _cachedFile = await folder.GetFileAsync(filePath);
        
#endif
        
#if UNITY_EDITOR
        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents");
        _cachedFilePath = Path.Combine(folderPath, filePath);
#endif
        
        lock (_lock)
        {
            _isInitialized = true;
        }
    }
    
    public static void GetVCHLogContent(Action<string> onSuccess, Action<Exception> onError = null)
    {
        if (!_isInitialized)
        {
            onError?.Invoke(new InvalidOperationException("VCHLogAccess not initialized. Call InitializeAsync first."));
            return;
        }
        
        Task.Run(async () => await FileReadingTask(onSuccess, onError));
        
        //Task.Run(async () => await FileLoadingTask("vchState.txt", onSuccess)); // old call
    }
    
    
    private static async Task FileReadingTask(Action<string> onSuccess, Action<Exception> onError)
    {
        try
        {
#if WINDOWS_UWP
            // Read directly from cached file reference
            var vchStateJSON = await Windows.Storage.FileIO.ReadTextAsync(_cachedFile);
            if(vchStateJSON == null || vchStateJSON == string.Empty) vchStateJSON = "ALARMMM";
            MainThreadDispatcher.Instance.DoInMainThread(() => onSuccess?.Invoke(vchStateJSON));
#endif
        
#if UNITY_EDITOR
            // Read from cached path - Works on computer
            var vchStateJSON = await File.ReadAllTextAsync(_cachedFilePath);
            MainThreadDispatcher.Instance.DoInMainThread(() => onSuccess?.Invoke(vchStateJSON));
#endif        
            // Dispatch to Unity main thread
            //MainThreadDispatcher.Instance.DoInMainThread(() => onSuccess?.Invoke(vchStateJSON));
        }
        catch (Exception ex)
        {
            // Handle errors gracefully
            MainThreadDispatcher.Instance.DoInMainThread(() => onError?.Invoke(ex));
        }
    }
    
    /// <summary>
    /// Optional: Reset initialization if file path needs to change
    /// </summary>
    public static void Reset()
    {
        lock (_lock)
        {
#if WINDOWS_UWP
            _cachedFile = null;
#endif
#if UNITY_EDITOR
            _cachedFilePath = null;
#endif
            _isInitialized = false;
        }
    }
    
///OLD 
//     private static async Task FileLoadingTask(string filePath, Action<string> onSuccess)
//     {
//         
// #if WINDOWS_UWP //HOLOLENSE
//
//         // Get the Documents folder
//         var folder = Windows.Storage.KnownFolders.DocumentsLibrary;
//         // get a file within it
//         var file = await folder.GetFileAsync(filePath);
//
//         // read the content into a buffer
//         var vchStateJSON = await Windows.Storage.FileIO.ReadTextAsync(file);
//         
// #endif
//         
// #if UNITY_EDITOR
//         
//         // as a fallback and for testing in the Editor use he normal FileIO
//         var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents");
//         var fullFilePath = Path.Combine(folderPath, filePath);
//         var vchStateJSON = await File.ReadAllTextAsync(fullFilePath);
//
// #endif        
//         // finally dispatch the callback action into the Unity main thread
//         MainThreadDispatcher.Instance.DoInMainThread(() => onSuccess?.Invoke(vchStateJSON));
//
//     }

}
