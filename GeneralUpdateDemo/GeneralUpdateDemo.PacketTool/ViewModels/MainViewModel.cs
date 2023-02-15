using CommunityToolkit.Mvvm.Input;
using GeneralUpdate.Zip.Factory;
using GeneralUpdateDemo.Domain.DTO;
using GeneralUpdateDemo.Domain.Enum;
using GeneralUpdateDemo.Infrastructure.MVVM;
using GeneralUpdateDemo.PacketTool.Services;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeneralUpdateDemo.PacketTool.ViewModels
{
    internal class MainViewModel : ViewModeBase
    {
        #region Private Members
        private MainService _mainService;
        private bool isPublish, snackbarActive;
        private string? _currentFormat, _currentEncoding, _currnetAppType, _currentVersion, _currentClientAppKey;
        private string? sourcePath, targetPath, patchPath, infoMessage, url, packetName, snackBarMessage;
        private List<string>? _formats, _encodings, _appTypes;
        private AsyncRelayCommand<object>? buildCommand;
        private RelayCommand<string>? selectFolderCommand;
        #endregion

        #region Constructors
        public MainViewModel()
        {
            _mainService = new MainService();
            isPublish = false;
            CurrentEncoding = Encodings?.First();
            CurrentFormat = Formats.First();
            CurrnetAppType = AppTypes.First();
        }
        #endregion


        #region Public Properties

        public string? SourcePath { get => sourcePath; set => SetProperty(ref sourcePath, value); }
        public string? TargetPath { get => targetPath; set => SetProperty(ref targetPath, value); }
        public string? PatchPath { get => patchPath; set => SetProperty(ref patchPath, value); }
        public string? InfoMessage { get => infoMessage; set => SetProperty(ref infoMessage, value); }
        public bool IsPublish { get => isPublish; set => SetProperty(ref isPublish, value); }
        public string Url { get => url ?? ""; set => SetProperty(ref url, value); }
        public string? PacketName { get => packetName; set => SetProperty(ref packetName, value); }

        public string? SnackBarMessage { get => snackBarMessage; set => SetProperty(ref snackBarMessage, value); }
        public bool SnackbarActive { get => snackbarActive; set => SetProperty(ref snackbarActive, value); }

        public RelayCommand<string> SelectFolderCommand
        {
            get => selectFolderCommand ??= new RelayCommand<string>(SelectFolderAction);
        }

        public AsyncRelayCommand<object> BuildCommand
        {
            get => buildCommand ??= new AsyncRelayCommand<object>(BuildPacketCallback);
        }

        public List<string> AppTypes
        {
            get
            {
                if (_appTypes == null)
                {
                    _appTypes = new List<string>
                    {
                        "Client",
                        "Upgrade"
                    };
                }
                return _appTypes;
            }
        }

        public List<string> Formats
        {
            get
            {
                _formats ??= new List<string> { ".zip", ".7z" };
                return _formats;
            }
        }

        public List<string>? Encodings
        {
            get
            {
                if (_currentEncoding == null)
                {
                    _encodings = new List<string>
                    {
                        "Default",
                        "UTF8",
                        "UTF7",
                        "Unicode",
                        "UTF32",
                        "BigEndianUnicode",
                        "Latin1",
                        "ASCII"
                    };
                }
                return _encodings;
            }
        }

        public string? CurrentFormat
        {
            get => _currentFormat;
            set => SetProperty(ref _currentFormat, value);
        }

        public string? CurrentEncoding
        {
            get => _currentEncoding;
            set => SetProperty(ref _currentEncoding, value);
        }

        public string CurrnetAppType
        {
            get => _currnetAppType ?? "Client";
            set => SetProperty(ref _currnetAppType, value);
        }

        public string CurrentVersion { get => _currentVersion ?? "1.1.1.1"; set => SetProperty(ref _currentVersion, value); }
        public string CurrentClientAppKey { get => _currentClientAppKey ?? "123"; set => SetProperty(ref _currentClientAppKey, value); }

        #endregion

        #region Private Methods

        /// <summary>
        /// Choose a path
        /// </summary>
        /// <param name="value"></param>
        private void SelectFolderAction(string? value)
        {
            if (string.IsNullOrEmpty(value)) return;
            FolderBrowserDialog dialog = new();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                switch (value)
                {
                    case "Source":
                        SourcePath = dialog.SelectedPath.Trim();
                        break;
                    case "Target":
                        TargetPath = dialog.SelectedPath.Trim();
                        break;
                    case "Patch":
                        PatchPath = dialog.SelectedPath.Trim();
                        break;
                }
            }
        }

        /// <summary>
        ///  Build patch package
        /// </summary>
        private async Task BuildPacketCallback(object? messageQueue)
        {
            if (messageQueue is SnackbarMessageQueue smq)
            {
                if (ValidationParameters())
                {
                    smq.Enqueue("带*的字段为必填项 !");
                    return;
                }

                if (ValidationFolder())
                {
                    smq.Enqueue("文件夹不存在！");
                    return;
                }

                try
                {
                    await GeneralUpdate.Differential.DifferentialCore.Instance.Clean(SourcePath, TargetPath, PatchPath, (sender, args) => { },
                        String2OperationType(CurrentFormat), String2Encoding(CurrentEncoding), PacketName);
                    if (IsPublish)
                    {
                        var packetPath = Path.Combine(TargetPath ?? @"C:\\", $"{PacketName}{CurrentFormat}");
                        if (!File.Exists(packetPath))
                        {
                            smq.Enqueue($"在路径{packetPath}下未找到包文件 !");
                            return;
                        }
                        var md5 = GeneralUpdate.Core.Utils.FileUtil.GetFileMD5(packetPath);
                        await _mainService.PostUpgradPakcet<UploadReapDTO>(Url, packetPath, String2AppType(CurrnetAppType), CurrentVersion, CurrentClientAppKey, md5, (resp) =>
                        {
                            if (resp == null)
                            {
                                smq.Enqueue("文件包上传失败 !");
                                return;
                            }

                            if (resp.Code == HttpStatus.OK)
                            {
                                smq.Enqueue($"{resp.Message}");
                            }
                            else
                            {
                                smq.Enqueue($"{resp.Body}");
                            }
                        });
                    }
                    else
                    {
                        smq.Enqueue("发布包完成！");
                    }
                }
                catch (Exception ex)
                {
                    smq.Enqueue($"操作失败：{TargetPath} , 错误信息：{ex.Message}  !");
                }
            }
        }

        private void ShowMessage(SnackbarMessageQueue smg, string message)
        {
            smg.Enqueue(message);
        }

        private bool ValidationParameters() => (string.IsNullOrEmpty(SourcePath) || string.IsNullOrEmpty(TargetPath) || string.IsNullOrEmpty(PatchPath) ||
            string.IsNullOrEmpty(PacketName) || string.IsNullOrEmpty(CurrentFormat) || string.IsNullOrEmpty(CurrentEncoding));

        private bool ValidationFolder() => !Directory.Exists(SourcePath) || !Directory.Exists(TargetPath) || !Directory.Exists(PatchPath);

        private Encoding String2Encoding(string? encoding)
        {
            Encoding result;
            switch (encoding)
            {
                case "Default":
                    result = Encoding.Default;
                    break;
                case "UTF8":
                    result = Encoding.UTF8;
                    break;
                case "Unicode":
                    result = Encoding.Unicode;
                    break;
                case "UTF32":
                    result = Encoding.UTF32;
                    break;
                case "BigEndianUnicode":
                    result = Encoding.BigEndianUnicode;
                    break;
                case "Latin1":
                    result = Encoding.Latin1;
                    break;
                case "ASCII":
                    result = Encoding.ASCII;
                    break;
                default:
                    result = Encoding.Default;
                    break;
            }
            return result;
        }

        private OperationType String2OperationType(string? type)
        {
            var result = OperationType.GZip;
            switch (type)
            {
                case "ZIP":
                    result = OperationType.GZip;
                    break;
                case "7Z":
                    result = OperationType.G7z;
                    break;
            }
            return result;
        }

        private int String2AppType(string appType)
        {
            int result = 0;
            switch (appType)
            {
                case "Client":
                    result = 1;
                    break;
                case "UTF8":
                    result = 2;
                    break;
            }
            return result;
        }

        #endregion
    }
}
