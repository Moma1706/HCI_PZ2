using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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



namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly List<int> fonst = new List<int>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
        public static RichTextBox sRtb;
        private string saveFile = "";
        private bool save = false;
        int wordsCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = fonst;
            sRtb = rtbEditor;
        }

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmbFontFamily.SelectedItem != null)
            {
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
            }
        }

        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = rtbEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
            btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) &&
                (temp.Equals(FontWeights.Bold));

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
            btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) &&
                (temp.Equals(FontStyles.Italic));

            temp = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) &&
                (temp.Equals(TextDecorations.Underline));

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            cmbFontFamily.SelectedItem = temp;

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
            cmbFontSize.Text = temp.ToString();

            temp = rtbEditor.Selection.GetPropertyValue(ForegroundProperty);
            btnColor.Background = temp == DependencyProperty.UnsetValue ? Brushes.Black : (Brush)temp;
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {

            if (!save)
            {
                if (wordsCount == 0)
                {
                    this.Close();
                    return;
                }

                MessageBoxResult res = MessageBox.Show("Do you want to save changes?", "Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    btnSave_Click(sender, e);
                }
                else if (res == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);

            if (saveFile == String.Empty)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = "new.rtf";
                saveFileDialog.Filter = "Rtf files (*.rtf)|*.rtf";

                if (saveFileDialog.ShowDialog() == true)
                {
                    FileStream file = new FileStream(saveFileDialog.FileName, FileMode.Create);
                    saveFile = file.Name;

                    textRange.Save(file, DataFormats.Rtf);
                    file.Close();
                }
            }
            else
            {
                FileStream file = new FileStream(saveFile, FileMode.Open);
                textRange.Save(file, DataFormats.Rtf);
                file.Close();
                MessageBox.Show("Save seccessful", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
            save = true;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            if (!save)
            {
                if (wordsCount == 0)
                    return;
                else
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to save changes?", "Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        btnSave_Click(sender, e);
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }
            }

            saveFile = "";
            save = false;

            btnBold.IsChecked = false;
            btnItalic.IsChecked = false;
            btnUnderline.IsChecked = false;
            cmbFontFamily.SelectedItem = "Segoe UI";
            cmbFontSize.Text = "12";

            rtbEditor.Document.Blocks.Clear();
        }

        private void rtbEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
                rtbEditor.CaretPosition.InsertTextInRun(DateTime.Now.ToString("dd-MM-yyyy HH:mm"));
        }

        private void btnDate_Click(object sender, RoutedEventArgs e)
        {
            rtbEditor.CaretPosition.InsertTextInRun(DateTime.Now.ToString("dd-MM-yyyy HH:mm"));
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            TextRange text = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            OpenFileDialog dialog = new OpenFileDialog();
            if (!save )
            {
                if (wordsCount == 0)
                {
                    dialog.Filter = "Rich text Format(*.rtf)|*.rtf|All files(*.*)|*.*";
                    if (dialog.ShowDialog() == true)
                    {
                        FileStream fileStream = new FileStream(dialog.FileName, FileMode.Open);
                        saveFile = fileStream.Name;

                        text.Load(fileStream, DataFormats.Rtf);
                        fileStream.Close();
                    }
                    save = true;
                    return;
                }
                else
                {
                    MessageBoxResult res = MessageBox.Show("Do you want to save changes?", "Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        btnSave_Click(sender, e);
                    }
                    else if (res == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }
            }
            dialog.Filter = "Rich text Format(*.rtf)|*.rtf|All files(*.*)|*.*";
            if (dialog.ShowDialog()==true)
            {
                FileStream fileStream = new FileStream(dialog.FileName, FileMode.Open);
                saveFile = fileStream.Name;

                text.Load(fileStream, DataFormats.Rtf);
                fileStream.Close();
            }
            save = true;
        }

        private void rtbEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextRange text = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);

            MatchCollection matchCollection = Regex.Matches(text.Text, @"[\w]+");
            wordsCount = matchCollection.Count;
            lblBroj.Content = wordsCount;
            save = false;
        }

        private void btnColor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog color = new System.Windows.Forms.ColorDialog();
            color.ShowDialog();
            System.Windows.Media.Color newColor = System.Windows.Media.Color.FromArgb(color.Color.A, color.Color.R, color.Color.G, color.Color.B);
            SolidColorBrush brush = new SolidColorBrush(newColor);

            btnColor.Background = brush;


            rtbEditor.Selection.ApplyPropertyValue(Inline.ForegroundProperty, brush);
        }
        bool validate()
        {
            bool temp=false; // provera za ceo velidate
            bool temp2; //provera za parse
            int size = 0;

            if (cmbFontSize.Text == String.Empty) //ako je prazan string -> err
            {
                temp = false;
            }
            else
            {
                try //ako nije prazan ali nije broj
                {
                    size = int.Parse(cmbFontSize.Text);
                    temp2 = true;
                }
                catch (Exception)
                {
                    temp2 = false;
                }

                if (temp2)
                {
                    if (!(size <= 0 || size > 35000))
                    {
                        temp = true;
                    }
                }
            }
            return temp;
        }

        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (validate())
            {
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
            }
        }

        private void btnFR_Click(object sender, RoutedEventArgs e)
        {
            Find find = new Find();
            find.ShowDialog();
        }
    }
}
