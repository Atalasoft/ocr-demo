// ------------------------------------------------------------------------------------
// <copyright file="MainForm.Designer.cs" company="Atalasoft">
//     (c) 2000-2024 Atalasoft, a Kofax Company. All rights reserved. Use is subject to license terms.
// </copyright>
// ------------------------------------------------------------------------------------

using System;
using System.Windows.Forms;
using Atalasoft.Imaging.WinControls;

namespace Atalasoft.Demo.Ocr
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new MainForm());
        }


        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._textBox = new System.Windows.Forms.TextBox();
            this._mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this._menuFile = new System.Windows.Forms.MenuItem();
            this._menuFileOpen = new System.Windows.Forms.MenuItem();
            this._menuFileExit = new System.Windows.Forms.MenuItem();
            this._menuAction = new System.Windows.Forms.MenuItem();
            this._menuActionResult = new System.Windows.Forms.MenuItem();
            this._menuActionDisplay = new System.Windows.Forms.MenuItem();
            this._menuActionSave = new System.Windows.Forms.MenuItem();
            this._menuActionTranslate = new System.Windows.Forms.MenuItem();
            this._menuEngine = new System.Windows.Forms.MenuItem();
            this._menuGlyphReaderEngine = new System.Windows.Forms.MenuItem();
            this._menuTesseract3 = new System.Windows.Forms.MenuItem();
            this._menuTesseract5 = new System.Windows.Forms.MenuItem();
            this._menuOmniPage = new System.Windows.Forms.MenuItem();
            this._menuLanguage = new System.Windows.Forms.MenuItem();
            this._menuHelp = new System.Windows.Forms.MenuItem();
            this._menuHelpAbout = new System.Windows.Forms.MenuItem();
            this._workspaceViewer = new Atalasoft.Imaging.WinControls.WorkspaceViewer();
            this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._splitter = new System.Windows.Forms.Splitter();
            this.SuspendLayout();
            // 
            // _textBox
            // 
            this._textBox.Dock = System.Windows.Forms.DockStyle.Left;
            this._textBox.Location = new System.Drawing.Point(0, 0);
            this._textBox.Multiline = true;
            this._textBox.Name = "_textBox";
            this._textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._textBox.Size = new System.Drawing.Size(374, 649);
            this._textBox.TabIndex = 2;
            // 
            // _mainMenu
            // 
            this._mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._menuFile,
            this._menuAction,
            this._menuEngine,
            this._menuLanguage,
            this._menuHelp});
            // 
            // _menuFile
            // 
            this._menuFile.Index = 0;
            this._menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._menuFileOpen,
            this._menuFileExit});
            this._menuFile.Text = "File";
            // 
            // _menuFileOpen
            // 
            this._menuFileOpen.Index = 0;
            this._menuFileOpen.Text = "Open...";
            this._menuFileOpen.Click += new System.EventHandler(this.OnMenuFileOpenClick);
            // 
            // _menuFileExit
            // 
            this._menuFileExit.Index = 1;
            this._menuFileExit.Text = "Exit";
            this._menuFileExit.Click += new System.EventHandler(this.OnMenuFileExitClick);
            // 
            // _menuAction
            // 
            this._menuAction.Index = 1;
            this._menuAction.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._menuActionResult,
            this._menuActionTranslate});
            this._menuAction.Text = "Action";
            // 
            // _menuActionResult
            // 
            this._menuActionResult.Index = 0;
            this._menuActionResult.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._menuActionDisplay,
            this._menuActionSave});
            this._menuActionResult.Text = "Result";
            this._menuActionResult.Click += new System.EventHandler(this.OnMenuActionOcrClick);
            // 
            // _menuActionDisplay
            // 
            this._menuActionDisplay.Checked = true;
            this._menuActionDisplay.Index = 0;
            this._menuActionDisplay.RadioCheck = true;
            this._menuActionDisplay.Text = "Displays in Text Box";
            this._menuActionDisplay.Click += new System.EventHandler(this.OnMenuActionOcrClick);
            // 
            // _menuActionSave
            // 
            this._menuActionSave.Index = 1;
            this._menuActionSave.RadioCheck = true;
            this._menuActionSave.Text = "Saves to File";
            this._menuActionSave.Click += new System.EventHandler(this.OnMenuActionOcrClick);
            // 
            // _menuActionTranslate
            // 
            this._menuActionTranslate.Index = 1;
            this._menuActionTranslate.Text = "Translate ...";
            this._menuActionTranslate.Click += new System.EventHandler(this.OnMenuActionTranslateClick);
            // 
            // _menuEngine
            // 
            this._menuEngine.Index = 2;
            this._menuEngine.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._menuGlyphReaderEngine,
            this._menuTesseract3,
            this._menuTesseract5,
            this._menuOmniPage});
            this._menuEngine.Text = "Engine";
            // 
            // _menuGlyphReaderEngine
            // 
            this._menuGlyphReaderEngine.Index = 0;
            this._menuGlyphReaderEngine.Text = "GlyphReader";
            this._menuGlyphReaderEngine.Click += new System.EventHandler(this.OnMenuGlyphReaderEngineClick);
            // 
            // _menuTesseract3
            // 
            this._menuTesseract3.Index = 1;
            this._menuTesseract3.Text = "Tesseract 3";
            this._menuTesseract3.Click += new System.EventHandler(this.OnMenuTesseract3Click);
            // 
            // _menuTesseract5
            // 
            this._menuTesseract5.Index = 2;
            this._menuTesseract5.Text = "Tesseract 5";
            this._menuTesseract5.Click += new System.EventHandler(this.OnMenuTesseract5Click);
            // 
            // _menuOmniPage
            // 
            this._menuOmniPage.Index = 3;
            this._menuOmniPage.Text = "OmniPage";
            this._menuOmniPage.Click += new System.EventHandler(this.OnMenuOmniPageClick);
            // 
            // _menuLanguage
            // 
            this._menuLanguage.Index = 3;
            this._menuLanguage.Text = "Language";
            // 
            // _menuHelp
            // 
            this._menuHelp.Index = 4;
            this._menuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._menuHelpAbout});
            this._menuHelp.Text = "Help";
            // 
            // _menuHelpAbout
            // 
            this._menuHelpAbout.Index = 0;
            this._menuHelpAbout.Text = "About ...";
            this._menuHelpAbout.Click += new System.EventHandler(this.OnMenuHelpAboutClick);
            // 
            // _workspaceViewer
            // 
            this._workspaceViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._workspaceViewer.AntialiasDisplay = Atalasoft.Imaging.WinControls.AntialiasDisplayMode.ReductionOnly;
            this._workspaceViewer.AutoZoom = Atalasoft.Imaging.WinControls.AutoZoomMode.BestFitShrinkOnly;
            this._workspaceViewer.DisplayProfile = null;
            this._workspaceViewer.Location = new System.Drawing.Point(390, 0);
            this._workspaceViewer.Magnifier.BackColor = System.Drawing.Color.White;
            this._workspaceViewer.Magnifier.BorderColor = System.Drawing.Color.Black;
            this._workspaceViewer.Magnifier.Size = new System.Drawing.Size(100, 100);
            this._workspaceViewer.Name = "_workspaceViewer";
            this._workspaceViewer.OutputProfile = null;
            this._workspaceViewer.Selection = null;
            this._workspaceViewer.Size = new System.Drawing.Size(629, 617);
            this._workspaceViewer.TabIndex = 3;
            this._workspaceViewer.Text = "workspaceViewer1";
            // 
            // _progressBar
            // 
            this._progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._progressBar.Location = new System.Drawing.Point(390, 623);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(629, 16);
            this._progressBar.TabIndex = 4;
            this._progressBar.Visible = false;
            // 
            // _splitter
            // 
            this._splitter.Location = new System.Drawing.Point(374, 0);
            this._splitter.Name = "_splitter";
            this._splitter.Size = new System.Drawing.Size(10, 649);
            this._splitter.TabIndex = 5;
            this._splitter.TabStop = false;
            this._splitter.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.OnSplitterMoved);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1027, 649);
            this.Controls.Add(this._splitter);
            this.Controls.Add(this._progressBar);
            this.Controls.Add(this._workspaceViewer);
            this.Controls.Add(this._textBox);
            this.Menu = this._mainMenu;
            this.Name = "MainForm";
            this.Text = "Atalasoft Simple OCR Demo";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.OnSimpleOcrFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnSimpleOcrFormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox _textBox;
        private MainMenu _mainMenu;
        private WorkspaceViewer _workspaceViewer;
        private OpenFileDialog _openFileDialog;
        private MenuItem _menuAction;
        private ProgressBar _progressBar;
        private MenuItem _menuFile;
        private MenuItem _menuFileOpen;
        private MenuItem _menuFileExit;
        private MenuItem _menuHelp;
        private MenuItem _menuHelpAbout;
        private MenuItem _menuActionResult;
        private MenuItem _menuActionTranslate;
        private MenuItem _menuActionSave;
        private MenuItem _menuActionDisplay;
        private MenuItem _menuEngine;
        private MenuItem _menuGlyphReaderEngine;
        private Splitter _splitter;
        private MenuItem _menuLanguage;
        private MenuItem _menuTesseract3;
        private MenuItem _menuTesseract5;
        private MenuItem _menuOmniPage;
    }
}
