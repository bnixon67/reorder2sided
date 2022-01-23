/*
Copyright 2022 Bill Nixon
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
    http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReorderPDF
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // expect one command line argument for filename
            if (args.Length != 1)
            {
                Console.WriteLine("usage: ReorderPDF filename");
                return;
            }

            // open file as PDF Document
            string srcFileName = args[0];
            PdfDocument srcPdfDoc = new PdfDocument(new PdfReader(srcFileName));

            // get number of pages
            int numPages = srcPdfDoc.GetNumberOfPages();

            // expect even number of pages
            if ((numPages % 2) != 0)
            {
                Console.WriteLine("error: odd number of pages");
                return;
            }

            // empty page list
            List<int> pagesToCopy = new List<int>();

            // create reordered page list, e.g. for a 4 page document, reorder as 1,4,2,3
            for (int i = 1; i <= (numPages / 2); i++)
            {
                pagesToCopy.Add(i);
                pagesToCopy.Add(numPages - i + 1);
            }
            
            // parse srcFileName into dirName, fileName without extension, and extension
            string dirName = Path.GetDirectoryName(srcFileName);
            string fileNameNoExt = Path.GetFileNameWithoutExtension(srcFileName);
            string ext = Path.GetExtension(srcFileName);

            // create dstFileName by adding -reorder to filename
            var dstFileName = Path.Combine(dirName, fileNameNoExt + "-reorder" + ext);

            // create new PDF document and copy pages
            PdfDocument dest = new PdfDocument(new PdfWriter(dstFileName));
            srcPdfDoc.CopyPagesTo(pagesToCopy, dest);
            dest.Close();

            // show output
            string newPageOrder = String.Join(", ", pagesToCopy);
            Console.WriteLine($"Reordered pages {newPageOrder} from {srcFileName} to {dstFileName}");
        }
    }
}
