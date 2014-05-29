﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Configuration;
using Oracle.DataAccess.Client;
using System.Runtime.InteropServices;

namespace CST.ISIS.Data
{
   
    public static class IsisDataHelper
    {
        public const string ConnectionStr = "OraIsis";

        public static DbConnection GetDbConnection(string connectionString)
        {
            string connString = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
            return new OracleConnection(connString);
        }

        public static void AddParm(this DbCommand cmd, string parameterName, object value)
        {
            ((OracleCommand)cmd).BindByName = true;

            DbParameter dbParameter = cmd.CreateParameter();
            dbParameter.IsNullable = true;
            dbParameter.ParameterName = parameterName;
            if (value != null)
            {
                dbParameter.Value = value;
            }
            else
            {
                dbParameter.Value = DBNull.Value;
            }

            cmd.Parameters.Add(dbParameter);
        }

        public static decimal GetDecimal(this IDataRecord dr, string fieldName)
        {
            return dr.GetDecimal(dr.GetOrdinal(fieldName));
        }

        public static int? GetNullableDecimal(this IDataRecord dr, string fieldName)
        {
            return GetNullableDecimal(dr, dr.GetOrdinal(fieldName));
        }

        public static int? GetNullableDecimal(this IDataRecord dr, int i)
        {
            return dr.IsDBNull(i) ? null : (int?)dr.GetDecimal(i);
        }

        public static int GetNumberInt(this IDataRecord dr, string fieldName)
        {
            return dr.GetNumberInt(dr.GetOrdinal(fieldName));
        }

        public static int GetNumberInt(this IDataRecord dr, int i)
        {
            return Convert.ToInt32( dr.GetDecimal(i) );
        }

        public static int? GetNullableNumberInt(this IDataRecord dr, string fieldName)
        {
            return GetNullableNumberInt(dr, dr.GetOrdinal(fieldName));
        }

        public static int? GetNullableNumberInt(this IDataRecord dr, int i)
        {
            return dr.IsDBNull(i) ? null : (int?)dr.GetNumberInt(i);
        }

