﻿// ------------------------------------------------------------------------------------
// <copyright file="WinDemoHelperMethods.cs" company="Atalasoft">
//     (c) 2000-2024 Atalasoft, a Kofax Company. All rights reserved. Use is subject to license terms.
// </copyright>
// ------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Atalasoft.Demo.Ocr.Properties;
using Atalasoft.Imaging;
using Atalasoft.Imaging.Codec;
using Atalasoft.Imaging.Codec.Pdf;

namespace Atalasoft.Demo.Ocr
{
    /// <summary>
    /// A collection of static methods.
    /// </summary>
    public sealed class WinDemoHelperMethods
    {
        private static readonly List<ImageFormatInformation> DecoderImageFormats = new List<ImageFormatInformation>();
        private static readonly List<ImageFormatInformation> EncoderImageFormats = new List<ImageFormatInformation>();

        /// <summary>
        /// Initializes static members of the <see cref="WinDemoHelperMethods"/> class.
        /// </summary>
        static WinDemoHelperMethods()
        {
            // Decoders
            AddDecoderFormat(new JpegDecoder(), "Joint Photographic Experts Group (*.jpg)", "*.jpg");
            AddDecoderFormat(new PngDecoder(), "Portable Network Graphic (*.png)", "*.png");
            AddDecoderFormat(new TiffDecoder(), "Tagged Image File (*.tif, *.tiff)", "*.tif;*.tiff");
            AddDecoderFormat(new PcxDecoder(), "ZSoft PaintBrush (*.pcx)", "*.pcx");
            AddDecoderFormat(new TgaDecoder(), "Truevision Targa (*.tga)", "*.tga");
            AddDecoderFormat(new BmpDecoder(), "Windows Bitmap (*.bmp)", "*.bmp");
            AddDecoderFormat(new WmfDecoder(), "Windows Meta File (*.wmf)", "*.wmf");
            AddDecoderFormat(new EmfDecoder(), "Enhanced Windows Meta File (*.emf)", "*.emf");
            AddDecoderFormat(new PsdDecoder(), "Adobe (tm) Photoshop format (*.psd)", "*.psd");
            AddDecoderFormat(new WbmpDecoder(), "Wireless Bitmap (*.wbmp)", "*.wbmp");
            AddDecoderFormat(new GifDecoder(), "Graphics Interchange Format (*.gif)", "*.gif");
            AddDecoderFormat(new TlaDecoder(), "Smaller Animals TLA (*.tla)", "*.tla");
            AddDecoderFormat(new PcdDecoder(), "Kodak (tm) PhotoCD (*.pcd)", "*.pcd");

            // Encoders
            AddEncoderFormat(new JpegEncoder(), "Joint Photographic Experts Group (*.jpg)", "*.jpg");
            AddEncoderFormat(new PngEncoder(), "Portable Network Graphic (*.png)", "*.png");
            AddEncoderFormat(new TiffEncoder(), "Tagged Image File (*.tif, *.tiff)", "*.tif;*.tiff");
            AddEncoderFormat(new PcxEncoder(), "ZSoft PaintBrush (*.pcx)", "*.pcx");
            AddEncoderFormat(new TgaEncoder(), "Truevision Targa (*.tga)", "*.tga");
            AddEncoderFormat(new BmpEncoder(), "Windows Bitmap (*.bmp)", "*.bmp");
            AddEncoderFormat(new WmfEncoder(), "Windows Meta File (*.wmf)", "*.wmf");
            AddEncoderFormat(new EmfEncoder(), "Enhanced Windows Meta File (*.emf)", "*.emf");
            AddEncoderFormat(new PsdEncoder(), "Adobe (tm) Photoshop format (*.psd)", "*.psd");
            AddEncoderFormat(new WbmpEncoder(), "Wireless Bitmap (*.wbmp)", "*.wbmp");
            AddEncoderFormat(new GifEncoder(), "Graphics Interchange Format (*.gif)", "*.gif");
            AddEncoderFormat(new TlaEncoder(), "Smaller Animals TLA (*.tla)", "*.tla");
            AddSeperatelyLicensedEncoderFormat(new PdfEncoder(), "PDF (*.pdf)", "*.pdf");
        }

        /// <summary>
        /// Creates the filter string for open and save dialogs.
        /// </summary>
        /// <param name="isOpenDialog">Set to true if this filter is for an open dialog.</param>
        /// <returns>The filter string for an open or save dialog.</returns>
        public static string CreateDialogFilter(bool isOpenDialog)
        {
            var filter = new StringBuilder();

            if (isOpenDialog)
            {
                // All supported formats
                filter.Append("All Supported Images|");
                foreach (var info in DecoderImageFormats)
                    filter.Append(info.Filter + ";");

                // Individual format filter
                foreach (var info in DecoderImageFormats)
                    filter.Append("|" + info.Description + "|" + info.Filter);

                // Add all files filter, this will cover, e.g. Dicom files without extension
                filter.Append("|All files (*.*)|*.*");
            }
            else
            {
                foreach (var info in EncoderImageFormats)
                    filter.Append("|" + info.Description + "|" + info.Filter);

                filter.Append("|Animated GIF (*.gif)|*.gif");
                filter.Remove(0, 1); // Remove leading "|"
            }

            return filter.ToString();
        }

        /// <summary>
        /// Populates a given DecoderCollection object with all currently registered decoders.
        /// </summary>
        /// <param name="col">The decoders collection to populate.</param>
        public static void PopulateDecoders(DecoderCollection col)
        {
            foreach (var info in DecoderImageFormats)
                if (!col.Contains(info.Decoder))
                    col.Add(info.Decoder);
        }

        private static void AddDecoderFormat(ImageDecoder decoder, string description, string filter)
        {
            DecoderImageFormats.Add(new ImageFormatInformation(decoder, description, filter));
        }

        private static void AddEncoderFormat(ImageEncoder encoder, string description, string filter)
        {
            EncoderImageFormats.Add(new ImageFormatInformation(encoder, description, filter));
            }

        private static void AddSeperatelyLicensedEncoderFormat(ImageEncoder encoder, string description, string filter)
        {
            try
            {
                EncoderImageFormats.Add(new ImageFormatInformation(encoder, description, filter));
            }
            catch (AtalasoftLicenseException e)
            {
                MessageBox.Show(null, string.Format(@"{0}; {1}", encoder, e), Resources.WinDemoHelperMethods_CoderLicenseError_Caption);
            }
        }

        /// <summary>
        /// Image information structure.
        /// </summary>
        private struct ImageFormatInformation
        {
            public readonly string Filter;
            public readonly string Description;
            public readonly ImageEncoder Encoder;
            public readonly ImageDecoder Decoder;

            /// <summary>
            /// Initializes a new instance of the <see cref="ImageFormatInformation"/> struct.
            /// </summary>
            /// <param name="encoder">The encoder.</param>
            /// <param name="description">The description.</param>
            /// <param name="filter">The filter.</param>
            public ImageFormatInformation(ImageEncoder encoder, string description, string filter)
            {
                Encoder = encoder;
                Decoder = null;
                Description = description;
                Filter = filter;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ImageFormatInformation"/> struct.
            /// </summary>
            /// <param name="decoder">The decoder.</param>
            /// <param name="description">The description.</param>
            /// <param name="filter">The filter.</param>
            public ImageFormatInformation(ImageDecoder decoder, string description, string filter)
            {
                Decoder = decoder;
                Encoder = null;
                Description = description;
                Filter = filter;
            }
        }
    }
}