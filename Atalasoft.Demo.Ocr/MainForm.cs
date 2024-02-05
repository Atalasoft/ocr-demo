// ------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Atalasoft">
//     (c) 2000-2024 Atalasoft, a Kofax Company. All rights reserved. Use is subject to license terms.
// </copyright>
// ------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Atalasoft.Imaging;
using Atalasoft.Imaging.Codec;
using Atalasoft.Ocr;
using Atalasoft.Ocr.GlyphReader;
using Atalasoft.Ocr.Tesseract;
using Atalasoft.Ocr.OmniPage;
using Microsoft.Win32;

namespace Atalasoft.Demo.Ocr
{
    /// <summary>
    /// Summary description for MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Fields

        private const string AppTitle = "Simple OCR Demo";
        private bool _validLicense;
        private static readonly string TempDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Atalasoft\Demos\OCR_temp");
        private static readonly string TempFile = Path.Combine(TempDir, "temp");
        private static readonly string DefaultOutputFile = Path.Combine(TempDir, "output.txt");
        private static string _outputFile = DefaultOutputFile;
        private string _selectedMimeType = "";
        private bool _fileLoaded;

        private OcrEngine _engine;          // currently selected engine
        private OcrEngine _tesseract;       // Tesseract ditto
        private OcrEngine _tesseract3;      // This is the new (as of 10.4.1) Tesseract 3 engine
        private OcrEngine _tesseract5;      // This is the new (as of 10.4.1) Tesseract 3 engine
        private OcrEngine _glyphReader;     // GlyphReader likewise
        private OcrEngine _omniPage;        // OmniPage

        private bool _saveToFile;

        #endregion

        #region Constructors

        static MainForm()
        {
            WinDemoHelperMethods.PopulateDecoders(RegisteredDecoders.Decoders);
        }

        public MainForm()
        {
            // Verify the DotImage license.
            CheckLicenseFile();

            if (!_validLicense)
                return;
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // Pick a licensed engine to start with.
            OnMenuGlyphReaderEngineClick(null, EventArgs.Empty);
        }

        #endregion

        #region Generation file names

