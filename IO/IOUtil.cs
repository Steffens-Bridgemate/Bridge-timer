using BridgeTimer.Extensions;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BridgeTimer.IO
{
    public static class IOUtil
    {
        public static string? GetFolder(string? defaultFolder = null)
        {
            
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            {
                var withBlock = dialog;
                withBlock.IsFolderPicker = true;
                withBlock.InitialDirectory = defaultFolder;
            }

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                return dialog.FileName;
            else
                return null;

        }

        public static string CreateFilter(string extension, string description)
        {
            if (extension.StartsWith("."))
                extension = Strings.Mid(extension, 2);
            return string.Format("{0} (*.{1})|*.{1}", description, extension);
        }

        public static string CreateFilter(IEnumerable<string> extensions, string description)
        {
            var ensuredExtensions = extensions.EnsurePrefix(".");
            ensuredExtensions = ensuredExtensions.Select(item => " *" + item).ToList();

            var extensionFilter = string.Join(";", ensuredExtensions);

            return string.Format("{0} |{1}", description, extensionFilter);
        }

        public static string GetFile(string title,
                                     string defaultExtension, 
                                     string filter, 
                                     string? defaultFilePath = null, 
                                     string? defaultFolder = null, 
                                     bool multiselection = false)
        {
          
            if (!System.IO.Directory.Exists(defaultFolder))
                defaultFolder = null;

            OpenFileDialog dlg = new OpenFileDialog();
            {
                var withBlock = dlg;
                withBlock.AddExtension = true;
                withBlock.CheckFileExists = true;
                withBlock.CheckPathExists = true;
                withBlock.DefaultExt = defaultExtension.EnsurePrefix(".");
                withBlock.DereferenceLinks = true;
                withBlock.Filter = filter;
                withBlock.InitialDirectory = defaultFolder ?? 
                                             Environment.GetFolderPath( Environment.SpecialFolder.Desktop);
                withBlock.Multiselect = multiselection;
                withBlock.Title = title;
                withBlock.ValidateNames = true;
            }

            var result = dlg.ShowDialog();
            if (!result == true)
                return string.Empty;

            return dlg.FileName;
        }

        public static List<string> GetFiles(string title, string defaultExtension, string filter, string? defaultFilePath = null, string? defaultFolder = null)
        {
            // 'Contract.'Requires(Of ArgumentNullException)(Not String.IsNullOrWhiteSpace(defaultExtension), "Default extension must be at least one non whitespace character.")
            // 'Contract.'Requires(Of ArgumentException)(String.IsNullOrWhiteSpace(defaultFolder) OrElse Directory.Exists(defaultFolder), "The specified default folder does not exist.")

            if (!Directory.Exists(defaultFolder))
                defaultFolder = null;

            OpenFileDialog dlg = new OpenFileDialog();
            {
                var withBlock = dlg;
                withBlock.AddExtension = true;
                withBlock.CheckFileExists = true;
                withBlock.CheckPathExists = true;
                withBlock.DefaultExt = defaultExtension.EnsurePrefix(".");
                withBlock.DereferenceLinks = true;
                withBlock.Filter = filter;
                withBlock.InitialDirectory = defaultFolder ?? 
                                             Environment.GetFolderPath( Environment.SpecialFolder.Desktop);
                withBlock.Multiselect = true;
                withBlock.Title = title;
                withBlock.ValidateNames = true;
            }

            var result = dlg.ShowDialog();
            if (!result == true)
                return new List<string>();

            return dlg.FileNames.ToList();
        }

        /// <summary>
        ///         ''' Returns a file that complies with a multi extension filter.
        ///         ''' </summary>
        ///         ''' <param name="title"></param>
        ///         ''' <param name="extensions">The extensions that the file may have</param>
        ///         ''' <param name="filterText">The text that describes the extensions</param>
        ///         ''' <param name="defaultFilePath"></param>
        ///         ''' <param name="defaultFolder"></param>
        ///         ''' <returns></returns>
        public static string GetFile(string title, IEnumerable<string> extensions, string filterText, string? defaultFilePath = null, string? defaultFolder = null)
        {
            if (extensions == null || extensions.Count() < 1)
                throw new ArgumentNullException(nameof(extensions));

            var ensuredExtensions = extensions.EnsurePrefix(".");

            var filter = CreateFilter(ensuredExtensions, filterText);

            OpenFileDialog dlg = new OpenFileDialog();
            {
                var withBlock = dlg;
                withBlock.AddExtension = true;
                withBlock.CheckFileExists = true;
                withBlock.CheckPathExists = true;
                withBlock.DefaultExt = ensuredExtensions.First();
                withBlock.DereferenceLinks = true;
                withBlock.Filter = filter;
                withBlock.InitialDirectory = defaultFolder ?? 
                                             Environment.GetFolderPath( Environment.SpecialFolder.Desktop);
                withBlock.Multiselect = false;
                withBlock.Title = title;
                withBlock.ValidateNames = true;
            }

            var result = dlg.ShowDialog();
            if (!result == true)
                return string.Empty;

            return dlg.FileName;
        }

        /// <summary>
        ///         ''' Returns the full path the location where the user wants to save the file or nothing if the user cancelled.
        ///         ''' </summary>
        ///         ''' <param name="extension">The extension of the file, will be added it the user omits it.</param>
        ///         ''' <param name="title">The title of the savedialog prompt.</param>
        ///         ''' <param name="proposedFilename">The proposed file name.</param>
        ///         ''' <param name="warnIfNotExists">If true the dialog will ask the user if he wants to create a new file if the file does not exist. Otherwise the file will be created without asking.</param>
        ///         '''<param name="filterDescription">The description for the filter. The function will create a full filterdescription with the extension.</param>
        ///         ''' <returns></returns>
        ///         ''' <remarks>This method removes invalid filename characters from the proposed filename.</remarks>
        public static string? GetSavePath(string extension, string title, string proposedFilename = "", bool warnIfNotExists = false, string filterDescription = "", bool overwritePrompt = true)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            if (proposedFilename.IsNullOrWhiteSpace())
                proposedFilename = "naamloos";

            if (!Directory.Exists(Path.GetDirectoryName(proposedFilename)))
                proposedFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), System.IO.Path.GetFileName(proposedFilename));

            {
                var withBlock = dlg;
                withBlock.AddExtension = true;
                withBlock.CheckFileExists = false;
                withBlock.CheckPathExists = false;
                withBlock.CreatePrompt = false;
                withBlock.DefaultExt = extension.EnsurePrefix(".");
                withBlock.DereferenceLinks = true;
                withBlock.FileName = Path.GetFileName(proposedFilename);
                withBlock.InitialDirectory = Path.GetDirectoryName(proposedFilename);
                withBlock.Filter = extension.IsNullOrWhiteSpace() ? "" : CreateFilter(extension, filterDescription);
                withBlock.OverwritePrompt = overwritePrompt;
                withBlock.Title = title;
                withBlock.ValidateNames = true;
            }

            var success = dlg.ShowDialog();
            if (!success == true)
                return null;

            return dlg.FileName;
        }

        /// <summary>
        ///         ''' Adds the prefix before the string if not present.
        ///         ''' </summary>
        ///         ''' <param name="text">The string to ensure the prefix for</param>
        ///         ''' <param name="prefix">The desired prefix</param>
        public static string EnsurePrefix(this string text, string prefix)
        {
            if (prefix.IsNullOrEmpty())
                throw new ArgumentNullException("Prefix must be at least one character.");

            if (text.StartsWith(prefix))
                return text;
            return prefix + text;
        }

        /// <summary>
        ///         ''' Adds the prefix before each of the strings if not present.
        ///         ''' </summary>
        ///         ''' <param name="texts">The strings to ensure the prefix for</param>
        ///         ''' <param name="prefix">The desired prefix</param>
        public static List<string> EnsurePrefix(this IEnumerable<string> texts, string prefix)
        {
            var ensuredTexts = new List<string>();
            foreach (var txt in texts)
                ensuredTexts.Add(txt.EnsurePrefix(prefix));
            return ensuredTexts;
        }
    }
}
