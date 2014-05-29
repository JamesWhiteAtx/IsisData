using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace CST.ISIS.Data
{
//    public class ProdImgInfo
//    {
//        string mime_Type = null;
//        string mime_SubType = null;

//        public ProdImgInfo() { }

//        public ProdImgInfo(IDataRecord dataRecord)
//        {
//            ProdCd = dataRecord["FG_PROD_CD"].ToString();
//            ImgSetCd = dataRecord["FG_IMAGE_SET_CD"].ToString();
//            ImageId = dataRecord["FG_IMAGE_ID"].ToString();
//            mime_Type = dataRecord["FG_CONTENT_TYPE"].ToString();
//            mime_SubType = dataRecord["FG_MIME_TYPE_CD"].ToString();
//            FileName = dataRecord["FG_FILE_NAME"].ToString();
//            ImgType = dataRecord["IMG_TYPE"].ToString();
//        }

//        public string ProdCd { get; set; }
//        public string ImgSetCd { get; set; }
//        public string ImgType { get; set; }
//        public string ImageId { get; set; }
//        public string FileName { get; set; }
//        public string MimeType()
//        {
//            return mime_Type + "/" + mime_SubType;
//        }

//        public static string ProdImgISql()
//        {
//            return
//@"SELECT PIS.FG_PROD_CD,
//PIS.FG_IMAGE_SET_CD,
//PIS.FG_IMAGE_ID,
//II.FG_FILE_NAME,
//II.FG_MIME_TYPE_CD,
//MT.FG_CONTENT_TYPE,
//IMS.FG_DESCRIPTION IMG_TYPE
//FROM FG_IMAGE_INFO II,
//FG_MIME_TYPE MT,
//FG_IMAGE_SET IMS,
//FG_PROD_PRNT_IMG_SET_VW PIS
//WHERE PIS.FG_PROD_CD = :ProdCd
//AND PIS.FG_IMAGE_SET_CD IN ('DRAWING', 'EMBROIDERY', 'HEAT SEAL')
//AND PIS.FG_IMAGE_ID = II.FG_IMAGE_ID
//AND II.FG_MIME_TYPE_CD = MT.FG_MIME_TYPE_CD
//AND PIS.FG_IMAGE_SET_CD = IMS.FG_IMAGE_SET_CD
//ORDER BY PIS.FG_PROD_CD, PIS.FG_SORT_ORDER, PIS.FG_IMAGE_SET_CD";
//        }

//        public static List<ProdImgInfo> ProdImageSet(string prodCd)
//        {
//            var prodImgInfos = new List<ProdImgInfo>();

//            using (DbConnection conn = IsisDataHelper.GetDbConnection("OraIsis"))
//            {
//                using (DbCommand cmd = conn.CreateCommand())
//                {
//                    //ProdImgIQuery query = new ProdImgIQuery(prodCd, setid);
//                    cmd.CommandText = ProdImgISql();
//                    cmd.AddParm("ProdCd", prodCd);
//                    conn.Open();
//                    using (DbDataReader rdr = cmd.ExecuteReader())
//                    {
//                        while (rdr.Read())
//                        {
//                            prodImgInfos.Add(new ProdImgInfo(rdr));
//                        }
//                    }
//                }
//            }
//            return prodImgInfos;
//        }
//    }
}