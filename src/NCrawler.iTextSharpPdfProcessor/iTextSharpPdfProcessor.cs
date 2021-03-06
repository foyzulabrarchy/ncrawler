﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

using NCrawler.Extensions;
using NCrawler.Interfaces;
using NCrawler.Utils;

namespace NCrawler.iTextSharpPdfProcessor
{
	public class iTextSharpPdfProcessor : IPipelineStep
	{
		#region IPipelineStep Members

		public Task ProcessAsync(ICrawler crawler, PropertyBag propertyBag)
		{
			AspectF.Define.
				NotNull(crawler, "crawler").
				NotNull(propertyBag, "propertyBag");

			if (propertyBag.StatusCode != HttpStatusCode.OK)
			{
				return Task.CompletedTask;
			}

			if (!IsPdfContent(propertyBag.ContentType))
            {
                return Task.CompletedTask;
            }

			using (var input = propertyBag.GetResponse())
			{
				var pdfReader = new PdfReader(input);
				try
				{
                    if (pdfReader.Info.TryGetValue("Title", out var title))
                    {
                        propertyBag.Title = Convert.ToString(title, CultureInfo.InvariantCulture).Trim();
                    }

                    var textExtractionStrategy = new SimpleTextExtractionStrategy();
					propertyBag.Text = Enumerable.Range(1, pdfReader.NumberOfPages).
						Select(pageNumber => PdfTextExtractor.GetTextFromPage(pdfReader, pageNumber, textExtractionStrategy)).
						Join(Environment.NewLine);
				}
				finally
				{
					pdfReader.Close();
				}
            }

            return Task.CompletedTask;
        }

		#endregion

		#region Class Methods

		private static bool IsPdfContent(string contentType)
		{
			return contentType.StartsWith("application/pdf", StringComparison.OrdinalIgnoreCase);
		}

		#endregion
	}
}