        // user picks a file name and mime type.
        private string GetSaveFileName()
        {
            var saveFile = new SaveFileDialog();
            var mimeTypes = _engine.SupportedMimeTypes();
            saveFile.Filter = PopulateFilter(mimeTypes);
            saveFile.AddExtension = true;
            //select the correct starting filter index
            for (var i = 0; i < mimeTypes.Length; i++)
            {
                if (mimeTypes[i] == _selectedMimeType)
                {
                    saveFile.FilterIndex = i + 1;
                    break;
                }
            }
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                // find out which mime type was selected, and save this for use when translating
                _selectedMimeType = mimeTypes[saveFile.FilterIndex - 1];
                return saveFile.FileName;
            }
            Refresh();
            return null;
        }

        // returns a string formatted for use as a filter in save/open FileDialog populated
        // with all the supported mime types.  An important thing is that the foreach statement
        // goes through the array in ascending order or else we will not know which mime type is
        // selected.
        private string PopulateFilter(string[] types)
        {
            var mimeFilter = "";
            foreach (var s in types)
            {
                switch (s)
                {
                    case "text/plain":
                        mimeFilter += s + " (.txt)|*.txt|";
                        break;
                    case "text/html":
                        mimeFilter += s + " (.htm,.html)|*.htm;*.html|";
                        break;
                    case "text/richtext":
                        mimeFilter += s + " (.rtf)|*.rtf|";
                        break;
                    case "image/x-amidraw":
                        mimeFilter += s + " (.txt)|*.txt|";
                        break;
                    case "application/pdf":
                        mimeFilter += s + " (.pdf)|*.pdf|";
                        break;
                    case "application/msword":
                        mimeFilter += s + " (.doc)|*.doc|";
                        break;
                    case "application/wordperfect":
                        mimeFilter += s + " (.wpd)|*.wpd|";
                        break;
                    case "text/tab-separated-values":
                        mimeFilter += s + " (.txt)|*.txt|";
                        break;
                    case "text/csv":
                        mimeFilter += s + " (.csv)|*.csv|";
                        break;
                    case "text/comma-separated-values":
                        mimeFilter += s + " (.csv)|*.csv|";
                        break;
                    case "application/vnd.lotus-1-2-3":
                        mimeFilter += s + " (.txt)|*.txt|";
                        break;
                    case "application/epub+zip":
                        mimeFilter += s + " (.epub)|*.epub|";
                        break;
                    case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                        mimeFilter += s + " (.docx)|*.docx|";
                        break;
                    case "application/vnd.ms-excel":
                    case "application/excel":
                        mimeFilter += s + " (.xls)|*.xls|";
                        break;
                    case "application/vnd.ms-powerpoint":
                        mimeFilter += s + " (.ppt)|*.ppt|";
                        break;
                    case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                        mimeFilter += s + " (.pptx)|*.pptx|";
                        break;
                    case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                        mimeFilter += s + " (.xlsx)|*.xlsx|";
                        break;
                    case "text/xml":
                        mimeFilter += s + " (.xml)|*.xml|";
                        break;
                    case "application/vnd.ms-xpsdocument":
                        mimeFilter += s + " (.xps)|*.xps|";
                        break;
                    default:
                        mimeFilter += s + " (.???)|*.*|";
                        break;
                }
            }
            // remove the last '|'
            return mimeFilter.Remove(mimeFilter.Length - 1, 1);
        }

        #endregion

        #region Check for license code

        private void CheckLicenseFile()
        {
            // Make sure a license for DotImage and Advanced DocClean exist.
            try
            {
                var img = new AtalaImage();
                img.Dispose();
            }
            catch (AtalasoftLicenseException ex1)
            {
                LicenseCheckFailure("This demo requires an Atalasoft DotImage license.", ex1.Message);
                return;
            }

            if (AtalaImage.Edition != LicenseEdition.Document)
            {
                LicenseCheckFailure("This demo requires an Atalasoft DotImage Document Imaging License.",
                    string.Format("Your current license is for '{0}'.", AtalaImage.Edition));
                return;
            }

            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                // We need this line to check the license.               
                new TranslatorCollection();
            }
            catch (AtalasoftLicenseException ex2)
            {
                LicenseCheckFailure("Licensing exception.", ex2.Message);
                return;
            }

            _validLicense = true;
        }

        private void LicenseCheckFailure(string message, string details)
        {
            Load += OnSimpleOcrFormLoad;
            if (details == "")
            {
                details = "(None)";
            }
            var dialogResult = YesNoBox(string.Format(
                "{0}\r\n\r\nWould you like to request an evaluation license?\r\n\r\nDetails: {1}",
                message, details));
            if (dialogResult != DialogResult.Yes)
                return;
            var asm = Assembly.Load("Atalasoft.dotImage");
            if (asm != null)
            {
                var version = asm.GetName().Version.ToString(2);

                // Locate the activation utility.
                var path = "";
                var key = Registry.LocalMachine.OpenSubKey(@"Software\Atalasoft\dotImage\" + version);
                if (key != null)
                {
                    path = Convert.ToString(key.GetValue("AssemblyBasePath"));
                    if (path.Length > 5)
                    {
                        var binDir = new DirectoryInfo(path);
                        if (binDir.Parent != null)
                            path = binDir.Parent.FullName;
                    }
                    else
                    {
                        path = Path.GetFullPath(@"..\..\..\..\..");
                    }
                    path = Path.Combine(path, "AtalasoftToolkitActivation.exe");

                    key.Close();
                }

                if (File.Exists(path))
                {
                    UseWaitCursor = true;
                    Cursor.Current = Cursors.WaitCursor;
                    var process = Process.Start(path);
                    if (process != null)
                        process.WaitForInputIdle(3000);
                    UseWaitCursor = false;
                }
                else
                    InfoBox("Could not find the DotImage Activation utility.\r\n\r\n" +
                        "Please run it from the Start menu shortcut.");
            }
            else
                InfoBox("Unable to load the DotImage assembly.");
        }

        #endregion

        #region Load Mime Types

        // event handler to apply find the selected mime type
        private void OnMimeClick(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            // save for using in OCR translate
            _selectedMimeType = item.Text;
            // This is the submenu to "Translate ..." so we want to start translation to display only, here.
            DoTranslation();
        }

        // load all of the supported mime types into a menu.
        public void LoadMimeMenu()
        {
            _menuActionTranslate.MenuItems.Clear();

            // add each type
            var mimes = _engine.SupportedMimeTypes();
            foreach (var mime in mimes)
            {
                _menuActionTranslate.MenuItems.Add(mime, OnMimeClick);
            }
            // first entry is default
            // save for using in OCR translate
            _selectedMimeType = _menuActionTranslate.MenuItems[0].Text;
        }

        #endregion

        #region Menu event handlers

        #region File Menu event handlers

        // This method copies the selected file into a temp directory for OCR processing.
        // The file must be coppied because the Translate method must be supplied a directory
        // containing all of the images to process.
        private void OnMenuFileOpenClick(object sender, EventArgs e)
        {
            // try to locate images folder
            var path = Application.ExecutablePath;
            // we assume we are running under the DotImage install folder
            var imagesFolder = Path.GetDirectoryName(path);
            while (path != null)
            {
                path = Path.GetDirectoryName(path);
                var folderName = Path.GetFileName(path);
                if (folderName == null || !folderName.Contains("DotImage "))
                    continue;
                imagesFolder = Path.Combine(path, "Images\\Documents");
                break;
            }

            //use this folder as starting point			
            _openFileDialog.InitialDirectory = imagesFolder;
            _openFileDialog.Filter = WinDemoHelperMethods.CreateDialogFilter(true);

            if (_openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            if (!Directory.Exists(TempDir))
                Directory.CreateDirectory(TempDir);

            try
            {
                if (File.Exists(TempFile))
                {
                    if ((File.GetAttributes(TempFile) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        File.SetAttributes(TempFile, FileAttributes.Normal);
                    }

                    File.Delete(TempFile);
                }
                                               
                File.Copy(_openFileDialog.FileName, TempFile, true);
            }
            catch (Exception ex)
            {
                _fileLoaded = false;
                InfoBox(ex.Message);
                return;
            }

            // display the file.
            try
            {
                _workspaceViewer.Open(TempFile);
                _fileLoaded = true;
            }
            catch (Exception)
            {
                _fileLoaded = false;
                MessageBox.Show("Unable to open requested image... Unsupported Image Type.");
            }
        }

        private void OnMenuFileExitClick(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Action Menu event handlers

        // event handler to select what to do with the results
        private void OnMenuActionOcrClick(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            if (item.Checked) return;

            if (item.Index == 1)
            {
                // save to file
                _saveToFile = true;
                _menuActionSave.Checked = true;
                _menuActionDisplay.Checked = false;
            }
            else
            {
                // display only
                _saveToFile = false;
                _menuActionDisplay.Checked = true;
                _menuActionSave.Checked = false;
                LoadMimeMenu();
            }
        }

        private void OnMenuActionTranslateClick(object sender, EventArgs e)
        {
            // only act on this event if in save to file mode, otherwise the translation
            // is started by a sub-menu.
            if (_saveToFile)
                DoTranslation();
        }

        #endregion

        #region Engine Memu event handler

        private void OnMenuGlyphReaderEngineClick(object sender, EventArgs e)
        {
            try
            {
                var loader = new GlyphReaderLoader();
                if (loader.Loaded)
                {
                    SelectGlyphReaderEngine();
                }
            }
            catch (AtalasoftLicenseException ex)
            {
                LicenseCheckFailure(
                    "Using GlyphReader OCR requires an Atalasoft DotImage GlyphReader OCR License.",
                    ex.Message);
            }
        }

        private void OnMenuTesseract3Click(object sender, EventArgs e)
        {
            try
            {
                SelectTesseract3Engine();
            }
            catch (AtalasoftLicenseException ex)
            {
                LicenseCheckFailure("Using Tesseract3 OCR requires a DotImage OCR License.", ex.Message);
            }
        }

        private void OnMenuTesseract5Click(object sender, EventArgs e)
        {
            try
            {
                SelectTesseract5Engine();
            }
            catch (AtalasoftLicenseException ex)
            {
                LicenseCheckFailure("Using Tesseract5 OCR requires a DotImage OCR License.", ex.Message);
            }
        }

        private void SelectOmniPageEngine()
        {
            if (_omniPage == null)
            {
                try
                {
                    OmniPageLoader loader = new OmniPageLoader();
                    if (loader != null)
                    {
                        // try to create an OmniPage engine
                        _omniPage = new OmniPageEngine();
                        InitializeEngine(_omniPage);
                    }
                }
                catch (AtalasoftLicenseException ex)
                {
                    LicenseCheckFailure("Using OmniPage OCR requires both an Atalasoft DotImage OCR License.", ex.Message);
                }
                catch (Exception err)
                {
                    InfoBox(err.Message);
                }
            }
            if (_omniPage != null)
            {
                _engine = _omniPage;
                UpdateMenusForEngine();
            }
        }

        private void OnMenuOmniPageClick(object sender, EventArgs e)
        {
            SelectOmniPageEngine();
        }

        private void UpdateMenusForEngine()
        {
            _menuGlyphReaderEngine.Checked = (_engine == _glyphReader);
            _menuTesseract3.Checked = (_engine == _tesseract3);
            _menuTesseract5.Checked = (_engine == _tesseract5);
            _menuOmniPage.Checked = (_engine == _omniPage);
            // Fill in the menu of supported recognition languages/cultures:
            CreateLanguageMenu();
            // Adds the list of supported output formats to the 'Action' menu.
            LoadMimeMenu();
        }

        private void CreateLanguageMenu()
        {
            // build language/culture menu
            var cultures = _engine.GetSupportedRecognitionCultures();
            var names = new StringCollection();
            foreach (var info in cultures)
            {
                names.Add(info.DisplayName);
            }
            // Sort into alphabetical order
            ArrayList.Adapter(names).Sort();
            // Create menu
            EventHandler ev = OnLanguageClick;
            _menuLanguage.MenuItems.Clear();
            var itemsInColumn = 1;
            foreach (var name in names)
            {
                var mi = new MenuItem(name, ev);
                mi.BarBreak = itemsInColumn++ % 40 == 0;
                _menuLanguage.MenuItems.Add(mi);
                if (_engine.RecognitionCulture.DisplayName == name)
                    mi.Checked = true;
            }
        }

        #endregion

        private void OnLanguageClick(object sender, EventArgs e)
        {
            var selecteditem = (MenuItem)sender;
            foreach (MenuItem item in _menuLanguage.MenuItems)
            {
                item.Checked = (item == selecteditem);
            }
            var cultures = _engine.GetSupportedRecognitionCultures();
            foreach (var info in cultures)
            {
                if (info.DisplayName == selecteditem.Text)
                    _engine.RecognitionCulture = info;
            }
        }

        private void OnMenuHelpAboutClick(object sender, EventArgs e)
        {
            var aboutBox = new About(
                "About Atalasoft Simple OCR Demo",
                "DotImage Simple OCR Demo")
            {
                Description =
                    @"Demonstrates the basics of OCR.  This 'no frills' example demonstrates " +
                    "translating an image to a text file or searchable PDF.  The output text " +
                    "style (or mime type) can be formatted as any of the supported types.  " +
                    "This is a great place to get started with DotImage OCR.  " +
                    "Requires evaluation or purchased licenses of DotImage Document Imaging, " +
                    "and at least one of these OCR Add-ons: GlyphReader, OmniPage or Tesseract."
            };
            aboutBox.ShowDialog();
        }

        #endregion

        #region OCR

        private void SelectGlyphReaderEngine()
        {
            if (_glyphReader == null)
            {
                _glyphReader = new GlyphReaderEngine();
                InitializeEngine(_glyphReader);
            }
            if (_glyphReader != null)
            {
                _engine = _glyphReader;
                UpdateMenusForEngine();
            }
        }

        private void SelectTesseract3Engine()
        {
            if (_tesseract3 == null)
            {
                _tesseract3 = new Tesseract3Engine();
                InitializeEngine(_tesseract3);
            }
            if (_tesseract3 != null)
            {
                _engine = _tesseract3;
                UpdateMenusForEngine();
            }
        }

        private void SelectTesseract5Engine()
        {
            if (_tesseract5 == null)
            {
                _tesseract5 = new Tesseract5Engine();
                InitializeEngine(_tesseract5);
            }
            if (_tesseract5 != null)
            {
                _engine = _tesseract5;
                UpdateMenusForEngine();
            }
        }

        private void InitializeEngine(OcrEngine eng)
        {
            eng.Initialize();
            // Add event handler to show translation progress
            eng.PageProgress += OnEnginePageProgress;
            // Add a standard PDF translator
            var pdf = new PdfTranslator { OutputType = PdfTranslatorOutputType.TextUnderImage };
            eng.Translators.Add(pdf);
        }

        // this eventhandler will show the progress of reading each page.
        private void OnEnginePageProgress(object sender, OcrPageProgressEventArgs e)
        {
            _progressBar.Show();
            _progressBar.Value = e.Progress;
        }

        //  This method does the actual translation into text.
        private void DoTranslation()
        {
            if (!_fileLoaded)
            {
                MessageBox.Show("No file loaded... Please open a file and try again.");
            }
            else
            {
                try
                {
                    _textBox.Clear();
                    _outputFile = DefaultOutputFile;

                    // choose output file location, either a temp directory, or a user selected spot.
                    if (_saveToFile)
                        _outputFile = GetSaveFileName();
                    if (_outputFile == null)
                        return;

                    // delete the output file if one already exists
                    if (File.Exists(_outputFile))
                        File.Delete(_outputFile);

                    // OK, we're committed, put up the wait cursor
                    UseWaitCursor = true;
                    Cursor.Current = Cursors.WaitCursor;

                    // this is how the image should be passed to the translator
                    var imageSource = new FileSystemImageSource(TempDir, true);

                    _progressBar.Value = 0;

                    // do the actual translation here.  The text is saved as a file in _outputFile.
                    _engine.Translate(imageSource, _selectedMimeType, _outputFile);

                    if (!_saveToFile)
                    {
                        // Load the text back into the text box for display.
                        var input = new StreamReader(_outputFile);
                        var oneLine = input.ReadLine();
                        while (oneLine != null)
                        {
                            _textBox.AppendText(oneLine);
                            oneLine = input.ReadLine();
                        }
                        input.Close();
                    }
                    else
                    {
                        try
                        {
                            Process.Start(_outputFile);
                        }
                        catch (Exception ex)
                        {
                            if (File.Exists(_outputFile))
                                InfoBox("File \"" + _outputFile + "\" is created.");
                            else
                                InfoBox(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    // if it's a license exception, it's probably because of pdfTranslator
                    if ((ex is AtalasoftLicenseException) && (_selectedMimeType == "application/pdf"))
                        LicenseCheckFailure(
                            "To generate PDF output, an Atalasoft PDF Translator license is required.",
                            ex.Message);
                    else
                        InfoBox(ex.ToString());
                }
                finally
                {
                    UseWaitCursor = false;
                    _progressBar.Hide();
                }
            }
        }

        #endregion

        #region Event handlers

        private void OnSimpleOcrFormLoad(object sender, EventArgs e)
        {
            if (!_validLicense)
                Application.Exit();
        }

        private void OnSimpleOcrFormClosing(object sender, CancelEventArgs e)
        {
            // ShutDown only when the form is being closed.
            if (_engine != null)
                _engine.ShutDown();
        }

        private void OnSimpleOcrFormClosed(object sender, FormClosedEventArgs e)
        {
            if (_tesseract != null)
            {
                _tesseract.ShutDown();
            }
            if (_tesseract3 != null)
            {
                _tesseract3.ShutDown();
            }
            if (_tesseract5 != null)
            {
                _tesseract5.ShutDown();
            }
            if (_glyphReader != null)
            {
                _glyphReader.ShutDown();
            }
        }

        private void OnSplitterMoved(object sender, SplitterEventArgs e)
        {
            _textBox.Width = _splitter.Left;
            _workspaceViewer.Left = _splitter.Left + _splitter.Width;
            _workspaceViewer.Width = ClientSize.Width - _workspaceViewer.Left;
            _progressBar.Left = _workspaceViewer.Left;
            _progressBar.Width = _workspaceViewer.Width;
        }

        #endregion

        #region Boxes

        private void InfoBox(string msg)
        {
            MessageBox.Show(this, msg, AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private DialogResult YesNoBox(string msg)
        {
            return MessageBox.Show(this, msg, AppTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        #endregion
    }
}
