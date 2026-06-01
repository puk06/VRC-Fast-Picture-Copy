using System.Runtime.InteropServices;
using System.Text.Json;

namespace VRC_Fast_Picture_Copy
{
    public partial class Main : Form
    {
        private string pictureDirectory = "";
        private string newestFile = "";
        private FileSystemWatcher watcher;

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

            foreach (var path in possiblePaths)
            {
                if (Directory.Exists(path))
                {
                    pictureDirectory = path;
                }
            }

            string configPath = $"C:\\Users\\{username}\\AppData\\Locallow\\VRChat\\VRChat\\config.json";
            if (File.Exists(configPath))
            {
                var configData = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(configPath)) ?? new();
                if (configData.TryGetValue("picture_output_folder", out var value))
                {
                    pictureDirectory = value.ToString() ?? pictureDirectory;
                }
            }

            if (File.Exists("./folder.json"))
            {
                var folderData = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText("./folder.json")) ?? new();
                if (folderData.TryGetValue("picture_output_folder", out var value))
                {
                    pictureDirectory = value.ToString() ?? pictureDirectory;
                }
            }

            if (string.IsNullOrEmpty(pictureDirectory))
            {
                MessageBox.Show("写真フォルダが見つかりませんでした。フォルダを選択してください。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FolderBrowserDialog dialog = new FolderBrowserDialog()
                {
                    Description = "写真フォルダを選択してください。"
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    pictureDirectory = dialog.SelectedPath;
                    File.WriteAllText("./folder.json", JsonSerializer.Serialize(new Dictionary<string, object> { { "picture_output_folder", pictureDirectory } }));
                }
            }

            FolderLabel.Text = $"写真フォルダ: {pictureDirectory}";
        }

        private void InitializeFileWatcher()
        {
            watcher = new FileSystemWatcher(pictureDirectory, "*.*")
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };
            watcher.Changed += (sender, e) => UpdateNewestFile(e.FullPath);
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
                    .OrderByDescending(File.GetLastWriteTime)
                    .FirstOrDefault() ?? "";
            }
            catch (Exception e)
            {
                MessageBox.Show($"エラーが発生しました: {e.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(newestFile) && File.Exists(newestFile))
            {
                SetImageToClipboard(newestFile);
            }
            else
            {
                UpdateNewestFile();
                if (!string.IsNullOrEmpty(newestFile) && File.Exists(newestFile))
                {
                    SetImageToClipboard(newestFile);
                }
                else
                {
                    MessageBox.Show("最新のファイルが見つかりませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static void SetImageToClipboard(string path)
        {
            try
            {
                using Image img = Image.FromFile(path);
                Clipboard.SetImage(img);
            }
            catch (ExternalException) { /* 無視 */ }
            catch (Exception e)
            {
                MessageBox.Show($"エラーが発生しました: {e.Message}");
            }
        }
    }
}