        public static decimal SeqNextVal(string seqName) 
        {
            using (DbConnection conn = GetDbConnection(ConnectionStr))
            {
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT " + seqName + ".NEXTVAL FROM DUAL";

                    conn.Open();
                    using (DbDataReader rdr = cmd.ExecuteReader())
                    {
                        rdr.Read();
                        return rdr.GetDecimal(0);
                    }
                }
            }
        }
    }

    public partial class IsisEntities : ObjectContext
    {
        public const string SalesSetid = "SALES";
        public const string CstBu = "CST";
        public const string BoolYNTrue = "Y";
        public const string BoolYNFalse = "N";
        public const string RequestCanceled = "CANCELED";
        public const string LeatherProdTypeCd = "300";

        public static readonly string[] ImgSetCds = { "DRAWING", "EMBROIDERY", "HEAT SEAL" };

        public List<ProdImageInfo> ProdImageInfoSet(string prodCD)
        {
            //var imgSetCds = new string[] { "DRAWING", "EMBROIDERY", "HEAT SEAL" };

            if (String.IsNullOrEmpty(prodCD))
            {
                return new List<ProdImageInfo>();
            }

            var iis = from pis in ProdPrntImageSetVWs
                      where pis.ProdCD == prodCD
                      where ImgSetCds.Contains(pis.ImageSetCD)
                      select new ProdImageInfo
                      {
                          ProdCD = pis.ProdCD,
                          ImageSetCD = pis.ImageSetCD,
                          ImageType = pis.ImageSet.Description,
                          ImageID = pis.ImageID,
                          FileName = pis.ImageInfo.FileName,
                          Description = pis.ImageInfo.Description
                      };

            return iis.ToList();
        }

        public ImageFile ImageFile(decimal imageID)
        {
            var imgFile = (from img in Images
                           where img.ID == imageID
                           select new ImageFile
                           {   ID = img.ID,
                               Description = img.ImageInfo.Description,
                               FileName = img.ImageInfo.FileName,
                               MimeContentType = img.ImageInfo.MimeType.ContentType,
                               MimeSubType = img.ImageInfo.MimeTypeCD,
                               Data = img.ImageData
                           }
                          ).FirstOrDefault();
            
            return imgFile;
        }

        //public IQueryable<ProdImageSet> SalesProdImageSets()
        //{
        //    return from m in ProdImageSets
        //           where m.ProdSetid == SalesSetid
        //           select m;
        //}
       
        #region People Soft 

        #endregion
    }

    public enum OptionType { Characteristic, Component };

    public class CharCompOpt
    {

        public CharCompOpt()
        {
            Overriden = false;
        }

        public virtual void LoadFromDataRecord(IDataRecord dataRecord)
        {
            Level = dataRecord.GetNumberInt("LEVEL");
            Type = OptTypeForString(dataRecord["OPT_TYPE"].ToString());
            TypeCD = dataRecord["TYPE_CD"].ToString();
            TypeDescr = dataRecord["TYPE_DESCR"].ToString();
            OptionCode1 = dataRecord["OPT_CD1"].ToString();
            OptionCode2 = dataRecord["OPT_CD2"].ToString();
            OptionCode3 = dataRecord["OPT_CD3"].ToString();
            OptionDescr = dataRecord["OPT_DESCR"].ToString();
            ParentOptionCode1 = dataRecord["PARENT_OPT_CD1"].ToString();
            ParentOptionCode2 = dataRecord["PARENT_OPT_CD2"].ToString();
            ParentOptionCode3 = dataRecord["PARENT_OPT_CD3"].ToString();
            CharTypeCD = dataRecord["FG_PROD_CHAR_TYPE_CD"].ToString();
            CharInternalName = dataRecord["FG_PCT_INTERNAL_NAME"].ToString();
            CharCD = dataRecord["FG_PROD_CHAR_CD"].ToString();
            CompProdTypeCD = dataRecord["FG_COMP_PROD_TYPE_CD"].ToString();
            CompProdCD = dataRecord["FG_COMP_PROD_CD"].ToString();
            CompProdSetid = dataRecord["FG_COMP_PROD_SETID"].ToString();
            CompSeq = dataRecord.GetNullableNumberInt("FG_SEQUENCE");
            ParentCompProdCD = dataRecord["PARENT_OPT_CD2"].ToString();
            ParentCompProdSetid = dataRecord["PARENT_OPT_CD1"].ToString();
            ParentCompSeq = String.IsNullOrEmpty(ParentOptionCode3) ? (int?)null : Convert.ToInt32(ParentOptionCode3);
            ImgCount = dataRecord.GetNumberInt("IMG_COUNT");
        }

        public const string LeatherPatternTypeCD = "LEATHER PATTERN";
        public const string LeatherPatternOptCD = "PATTERN";
        public const string LeatherColorTypeCD = "LEATHER COLOR";
        public const string InsertOptionTypeCD = "INSERT OPTION";
        public const string Color1ColorOptCD = "COLOR";
        public const string Color2ColorOptCD = "1ST INSERT";
        public const string Color2OptionOptCD = "INSERT OPTION";
        public const string Color3ColorOptCD = "2ND INSERT";

        public const string ProdCDEmbroidery = "600";
        public const string ProdCDEmbrThread = "605";
        public const string ProdCDHeatSeal = "610";
        public const string ProdCDPerforatedInsert = "620";
        public const string ProdCDSewingThread = "640";
        public const string ProdCDPiping = "630";

        public bool Overriden { get; set; }

        public int Level { get; set; }

        public OptionType Type { get; set; }

        public string TypeCD { get; set; }

        public string TypeDescr { get; set; }

        public string OptionCode1 { get; set; }

        public string OptionCode2 { get; set; }

        public string OptionCode3 { get; set; }

        public string OptionDescr { get; set; }

        public string ParentOptionCode1 { get; set; }

        public string ParentOptionCode2 { get; set; }

        public string ParentOptionCode3 { get; set; }

        public string CharTypeCD { get; set; }

        public string CharInternalName { get; set; }

        public string CharCD { get; set; }

        public string CompProdTypeCD { get; set; }

        public string CompProdCD { get; set; }

        public string CompProdSetid { get; set; }

        public int? CompSeq { get; set; }

        public string ParentCompProdCD { get; set; }

        public string ParentCompProdSetid { get; set; }

        public int? ParentCompSeq { get; set; }

        public int ImgCount { get; set; }

        public List<ProdImageInfo> ProdImageInfoSet { get; set; }
        //public List<ProdImageInfo> ProdImageInfoSet { get; set; }

        public void AssignTypeProps()
        {
            if (Type == OptionType.Characteristic)
            {
                CharTypeCD = TypeCD;
                CharInternalName = OptionCode1;
                CharCD = OptionCode3;
            }
            else if (Type == OptionType.Component)
            {
                CompProdTypeCD = TypeCD;
                CompProdCD = OptionCode2;
                CompProdSetid = OptionCode1;
                CompSeq = String.IsNullOrEmpty(OptionCode3) ? (int?)null : Convert.ToInt32(OptionCode3);
            }
            ParentCompProdCD = ParentOptionCode2;
            ParentCompProdSetid = ParentOptionCode1;
            if (String.IsNullOrEmpty(ParentOptionCode3))
            {
                ParentCompSeq = null;
            }
            else
            {
                ParentCompSeq = Convert.ToInt32(ParentOptionCode3); 
            }
        }

        public int CompOptCd3Seq { set {
            CompSeq = value;
            OptionCode3 = CompSeq.ToString(); 
        } }

        public bool IsRoot {
            get { return String.IsNullOrEmpty(ParentOptionCode2); }  
        }

        public virtual bool OverrideMatch(CharCompOpt opt)
        {
            bool match =
            (
                ((!Overriden) && (Type == opt.Type) && (CharTypeCD == opt.CharTypeCD))
                &&
                (
                    (
                        (Type == OptionType.Characteristic)
                        &&
                        (OptionCode1 == opt.OptionCode1)
                        &&
                        (ParentOptionCode1 == opt.ParentOptionCode1)
                        &&
                        (ParentOptionCode2 == opt.ParentOptionCode2)
                    )
                    ||
                    ((Type == OptionType.Component)
                        &&
                        (OptionCode1 == opt.OptionCode1)
                        &&
                        (
                            (String.IsNullOrEmpty(ParentOptionCode1)
                                &&
                                (OptionCode2 == opt.OptionCode2)
                            )
                            ||
                            ((!String.IsNullOrEmpty(ParentOptionCode1))
                                &&
                                (ParentOptionCode1 == opt.ParentOptionCode1)
                                &&
                                (ParentOptionCode2 == opt.ParentOptionCode2)
                            )
                        )

                    )
                )
            );
            return match;
        }

        public static OptionType OptTypeForString(string optTypeString)
        {
            if (optTypeString.Trim().ToUpper().Equals("CHAR"))
            {
                return OptionType.Characteristic;
            }
            else
            {
                return OptionType.Component;
            }
        }

        public static CharCompOpt FindCharTypeIntrnl(List<CharCompOpt> list, CharCompOpt charCompOpt)
        {
            return list.Find(item => item.OverrideMatch(charCompOpt));
        }

        public static IsisEntities IsisDBContext
        {
            get
            {
                if (_isisDbContext == null)
                {
                    _isisDbContext = new IsisEntities("name=IsisEntities");
                }
                return _isisDbContext;
            }
        }

        private static IsisEntities _isisDbContext = null;

    }

    public class ProdOptDefn : CharCompOpt
    {
        public static List<ProdOptDefn> ProdOptions(string prodCD, string parentProdCD)
        {
            List<ProdOptDefn> prodOpts = ProdOptions(prodCD);

            if (parentProdCD != null)
            {
                if (prodOpts == null)
                {
                    prodOpts = new List<ProdOptDefn>();
                }

                List<ProdOptDefn> parentProdOpts = ProdOptions(parentProdCD);
                if (parentProdOpts.Count() > 0)
                {
                    parentProdOpts.AddRange(prodOpts);
                    prodOpts = parentProdOpts;
                }
            }

            return prodOpts;
        }

        public static List<ProdOptDefn> ProdOptions(string prodCD)
        {
            if (prodCD == null)
            {
                return null;
            }

            var prodOptions = new List<ProdOptDefn>();

            using (DbConnection conn = IsisDataHelper.GetDbConnection(IsisDataHelper.ConnectionStr))
            {
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = ProdOptionsSql();
                    cmd.AddParm(":PROD_CD", prodCD);

                    conn.Open();
                    using (DbDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            ProdOptDefn prodOptDefn = new ProdOptDefn();
                            prodOptDefn.LoadFromDataRecord(rdr);
                            if (prodOptDefn.ImgCount > 0)
                            {
                                if (prodOptDefn.Type == OptionType.Component)
                                {
                                    prodOptDefn.ProdImageInfoSet = IsisDBContext.ProdImageInfoSet(prodOptDefn.CompProdCD);     //ProdImgInfo.ProdImageSet(prodOptDefn.CompProdCD);
                                }
                            }
                            prodOptions.Add(prodOptDefn);
                        }
                    }
                }
            }
            return prodOptions;
        }

        public static string ProdOptionsSql()
        {
            return
@"SELECT 
SORT1,
SORT2,
SORT3,
LEVEL,
FG_PROD_CD,
FG_PROD_SETID,
OPT_TYPE,
TYPE_CD,
TYPE_DESCR,
OPT_CD1,
OPT_CD2,
OPT_CD3,
OPT_DESCR,
PARENT_OPT_CD1,
PARENT_OPT_CD2,
PARENT_OPT_CD3,
FG_PROD_ITEM_FLAG,
FG_PROD_CHAR_TYPE_CD,
FG_PCT_INTERNAL_NAME, 
FG_PROD_CHAR_CD,
FG_COMP_PROD_TYPE_CD,
FG_COMP_PROD_CD, 
FG_COMP_PROD_SETID, 
FG_SEQUENCE,
IMG_COUNT
FROM (
  SELECT   
  PV.SORT1,
  PV.SORT2,
  PV.SORT3,
  PV.FG_PROD_CD,
  PV.FG_PROD_SETID,
  PV.OPT_TYPE,
  PV.TYPE_CD,
  PV.TYPE_DESCR,
  PV.OPT_CD1,
  PV.OPT_CD2,
  PV.OPT_CD3,
  PV.OPT_DESCR,
  PV.PARENT_OPT_CD1,
  PV.PARENT_OPT_CD2,
  PV.PARENT_OPT_CD3,
  PV.FG_PROD_ITEM_FLAG,
  PV.FG_PRODUCTION_FLAG,
  PV.FG_PROD_CHAR_TYPE_CD,
  PV.FG_PCT_INTERNAL_NAME, 
  PV.FG_PROD_CHAR_CD,
  PV.FG_COMP_PROD_TYPE_CD,
  PV.FG_COMP_PROD_CD, 
  PV.FG_COMP_PROD_SETID, 
  PV.FG_SEQUENCE,
  PV.IMG_COUNT
  FROM 
  FG_PROD_OPT_VW PV
  WHERE PV.FG_PROD_CD = :PROD_CD
) 
START WITH PARENT_OPT_CD1 IS NULL
AND FG_PRODUCTION_FLAG = 'Y'
AND ((OPT_CD1 NOT IN ('SPECIAL', 'ALL LEATHER')) OR (OPT_CD2 <> 'N'))
CONNECT BY FG_PROD_CD = PRIOR FG_PROD_CD
AND PARENT_OPT_CD1 = PRIOR OPT_CD1
AND PARENT_OPT_CD2 = PRIOR OPT_CD2
AND PARENT_OPT_CD3 = PRIOR OPT_CD3
ORDER BY SORT1, SORT2, SORT3, LEVEL";
        }
    }

    public class InvItemOptDefn : ProdOptDefn
    {
        public static List<ProdOptDefn> ItemOptions(string itemID, string prodCD, string parentProdCD)
        {
            List<ProdOptDefn> itemOpts = ItemOptions(itemID);

            if (prodCD != null)
            {
                if (itemOpts == null)
                {
                    itemOpts = new List<ProdOptDefn>();
                }

                List<ProdOptDefn> prodOpts = ProdOptions(prodCD, parentProdCD);
                if (prodOpts.Count() > 0)
                {
                    if (itemOpts.Count() > 0)
                    {
                        foreach (var iOpt in itemOpts)
                        {
                            foreach (var pOpt in prodOpts.Where(o => !o.Overriden && o.OverrideMatch(iOpt)))
                            {
                                pOpt.Overriden = true;
                            }
                        }
                    };

                    prodOpts.AddRange(itemOpts);
                    itemOpts = prodOpts;
                }
            }

            return itemOpts;
        }

        public static List<ProdOptDefn> ItemOptions(string itemID)
        {
            if (itemID == null)
            {
                return null;
            }

            var itemOptions = new List<ProdOptDefn>();

            using (DbConnection conn = IsisDataHelper.GetDbConnection(IsisDataHelper.ConnectionStr))
            {
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = InvItemOptionsSql();
                    cmd.AddParm(":INV_ITEM_ID", itemID);

                    conn.Open();
                    using (DbDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            InvItemOptDefn itemOptDefn = new InvItemOptDefn();
                            itemOptDefn.LoadFromDataRecord(rdr);
                            if (itemOptDefn.ImgCount > 0)
                            {
                                if (itemOptDefn.Type == OptionType.Component)
                                {
                                    itemOptDefn.ProdImageInfoSet = IsisDBContext.ProdImageInfoSet(itemOptDefn.CompProdCD);     //ProdImgInfo.ProdImageSet(prodOptDefn.CompProdCD);
                                }
                            }
                            itemOptions.Add(itemOptDefn);
                        }
                    }
                }
            }
            return itemOptions;
        }

        public static string InvItemOptionsSql()
        {
            return
@"SELECT
CHARS.FG_INV_ITEM_ID,
61 SORT1,
PCS.FG_SORT_ORDER SORT2,
1 ""LEVEL"",
CHARS.FG_PROD_CD,
CHARS.FG_PROD_SETID,
'CHAR' OPT_TYPE,
CHARS.FG_PROD_CHAR_TYPE_CD TYPE_CD,
CHARS.FG_DISPLAY TYPE_DESCR,
CHARS.FG_PCT_INTERNAL_NAME OPT_CD1,
CHARS.FG_PROD_CHAR_VALUE_CD OPT_CD2,
CHARS.FG_PROD_CHAR_CD OPT_CD3,
CHARS.CHAR_VAL_DESCR OPT_DESCR,
CAST(NULL AS VARCHAR(5)) PARENT_OPT_CD1,
CAST(NULL AS VARCHAR(30)) PARENT_OPT_CD2,
CAST(NULL AS NUMBER) PARENT_OPT_CD3,
PCS.FG_PROD_ITEM_FLAG,
PCS.FG_PRODUCTION_FLAG,
CHARS.FG_PROD_CHAR_TYPE_CD,
CHARS.FG_PCT_INTERNAL_NAME,
CHARS.FG_PROD_CHAR_CD,
CAST(NULL AS VARCHAR(30)) FG_COMP_PROD_TYPE_CD,
CAST(NULL AS VARCHAR(30)) FG_COMP_PROD_CD,
CAST(NULL AS VARCHAR(5)) FG_COMP_PROD_SETID,
CAST(NULL AS NUMBER) FG_SEQUENCE,
0 IMG_COUNT
FROM 
(   SELECT 
    I.FG_INV_ITEM_ID,
    P.FG_PROD_TYPE_CD,
    I.FG_PROD_CD,
    I.FG_PROD_SETID,
    D.FG_PCT_INTERNAL_NAME,
    D.FG_PROD_CHAR_CD,
    D.FG_PARENT_COMP_PROD_CD,
    D.FG_PARENT_COMP_PROD_SETID,
    D.FG_USER_DEFINITION,
    D.FG_PARENT_SEQUENCE,
    PC.FG_PROD_CHAR_TYPE_CD,
    PC.FG_PROD_CHAR_VALUE_CD,
    PC.FG_PC_DESCRIPTION,
    DECODE(PC.FG_USER_DEFINED_FLAG, 'Y', D.FG_USER_DEFINITION, PC.FG_PC_DESCRIPTION) CHAR_VAL_DESCR,
    N.FG_DISPLAY
    FROM
    FG_INTERNAL_NAME N,
    FG_PROD_CHAR PC,
    FG_PROD P,
    FG_INV_DEFN D,
    FG_INV_ITEM I
    WHERE I.FG_INV_ITEM_ID = D.FG_INV_ITEM_ID
    AND I.FG_PROD_CD = P.FG_PROD_CD
    AND I.FG_PROD_SETID = P.FG_PROD_SETID
    AND D.FG_PROD_CHAR_CD = PC.FG_PROD_CHAR_CD
    AND D.FG_PCT_INTERNAL_NAME = N.FG_PCT_INTERNAL_NAME
    AND I.FG_INV_ITEM_ID = :INV_ITEM_ID
) CHARS
JOIN FG_PROD_CHAR_SET PCS ON (
CHARS.FG_PROD_TYPE_CD = PCS.FG_PROD_TYPE_CD
AND CHARS.FG_PROD_CHAR_TYPE_CD = PCS.FG_PROD_CHAR_TYPE_CD
AND CHARS.FG_PCT_INTERNAL_NAME = PCS.FG_PCT_INTERNAL_NAME)
UNION
SELECT
CHARS.FG_INV_ITEM_ID,
2 SORT1,
COMP.FG_SORT_ORDER SORT2,
COMP.LEV+1 ""LEVEL"",
CHARS.FG_PROD_CD,
CHARS.FG_PROD_SETID,
'CHAR' OPT_TYPE,
CHARS.FG_PROD_CHAR_TYPE_CD TYPE_CD,
CHARS.FG_DISPLAY TYPE_DESCR,
CHARS.FG_PCT_INTERNAL_NAME OPT_CD1,
CHARS.FG_PROD_CHAR_VALUE_CD OPT_CD2,
CHARS.FG_PROD_CHAR_CD OPT_CD3,
CHARS.CHAR_VAL_DESCR OPT_DESCR,
COMP.FG_PROD_SETID PARENT_OPT_CD1,
COMP.FG_PROD_CD PARENT_OPT_CD2,
COMP.FG_SEQUENCE PARENT_OPT_CD3,
PCS.FG_PROD_ITEM_FLAG,
PCS.FG_PRODUCTION_FLAG,
CHARS.FG_PROD_CHAR_TYPE_CD,
CHARS.FG_PCT_INTERNAL_NAME,
CHARS.FG_PROD_CHAR_CD,
CAST(NULL AS VARCHAR(30)) FG_COMP_PROD_TYPE_CD,
CAST(NULL AS VARCHAR(30)) FG_COMP_PROD_CD,
CAST(NULL AS VARCHAR(5)) FG_COMP_PROD_SETID,
CAST(NULL AS NUMBER) FG_SEQUENCE,
0 IMG_COUNT
FROM
(   SELECT
    I.FG_INV_ITEM_ID,
    I.FG_PROD_CD,
    I.FG_PROD_SETID,
    D.FG_PCT_INTERNAL_NAME,
    D.FG_PROD_CHAR_CD,
    D.FG_PARENT_COMP_PROD_CD,
    D.FG_PARENT_COMP_PROD_SETID,
    D.FG_USER_DEFINITION,
    D.FG_PARENT_SEQUENCE,
    PC.FG_PROD_CHAR_TYPE_CD,
    PC.FG_PROD_CHAR_VALUE_CD,
    PC.FG_PC_DESCRIPTION,
    DECODE(PC.FG_USER_DEFINED_FLAG, 'Y', D.FG_USER_DEFINITION, PC.FG_PC_DESCRIPTION) CHAR_VAL_DESCR,
    N.FG_DISPLAY
    FROM
    FG_INTERNAL_NAME N,
    FG_PROD_CHAR PC,
    FG_PROD P,
    FG_INV_DEFN D,
    FG_INV_ITEM I
    WHERE I.FG_INV_ITEM_ID = D.FG_INV_ITEM_ID
    AND I.FG_PROD_CD = P.FG_PROD_CD
    AND I.FG_PROD_SETID = P.FG_PROD_SETID
    AND D.FG_PROD_CHAR_CD = PC.FG_PROD_CHAR_CD
    AND D.FG_PCT_INTERNAL_NAME = N.FG_PCT_INTERNAL_NAME
    AND I.FG_INV_ITEM_ID = :INV_ITEM_ID
) CHARS
JOIN ( SELECT
  LEVEL LEV,
  C.FG_INV_ITEM_ID,
  C.FG_PROD_CD,
  C.FG_PROD_SETID,
  C.FG_USER_DEFINITION,
  C.FG_SEQUENCE,
  C.FG_SORT_ORDER,
  P.FG_PROD_TYPE_CD
  FROM FG_INV_ITEM_COMP C
  JOIN FG_PROD P ON (C.FG_PROD_CD = P.FG_PROD_CD
       AND C.FG_PROD_SETID = P.FG_PROD_SETID)
  START WITH C.FG_PARENT_PROD_CD IS NULL
        AND C.FG_PARENT_PROD_SETID IS NULL
        AND C.FG_INV_ITEM_ID = :INV_ITEM_ID
  CONNECT BY C.FG_PARENT_INV_ITEM_ID = PRIOR C.FG_INV_ITEM_ID
          AND C.FG_PARENT_PROD_CD = PRIOR C.FG_PROD_CD
          AND C.FG_PARENT_PROD_SETID = PRIOR C.FG_PROD_SETID
          AND C.FG_PARENT_SEQUENCE = PRIOR C.FG_SEQUENCE
) COMP
ON (CHARS.FG_INV_ITEM_ID = COMP.FG_INV_ITEM_ID
AND CHARS.FG_PARENT_COMP_PROD_CD = COMP.FG_PROD_CD
AND CHARS.FG_PARENT_COMP_PROD_SETID = COMP.FG_PROD_SETID
AND CHARS.FG_PARENT_SEQUENCE = COMP.FG_SEQUENCE)
JOIN FG_PROD_CHAR_SET PCS ON (
COMP.FG_PROD_TYPE_CD = PCS.FG_PROD_TYPE_CD
AND CHARS.FG_PROD_CHAR_TYPE_CD = PCS.FG_PROD_CHAR_TYPE_CD
AND CHARS.FG_PCT_INTERNAL_NAME = PCS.FG_PCT_INTERNAL_NAME)
UNION
SELECT
COMP.FG_INV_ITEM_ID,
2 SORT1,
COMP.FG_SORT_ORDER SORT2,
COMP.LEV ""LEVEL"",
I.FG_PROD_CD,
I.FG_PROD_SETID,
'COMP' OPT_TYPE,
COMP_P.FG_PROD_TYPE_CD TYPE_CD,
COMP_PT.FG_DESCRIPTION TYPE_DESCR,
COMP.FG_PROD_SETID OPT_CD1,
COMP.FG_PROD_CD OPT_CD2,
TO_CHAR(COMP.FG_SEQUENCE) OPT_CD3,
DECODE(COMP_P.FG_USER_TEXT_FLAG, 'Y', COMP.FG_USER_DEFINITION, COMP_P.FG_DESCRIPTION) OPT_DESCR,
COMP.FG_PARENT_PROD_SETID PARENT_OPT_CD1,
COMP.FG_PARENT_PROD_CD PARENT_OPT_CD2,
COMP.FG_PARENT_SEQUENCE PARENT_OPT_CD3,
'B' FG_PROD_ITEM_FLAG,
'Y' FG_PRODUCTION_FLAG,
CAST(NULL AS VARCHAR(50)) FG_PROD_CHAR_TYPE_CD,
CAST(NULL AS VARCHAR(30)) FG_PCT_INTERNAL_NAME,
CAST(NULL AS VARCHAR(30)) FG_PROD_CHAR_CD,
COMP.FG_PROD_TYPE_CD FG_COMP_PROD_TYPE_CD,
COMP.FG_PROD_CD FG_COMP_PROD_CD,
COMP.FG_PROD_SETID FG_COMP_PROD_SETID,
COMP.FG_SEQUENCE,
(SELECT COUNT(*) FROM FG_PROD_IMAGE_SET PIS
WHERE PIS.FG_PROD_CD = COMP.FG_PROD_CD
AND PIS.FG_PROD_SETID = COMP.FG_PROD_SETID) IMG_COUNT
FROM
FG_PROD_TYPE COMP_PT,
FG_PROD COMP_P,
FG_INV_ITEM I,
( SELECT
  LEVEL LEV,
  C.FG_INV_ITEM_ID,
  C.FG_PROD_CD,
  C.FG_PROD_SETID,
  C.FG_USER_DEFINITION,
  C.FG_SEQUENCE,
  C.FG_PARENT_PROD_CD,
  C.FG_PARENT_PROD_SETID,
  C.FG_PARENT_SEQUENCE,
  C.FG_SORT_ORDER,
  P.FG_PROD_TYPE_CD
  FROM FG_INV_ITEM_COMP C
  JOIN FG_PROD P ON (C.FG_PROD_CD = P.FG_PROD_CD AND C.FG_PROD_SETID = P.FG_PROD_SETID)
  START WITH C.FG_PARENT_PROD_CD IS NULL
        AND C.FG_PARENT_PROD_SETID IS NULL
        AND C.FG_INV_ITEM_ID = :INV_ITEM_ID
  CONNECT BY C.FG_PARENT_INV_ITEM_ID = PRIOR C.FG_INV_ITEM_ID
          AND C.FG_PARENT_PROD_CD = PRIOR C.FG_PROD_CD
          AND C.FG_PARENT_PROD_SETID = PRIOR C.FG_PROD_SETID
          AND C.FG_PARENT_SEQUENCE = PRIOR C.FG_SEQUENCE
) COMP
WHERE COMP.FG_INV_ITEM_ID = I.FG_INV_ITEM_ID
AND COMP.FG_PROD_CD = COMP_P.FG_PROD_CD
AND COMP.FG_PROD_SETID = COMP_P.FG_PROD_SETID
AND COMP_P.FG_PROD_TYPE_CD = COMP_PT.FG_PROD_TYPE_CD
ORDER BY SORT1, SORT2, ""LEVEL""";
        }
    }

    public class WorksheetOpt : CharCompOpt
    {
        public override void LoadFromDataRecord(IDataRecord dataRecord)
        {
            base.LoadFromDataRecord(dataRecord);
            WorksheetID = dataRecord.GetNullableDecimal("FG_WORKSHEET_ID");
            UserDefined = dataRecord["USER_DEFINED_FLAG"].ToString().Trim().ToUpper().Equals(IsisEntities.BoolYNTrue);
        }

        public decimal? WorksheetID { get; set; }

        public bool UserDefined { get; set; }

        public virtual DbCommand MakeCommand(DbConnection conn)
        {
            return conn.CreateCommand();
        }

        public static List<WorksheetOpt> Worksheet(decimal? worksheetID)
        {
            return Worksheet<WorksheetOpt>(worksheetID);
        }

        public static List<T> Worksheet<T>(decimal? worksheetID) where T : WorksheetOpt, new()
        {
            var worksheetOpts = new List<T>();

            if (worksheetID != null)
            {
                using (DbConnection conn = IsisDataHelper.GetDbConnection(IsisDataHelper.ConnectionStr))
                {
                    using (DbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = WorksheetSql();
                        cmd.AddParm(":WORKSHEET_ID", worksheetID);

                        conn.Open();
                        using (DbDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                T worksheetOpt = new T();  //WorksheetOpt worksheetOpt = new WorksheetOpt();
                                worksheetOpt.LoadFromDataRecord(rdr);
                                if (worksheetOpt.ImgCount > 0)
                                {
                                    if (worksheetOpt.Type == OptionType.Component)
                                    {
                                        worksheetOpt.ProdImageInfoSet = IsisDBContext.ProdImageInfoSet(worksheetOpt.CompProdCD); //ProdImgInfo.ProdImageSet(worksheetOpt.CompProdCD);
                                    }
                                }
                                worksheetOpts.Add(worksheetOpt);
                            }
                        }
                    }
                }
            }
            return worksheetOpts;
        }

        public static string WorksheetSql()
        {
            return
@"SELECT 
FG_WORKSHEET_ID,
SORT1,
SORT2,
SORT3,
LEVEL,
FG_PROD_CD,
FG_PROD_SETID,
OPT_TYPE,
TYPE_CD,
TYPE_DESCR,
OPT_CD1,
OPT_CD2,
OPT_CD3,
OPT_DESCR,
USER_DEFINED_FLAG,
PARENT_OPT_CD1,
PARENT_OPT_CD2,
PARENT_OPT_CD3,
FG_PROD_CHAR_TYPE_CD,
FG_PCT_INTERNAL_NAME, 
FG_PROD_CHAR_CD,
FG_COMP_PROD_TYPE_CD,
FG_COMP_PROD_CD, 
FG_COMP_PROD_SETID, 
FG_SEQUENCE,
IMG_COUNT
FROM (
  SELECT   
  WV.FG_WORKSHEET_ID,
  WV.SORT1,
  WV.SORT2,
  WV.SORT3,
  WV.FG_PROD_CD,
  WV.FG_PROD_SETID,
  WV.OPT_TYPE,
  WV.TYPE_CD,
  WV.TYPE_DESCR,
  WV.OPT_CD1,
  WV.OPT_CD2,
  WV.OPT_CD3,
  WV.OPT_DESCR,
  WV.USER_DEFINED_FLAG,
  WV.PARENT_OPT_CD1,
  WV.PARENT_OPT_CD2,
  WV.PARENT_OPT_CD3,
  WV.FG_PROD_CHAR_TYPE_CD,
  WV.FG_PCT_INTERNAL_NAME, 
  WV.FG_PROD_CHAR_CD,
  WV.FG_COMP_PROD_TYPE_CD,
  WV.FG_COMP_PROD_CD, 
  WV.FG_COMP_PROD_SETID, 
  WV.FG_SEQUENCE,
  WV.IMG_COUNT
  FROM FG_WORKSHEET_OPT_VW WV
  WHERE WV.FG_WORKSHEET_ID = :WORKSHEET_ID
) 
START WITH PARENT_OPT_CD1 IS NULL
CONNECT BY PARENT_OPT_CD1 = PRIOR OPT_CD1
AND PARENT_OPT_CD2 = PRIOR OPT_CD2
AND PARENT_OPT_CD3 = PRIOR OPT_CD3
ORDER BY SORT1, SORT2, SORT3, LEVEL";
        }

        public static List<WorksheetOpt> ReqWorksheet(string requestID)
        {
            return ReqWorksheet<WorksheetOpt>(requestID);
        }

        public static List<T> ReqWorksheet<T>(string requestID) where T : WorksheetOpt, new()
        {

            var worksheetOpts = new List<T>();

            if (requestID != null)
            {
                using (DbConnection conn = IsisDataHelper.GetDbConnection(IsisDataHelper.ConnectionStr))
                {
                    using (DbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = ReqWorksheetSql();
                        cmd.AddParm(":REQUEST_ID", requestID);

                        conn.Open();
                        using (DbDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                T worksheetOpt = new T();  //WorksheetOpt worksheetOpt = new WorksheetOpt();
                                worksheetOpt.LoadFromDataRecord(rdr);
                                if (worksheetOpt.ImgCount > 0)
                                {
                                    if (worksheetOpt.Type == OptionType.Component)
                                    {
                                        worksheetOpt.ProdImageInfoSet = IsisDBContext.ProdImageInfoSet(worksheetOpt.CompProdCD);    //ProdImgInfo.ProdImageSet(worksheetOpt.CompProdCD);
                                    }
                                }
                                worksheetOpts.Add(worksheetOpt);
                            }
                        }
                    }
                }
            }
            return worksheetOpts;
        }
           
        public static string ReqWorksheetSql()
        {
            return
@"SELECT 
FG_WORKSHEET_ID,
SORT1,
SORT2,
SORT3,
LEVEL,
FG_PROD_CD,
FG_PROD_SETID,
OPT_TYPE,
TYPE_CD,
TYPE_DESCR,
OPT_CD1,
OPT_CD2,
OPT_CD3,
OPT_DESCR,
USER_DEFINED_FLAG,
PARENT_OPT_CD1,
PARENT_OPT_CD2,
PARENT_OPT_CD3,
FG_PROD_CHAR_TYPE_CD,
FG_PCT_INTERNAL_NAME, 
FG_PROD_CHAR_CD,
FG_COMP_PROD_TYPE_CD,
FG_COMP_PROD_CD, 
FG_COMP_PROD_SETID, 
FG_SEQUENCE,
IMG_COUNT
FROM (
  SELECT   
  WV.FG_WORKSHEET_ID,
  WV.SORT1,
  WV.SORT2,
  WV.SORT3,
  WV.FG_PROD_CD,
  WV.FG_PROD_SETID,
  WV.OPT_TYPE,
  WV.TYPE_CD,
  WV.TYPE_DESCR,
  WV.OPT_CD1,
  WV.OPT_CD2,
  WV.OPT_CD3,
  WV.OPT_DESCR,
  WV.USER_DEFINED_FLAG,
  WV.PARENT_OPT_CD1,
  WV.PARENT_OPT_CD2,
  WV.PARENT_OPT_CD3,
  WV.FG_PROD_CHAR_TYPE_CD,
  WV.FG_PCT_INTERNAL_NAME, 
  WV.FG_PROD_CHAR_CD,
  WV.FG_COMP_PROD_TYPE_CD,
  WV.FG_COMP_PROD_CD, 
  WV.FG_COMP_PROD_SETID, 
  WV.FG_SEQUENCE,
  WV.IMG_COUNT
  FROM 
  FG_REQ_PART R,
  FG_WORKSHEET_OPT_VW WV
  WHERE R.FG_WORKSHEET_ID = WV.FG_WORKSHEET_ID
  AND R.FG_REQUEST_ID = :REQUEST_ID
) 
START WITH PARENT_OPT_CD1 IS NULL
CONNECT BY PARENT_OPT_CD1 = PRIOR OPT_CD1
AND PARENT_OPT_CD2 = PRIOR OPT_CD2
AND PARENT_OPT_CD3 = PRIOR OPT_CD3
ORDER BY SORT1, SORT2, SORT3, LEVEL";
        }

        public static List<T> CompSubChars<T>(WorksheetOpt parentCompOpt, List<T> worksheetOpts = null) where T : WorksheetOpt, new()
        {
            if (worksheetOpts == null)
            {
                worksheetOpts = new List<T>();
            }

            using (DbConnection conn = IsisDataHelper.GetDbConnection(IsisDataHelper.ConnectionStr))
            {
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = CompSubCharSql();
                    cmd.AddParm(":PROD_TYPE", parentCompOpt.TypeCD);

                    conn.Open();
                    using (DbDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            T worksheetOpt = new T
                            {
                                Level = parentCompOpt.Level + 1,
                                Type = OptionType.Characteristic,
                                TypeCD = rdr["FG_PROD_CHAR_TYPE_CD"].ToString(),
                                TypeDescr = rdr["FG_DISPLAY"].ToString(),
                                OptionCode1 = rdr["FG_PCT_INTERNAL_NAME"].ToString(),
                                OptionCode2 = null,
                                OptionCode3 = null,
                                OptionDescr = null,
                                ParentOptionCode1 = parentCompOpt.OptionCode1,
                                ParentOptionCode2 = parentCompOpt.OptionCode2,
                                ParentOptionCode3 = parentCompOpt.OptionCode3,
                                ImgCount = 0,
                                //WorksheetID = dataRecord["FG_WORKSHEET_ID"].ToString();
                                UserDefined = false
                            };
                            worksheetOpts.Add(worksheetOpt);
                        }
                    }
                }
            }

            return worksheetOpts;
        }

        public static string CompSubCharSql()
        {
            return
@"SELECT 
PCS.FG_PROD_TYPE_CD,
PCS.FG_PROD_CHAR_TYPE_CD,
PCS.FG_PCT_INTERNAL_NAME,
PCS.FG_SORT_ORDER,
PCS.FG_REQUIRED_FLAG,
PCS.FG_UNIQUE_FLAG,
PCS.FG_PROD_ITEM_FLAG,
N.FG_DISPLAY,
ROWNUM
FROM FG_INTERNAL_NAME N, FG_PROD_CHAR_SET PCS
WHERE PCS.FG_PCT_INTERNAL_NAME = N.FG_PCT_INTERNAL_NAME
AND PCS.FG_PROD_TYPE_CD = :PROD_TYPE
AND FG_PROD_ITEM_FLAG IN ('I','B')
ORDER BY FG_SORT_ORDER";
        }

        public static List<T> CompSubComps<T>(WorksheetOpt parentCompOpt, List<T> worksheetOpts = null) where T : WorksheetOpt, new()
        {
            if (worksheetOpts == null)
            {
                worksheetOpts = new List<T>();
            }

            using (DbConnection conn = IsisDataHelper.GetDbConnection(IsisDataHelper.ConnectionStr))
            {
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = CompSubCompSql();
                    cmd.AddParm(":PROD_TYPE", parentCompOpt.TypeCD);
                    cmd.AddParm(":PROD_CD", parentCompOpt.OptionCode2);
                    cmd.AddParm(":SETID", parentCompOpt.OptionCode1);

                    conn.Open();
                    using (DbDataReader rdr = cmd.ExecuteReader())
                    {
                        int seq = (parentCompOpt.CompSeq == null) ? 1 : (int)parentCompOpt.CompSeq;
                        while (rdr.Read())
                        {
                            int qty = (int)rdr.GetDecimal(rdr.GetOrdinal("FG_QUANTITY"));
                            for (int i = 0; i < qty; i++)
                            {
                                seq++;
                                T worksheetOpt = new T
                                {
                                    Level = parentCompOpt.Level + 1,
                                    Type = OptionType.Component,
                                    TypeCD = rdr["FG_PROD_TYPE_CD"].ToString(),
                                    TypeDescr = rdr["FG_DESCRIPTION"].ToString(),
                                    OptionCode1 = rdr["FG_PROD_SETID"].ToString(),
                                    OptionCode2 = null,
                                    CompOptCd3Seq = seq,
                                    OptionDescr = null,
                                    ParentOptionCode1 = parentCompOpt.OptionCode1,
                                    ParentOptionCode2 = parentCompOpt.OptionCode2,
                                    ParentOptionCode3 = parentCompOpt.OptionCode3,
                                    ImgCount = 0,
                                    //WorksheetID = dataRecord["FG_WORKSHEET_ID"].ToString();
                                    UserDefined = false
                                };
                                worksheetOpts.Add(worksheetOpt);
                            }
                        }
                    }
                }
            }

            return worksheetOpts;
        }

        public static string CompSubCompSql()
        {
            return
@"SELECT
NVL(COMPS.FG_SORT_ORDER, OPTS.FG_SORT_ORDER) FG_SORT_ORDER,
COMPS.FG_PROD_CD,
COMPS.FG_PROD_SETID,
NVL(COMPS.FG_PROD_TYPE_CD, OPTS.FG_PROD_TYPE_CD) FG_PROD_TYPE_CD,
NVL(COMPS.FG_QUANTITY, OPTS.FG_QUANTITY) FG_QUANTITY,
NVL(COMPS.FG_DESCRIPTION, OPTS.FG_DESCRIPTION) FG_DESCRIPTION
FROM (
SELECT 
  O.FG_COMP_PROD_TYPE_CD FG_PROD_TYPE_CD,
  O.FG_QUANTITY,
  O.FG_SORT_ORDER,
  CPT.FG_DESCRIPTION
  FROM FG_PROD_TYPE CPT, FG_PROD_TYPE_OPTIONS O
  WHERE O.FG_COMP_PROD_TYPE_CD = CPT.FG_PROD_TYPE_CD
  AND O.FG_PROD_TYPE_CD = :PROD_TYPE
) OPTS,
(  SELECT PTC.FG_PROD_CD,
  PTC.FG_PROD_SETID,
  PTC.FG_PROD_TYPE_CD,
  PTC.FG_QUANTITY,
  PTC.FG_SORT_ORDER,
  PT.FG_DESCRIPTION
  FROM FG_PROD_TYPE PT, FG_PROD_TYPE_COMP PTC
  WHERE PTC.FG_PROD_TYPE_CD = PT.FG_PROD_TYPE_CD
  AND  PTC.FG_PROD_CD = :PROD_CD
  AND PTC.FG_PROD_SETID = :SETID
) COMPS
WHERE OPTS.FG_PROD_TYPE_CD = COMPS.FG_PROD_TYPE_CD (+)
ORDER BY 1";
        }
    }

    public class ProdImageInfo
    {
        public string ProdCD { get; set; }
        public string ImageSetCD { get; set; }
        public string ImageType { get; set; }
        public decimal ImageID { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
    }

    public class Attachment
    {
        public decimal ID { get; set; }

        public string Description { get; set; }

        public string FileName { get; set; }

        public string MimeContentType { get; set; }

        public string MimeSubType { get; set; }

        public static string DefaultMimeType = System.Net.Mime.MediaTypeNames.Application.Octet;

        public static char MimeSeperator = '/';

        public static string ConcatMimeType(string contentType, string subType)
        { 
            return contentType.ToLower() + MimeSeperator + subType.ToLower();;
        }

        public string MimeType
       {
           get
           {
               if (!String.IsNullOrEmpty(MimeContentType) && !String.IsNullOrEmpty(MimeSubType))
               {
                   return ConcatMimeType(MimeContentType, MimeSubType);
               }
               else
               {
                   string mimeType = null;

                   if (!String.IsNullOrEmpty(FileName))
                   {
                       mimeType = GetRegistryMimeFromName(FileName);
                   }

                   if (mimeType == null)
                   {
                       mimeType = GetMimeFromBytes(Data);
                   }

                   if (mimeType == null)
                   {
                       mimeType = DefaultMimeType;
                   }
                   return mimeType;
               }
           }

           set {
               string[] mimeParts = value.Split(MimeSeperator);
               if (mimeParts.Length > 0)
               {
                   MimeContentType = mimeParts[0];    
               } else {
                   MimeContentType = null;
               }

               if (mimeParts.Length > 1)
               {
                   MimeSubType = mimeParts[1];
               } else {
                   MimeSubType = null;
               }
           }
       }

        public byte[] Data { get; set; }
       
           [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
           private extern static System.UInt32 FindMimeFromData(
               System.UInt32 pBC,
               [MarshalAs(UnmanagedType.LPStr)] System.String pwzUrl,
               [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
               System.UInt32 cbSize,
               [MarshalAs(UnmanagedType.LPStr)] System.String pwzMimeProposed,
               System.UInt32 dwMimeFlags,
               out System.UInt32 ppwzMimeOut,
               System.UInt32 dwReserverd
           );

        public static int MimeSampleSize = 256;

        public static string GetMimeFromBytes(byte[] data)
       {
           try
           {
               uint mimeType;
               FindMimeFromData(0, null, data, (uint)MimeSampleSize, null, 0, out mimeType, 0);

               var mimePointer = new IntPtr(mimeType);
               var mime = Marshal.PtrToStringUni(mimePointer);
               Marshal.FreeCoTaskMem(mimePointer);

               return mime;
           }
           catch
           {
               return null;
           }
       }

        private static string GetRegistryMimeFromName(string Filename)
       {
           string mime = null;
           string ext = System.IO.Path.GetExtension(Filename).ToLower();
           Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
           if (rk != null && rk.GetValue("Content Type") != null)
               mime = rk.GetValue("Content Type").ToString();
           return mime;
       }
    }

    public class ImageFile : Attachment
   { 
   }
}
