using System.Runtime.InteropServices;
using System.Text.Json;

namespace VRC_Fast_Picture_Copy
{
    public partial class Main : Form
    {
        private static readonly HashSet<string> SupportedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".bmp",
            ".gif",
            ".jpeg",
            ".jpg",
            ".png",
            ".tif",
            ".tiff",
            ".webp"
        };

        private string pictureDirectory = "";
        private string newestFile = "";
        private FileSystemWatcher? watcher = null;

        public Main()
        {
            InitializeComponent();
            InitializePictureDirectory();

            if (string.IsNullOrEmpty(pictureDirectory) || !Directory.Exists(pictureDirectory))
            {
                MessageBox.Show("写真フォルダが見つかりませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UpdateNewestFile();
            InitializeFileWatcher();
        }

        private void InitializePictureDirectory()
        {
            string username = Environment.UserName;
            string[] possiblePaths =
            {
                $"C:\\Users\\{username}\\Pictures\\VRChat",
                $"C:\\Users\\{username}\\OneDrive\\Pictures\\VRChat"
            };

            pictureDirectory = possiblePaths.FirstOrDefault(Directory.Exists) ?? "";

            string configPath = $"C:\\Users\\{username}\\AppData\\Locallow\\VRChat\\VRChat\\config.json";
            pictureDirectory = GetPicturePathFromJson(configPath) ?? pictureDirectory;
            pictureDirectory = GetPicturePathFromJson("./folder.json") ?? pictureDirectory;

            if (string.IsNullOrEmpty(pictureDirectory))
            {
                BrowseForFolder();
            }

            FolderLabel.Text = $"写真フォルダ: {pictureDirectory}";
        }

        private string? GetPicturePathFromJson(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                var jsonData = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(filePath));
                return jsonData?.TryGetValue("picture_output_folder", out var value) == true
                    ? value.ToString()
                    : null;
            }
            catch
            {
                return null;
            }
        }

        private void BrowseForFolder()
        {
            MessageBox.Show("写真フォルダが見つかりませんでした。フォルダを選択してください。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            using (FolderBrowserDialog dialog = new FolderBrowserDialog()
            {
                Description = "写真フォルダを選択してください。"
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    pictureDirectory = dialog.SelectedPath;
                    File.WriteAllText("./folder.json", JsonSerializer.Serialize(new Dictionary<string, object> { { "picture_output_folder", pictureDirectory } }));
                }
            }
        }

        private void InitializeFileWatcher()
        {
            watcher = new FileSystemWatcher(pictureDirectory, "*.*")
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite
            };
            watcher.Changed += (_, e) => UpdateNewestFile(e.FullPath);
        }

        private void UpdateNewestFile(string? newFilePath = null)
        {
            newestFile = newFilePath ?? GetNewestFile();
            if (!string.IsNullOrEmpty(newestFile) && File.Exists(newestFile))
            {
                FileLabel.Text = $"最新のファイル: {File.GetLastWriteTime(newestFile):yyyy/MM/dd HH:mm:ss}";
            }
        }

        private string GetNewestFile()
        {
            try
            {
                return Directory.EnumerateFiles(pictureDirectory, "*", SearchOption.AllDirectories)
                    .Where(IsSupportedImageFile)
                    .OrderByDescending(File.GetLastWriteTime)
                    .FirstOrDefault() ?? "";
            }
            catch (Exception e)
            {
                MessageBox.Show($"エラーが発生しました: {e.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(newestFile) || !File.Exists(newestFile))
            {
                UpdateNewestFile();
            }

            if (!string.IsNullOrEmpty(newestFile) && File.Exists(newestFile))
            {
                SetImageToClipboard(newestFile);
            }
            else
            {
                MessageBox.Show("最新のファイルが見つかりませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void SetImageToClipboard(string path)
        {
            try
            {
                using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using Image img = Image.FromStream(stream);
                using Bitmap bitmap = new(img);
                Clipboard.SetImage(bitmap);
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("画像を読み込めませんでした。書き込み中のファイルか、壊れた画像の可能性があります。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ExternalException) { /* 無視 */ }
            catch (Exception e)
            {
                MessageBox.Show($"エラーが発生しました: {e.Message}");
            }
        }

        private static bool IsSupportedImageFile(string path)
        {
            string extension = Path.GetExtension(path);
            return SupportedImageExtensions.Contains(extension);
        }
    }
}
