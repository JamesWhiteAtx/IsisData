using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.Entity;
using CST.ISIS.Data;

namespace ConsoleTestIsisData
{
    class Program
    {
        static void Main(string[] args)
        {
            ImageFile();
            //ImgInfos();
            //Seq();
        }
        
        static void ImageFile()
        {
            using (IsisEntities ctx = new IsisEntities())
            {

                var imgFile = (from img in ctx.Images
                            where img.ID == 5735
                            select new ImageFile
                            {
                                ID = img.ID,
                                Description = img.ImageInfo.Description,
                                FileName = img.ImageInfo.FileName,
                                MimeContentType = img.ImageInfo.MimeType.ContentType,
                                MimeSubType = img.ImageInfo.MimeTypeCD,
                                Data = img.ImageData
                            }
                           ).FirstOrDefault();
                      

                Console.WriteLine();
                Console.ReadLine();
            }
        }
        
        static void ImgInfos()
        {
            using (IsisEntities ctx = new IsisEntities())
            {

                List<ProdImageInfo> iis = ctx.ProdImageInfoSet("1122");

                Console.WriteLine();
                Console.ReadLine();                
            }
        }

        private static void Seq()
        {
            decimal s = IsisDataHelper.SeqNextVal("FG_WORKSHEET_ID_SEQ");
            Console.WriteLine(s);
            Console.ReadLine();
        }
    }
}
