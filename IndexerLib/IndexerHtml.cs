﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using HelpersLib;
using IndexerLib.Properties;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace IndexerLib
{
    public class IndexerHtml : Indexer
    {
        public IndexerHtml(IndexerSettings indexerSettings)
            : base(indexerSettings)
        {
        }

        public override string Index(string folderPath)
        {
            StringBuilder sbHtmlIndex = new StringBuilder();
            sbHtmlIndex.AppendLine(Resources.doctype_xhtml);
            sbHtmlIndex.AppendLine(HtmlHelper.Tag("title", "Index for " + Path.GetFileName(folderPath)));
            sbHtmlIndex.AppendLine(HtmlHelper.GetCssStyle(config.CssFilePath));
            sbHtmlIndex.AppendLine(HtmlHelper.EndTag("head"));
            sbHtmlIndex.AppendLine(HtmlHelper.StartTag("body"));
            string index = base.Index(folderPath).Trim();
            sbHtmlIndex.AppendLine(index);
            sbHtmlIndex.AppendLine(HtmlHelper.StartTag("div") + GetFooter() + HtmlHelper.EndTag("div"));
            if (config.AddValidationIcons) sbHtmlIndex.AppendLine(Resources.valid_xhtml);
            sbHtmlIndex.AppendLine(HtmlHelper.EndTag("body"));
            sbHtmlIndex.AppendLine(HtmlHelper.EndTag("html"));
            return sbHtmlIndex.ToString().Trim();
        }

        protected override void IndexFolder(FolderInfo dir, int level)
        {
            sbContent.AppendLine(GetFolderNameRow(dir, level));

            string divClass = level > 0 ? "FolderBorder" : "MainFolderBorder";
            sbContent.AppendLine(HtmlHelper.StartTag("div", "", "class=\"" + divClass + "\""));

            if (dir.Files.Count > 0)
            {
                sbContent.AppendLine(HtmlHelper.StartTag("ul"));

                foreach (FileInfo fi in dir.Files)
                {
                    sbContent.AppendLine(GetFileNameRow(fi, level));
                }

                sbContent.AppendLine(HtmlHelper.EndTag("ul"));
            }

            foreach (FolderInfo subdir in dir.Folders)
            {
                IndexFolder(subdir, level + 1);
            }

            sbContent.AppendLine(HtmlHelper.EndTag("div"));
        }

        protected override string GetFolderNameRow(FolderInfo dir, int level)
        {
            int heading = (level + 1).Between(1, 6);

            string folderInfoText = string.Empty;

            if (!dir.IsEmpty)
            {
                folderInfoText = dir.Size.ToSizeString(config.BinaryUnits) + " (";

                if (dir.TotalFileCount > 0)
                {
                    folderInfoText += dir.TotalFileCount + " file" + (dir.TotalFileCount > 1 ? "s" : "");
                }

                if (dir.TotalFolderCount > 0)
                {
                    if (dir.TotalFileCount > 0)
                    {
                        folderInfoText += ", ";
                    }

                    folderInfoText += dir.TotalFolderCount + " folder" + (dir.TotalFolderCount > 1 ? "s" : "");
                }

                folderInfoText += ")";
                folderInfoText = "  " + HtmlHelper.Tag("span", folderInfoText, "", "class=\"folderinfo\"");
            }

            return HtmlHelper.StartTag("h" + heading) + Helpers.HtmlEncode(dir.FolderName) + folderInfoText + HtmlHelper.EndTag("h" + heading);
        }

        protected override string GetFileNameRow(FileInfo fi, int level)
        {
            string size = " " + HtmlHelper.Tag("span", fi.Length.ToSizeString(config.BinaryUnits), "", "class=\"filesize\"");

            return HtmlHelper.StartTag("li") + Helpers.HtmlEncode(fi.Name) + size + HtmlHelper.EndTag("li");
        }

        protected override string GetFooter()
        {
            return string.Format("Generated by {0} on {1}.", string.Format("<a href=\"{0}\">{1}</a>", Links.URL_WEBSITE,
                string.Format("{0} v{1}", Application.ProductName, Application.ProductVersion)),
                DateTime.UtcNow.ToString("yyyy-MM-dd 'at' HH:mm:ss 'UTC'"));
        }
    }
}