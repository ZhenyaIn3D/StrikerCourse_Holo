using System;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace VCHCommunication
{
    public class VCHSignalHandler: MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _textMeshProUGUI;
        
        public static Action<VCHKeyCode> OnPressButtonRecieved;
        
        private void Start()
        {
            KeyMap.Instance.OnPressButton += OnPressButton;
            KeyMap.Instance.OnTumbleSwitch += OnTumbleSwitch;
            KeyMap.Instance.OnShooterControlChange += OnShooterControlChange;
            
            KeyMap.Instance.OnJoystickControlChange += OnJoystickControlChange;
            //C:\Users\<user>\AppData\LocalLow\<company name> - persistentPath
        }
        
        //NO NEED - we rite to dataPath and copy from there
//         private void WriteSignalToSystemFile(string content)
//         {
// #if WINDOWS
// use System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)
//             using (var file = new FileStream("U:\Users\In3D-Tech\Documents\VCH", FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
//             {
//                 using (var writer = new StreamWriter(file, Encoding.UTF8))
//                 {
//                     writer.Write(content);
//                 }
//             }
// #endif
//         }
//         

        private void WriteSignal(VCHKeyCode content)
        {
            //WriteSignalToSystemFile();  NO NEED
            OnPressButtonRecieved.Invoke(content);
        }
        
        private void OnDisable()
        {
            KeyMap.Instance.OnPressButton -= OnPressButton;
            KeyMap.Instance.OnTumbleSwitch -= OnTumbleSwitch;
            KeyMap.Instance.OnShooterControlChange -= OnShooterControlChange;
            KeyMap.Instance.OnJoystickControlChange -= OnJoystickControlChange;
        }
        
        private void OnPressButton(VCHKeyCode keyCode)
        {
            _textMeshProUGUI.text = "OnPressButton: " + keyCode;
            WriteSignal(keyCode); // DO THIIIIIIIS
        }
        
        private void OnTumbleSwitch(TumblerCode tumblerCode, bool isOn)
        {
            _textMeshProUGUI.text = "OnTumbleSwitch: " + tumblerCode + " " + isOn;

        }
        
        private void OnShooterControlChange(ShooterControl shooterControl)
        {
            _textMeshProUGUI.text = "OnShooterControlChange: " + shooterControl;
        }

        private void OnJoystickControlChange(JoystickControl joystickControl)
        {
            _textMeshProUGUI.text = "OnJoystickControlChange: " + joystickControl;
        }
        
        
    }
}