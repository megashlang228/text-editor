using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pz12
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        delegate void isSaveHandler(bool save);
        event isSaveHandler Notify;
        private bool isSave = false;
        private string currentFile = "";
        public MainWindow()
        {
            InitializeComponent();
            Notify += changeStatusSave;
        }

        private void changeStatusSave(bool save)
        {
            if (save)
            {
                tb_status.Text = "сохранено";
            }
            else
            {
                tb_status.Text = "требуется сохранение";
            }
        }

        private void FileNewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(!isSave)
            {
                if(MessageBox.Show("предупреждение", "сохранить текущий файл", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SaveFile();
                }
            }
            TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            range.Text = "";
            currentFile = "";
            isSave = true;
            Notify.Invoke(isSave);
            
            tb_size.Text = "";
        }

        private void FileOpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void FileSaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void FilesList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!isSave)
            {
                if (MessageBox.Show("предупреждение", "сохранить текущий файл", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SaveFile();
                }
            }
            TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            var path = FilesList.SelectedItem as string;
            currentFile = path;
            FileStream fileStream = new FileStream(currentFile, FileMode.Open);
            range.Load(fileStream, DataFormats.Rtf);
            isSave = true;
            Notify.Invoke(isSave);
            fileStream.Close();
        }

        void SaveFile()
        {
            if (currentFile == "")
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
                if (dlg.ShowDialog() == true)
                {
                    FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                    TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                    range.Save(fileStream, DataFormats.Rtf);
                    currentFile = dlg.FileName;
                    FilesList.Items.Add(currentFile);
                    fileStream.Close();
                }
            }
            else
            {
                FileStream fileStream = new FileStream(currentFile, FileMode.Create);
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);
                fileStream.Close();
            }
            isSave = true;
            Notify.Invoke(isSave);
            FileInfo file = new FileInfo(currentFile);
            long size = file.Length;
            tb_size.Text = BytesToString(size);

        }

        void OpenFile()
        {
            if(!isSave)
            {
                if (MessageBox.Show("предупреждение", "сохранить текущий файл", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SaveFile();
                }
            }
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
                currentFile = dlg.FileName;
                FilesList.Items.Add(currentFile);
                isSave = true;
                Notify.Invoke(isSave);
                FileInfo file = new FileInfo(currentFile);
                long size = file.Length;
                tb_size.Text = BytesToString(size);
                fileStream.Close();
                tb_date.Text = file.LastWriteTime.ToShortDateString();

            }
        }


        private void btnBold_Click(object sender, RoutedEventArgs e)
        {
            TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            object temp = range.GetPropertyValue(FontWeightProperty);
            if (temp.Equals(FontWeights.Bold))
                range.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Normal);

            if (temp.Equals(FontWeights.Normal))
                range.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold);
        }

        private void btnItalic_Click(object sender, RoutedEventArgs e)
        {
            TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            object temp = range.GetPropertyValue(Inline.FontStyleProperty);
            if (temp.Equals(FontStyles.Italic))
                range.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Normal);
            if (temp.Equals(FontStyles.Normal))
                range.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Italic);
        }

        private void btnUnderline_Click(object sender, RoutedEventArgs e)
        {
            TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            object temp = range.GetPropertyValue(Inline.TextDecorationsProperty);
            if (temp.Equals(TextDecorations.Underline))
                range.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
            else
                range.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);

        }

        private void rtbEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            isSave = false;
            Notify.Invoke(isSave);

        }
        static String BytesToString(long byteCount)
        {
            string[] suf = { "Byt", "KB", "MB", "GB", "TB", "PB", "EB" }; //
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
    }
}
