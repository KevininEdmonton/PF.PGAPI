using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using K.Common;
using DomainRepository.IRepositories;
using PFAPI.SupportModels;
using KS.Library.EFDB;

namespace PFAPI.utility
{
    public static class WebAPIExtentions
    {
        public static void AddAuitInfo(DbContext theContext, string theUserID, string through)
        {
            var entries = theContext.ChangeTracker.Entries().Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));
            if (!entries.HasData())
                return;

            foreach (var entry in entries)
            {
                SetSystemdata(entry.Entity, theUserID, through, (entry.State == EntityState.Added));
            }
        }
        private static void SetSystemdata(object o, string by, string through, bool isforCreate = true, bool setAllChildren = true)
        {
            try
            {
                int count = 1;
                string theby = by.IsNullOrEmptyOrWhiteSpace() ? Guid.Empty.ToString() : by;
                string thethrough = through.IsNullOrEmptyOrWhiteSpace() ? SystemStatics.SYS_SoftwareName : through;
                SetSystemdataRecursive(o, isforCreate, setAllChildren, DateTime.UtcNow, theby, thethrough, ref count);
            }
            catch (Exception)
            {
                return;
            }
        }
        private static void SetSystemdataRecursive(object o, bool isforCreate, bool setAllChildren, DateTime theDateTime, string by, string through, ref int count)
        {
            void SetValue(PropertyInfo thepropertyInfo, object value)
            {
                if (thepropertyInfo == null || value == null)
                    return;
                Type propertyType = thepropertyInfo.PropertyType;
                object propertyVal = Convert.ChangeType(value, propertyType);
                thepropertyInfo.SetValue(o, propertyVal, null);
            }
            //find out the type
            Type type = o.GetType();
            PropertyInfo propertyInfo;

            propertyInfo = type.GetProperty(SystemStatics.SYS_EFPeoperty_LastUpdatedAtUTC);
            SetValue(propertyInfo, theDateTime);

            propertyInfo = type.GetProperty(SystemStatics.SYS_EFPeoperty_LastUpdatedByUserID);
            SetValue(propertyInfo, by);

            propertyInfo = type.GetProperty(SystemStatics.SYS_EFPeoperty_LastUpdatedThrough);
            SetValue(propertyInfo, through);

            string thev = string.Empty;
            propertyInfo = type.GetProperty(SystemStatics.SYS_EFPeoperty_CreatedByUserID);
            if (propertyInfo != null)
                thev = ((propertyInfo.GetValue(o, null)) == null) ? "" : propertyInfo.GetValue(o, null).ToString();

            if (isforCreate || string.IsNullOrEmpty(thev) || string.IsNullOrWhiteSpace(thev))
            {
                propertyInfo = type.GetProperty(SystemStatics.SYS_EFPeoperty_CreatedAtUTC);
                SetValue(propertyInfo, theDateTime);

                propertyInfo = type.GetProperty(SystemStatics.SYS_EFPeoperty_CreatedByUserID);
                SetValue(propertyInfo, by);

                propertyInfo = type.GetProperty(SystemStatics.SYS_EFPeoperty_CreatedThrough);
                SetValue(propertyInfo, through);
            }

            #region handle children entities
            if (!setAllChildren || (count >= 100))  // in case it goes too deep, or become some endless-loop or dead-loop
                return;

            List<PropertyInfo> ChildEntityPropertys = type.GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System.Collections.Generic.ICollection")).ToList();

            if (ChildEntityPropertys != null && ChildEntityPropertys.Count > 0)
            {
                foreach (PropertyInfo apropertyInfo in ChildEntityPropertys)
                {
                    try
                    {
                        System.Collections.IEnumerable list = (System.Collections.IEnumerable)apropertyInfo.GetValue(o, null);

                        if (list == null)
                            continue;

                        bool hasData = false;
                        foreach (var oneData in list)
                        {
                            hasData = true;
                            break;
                        }

                        if (!hasData)
                            continue;

                        count++;
                        foreach (var childObj in list)
                        {
                            SetSystemdataRecursive(childObj, isforCreate, setAllChildren, theDateTime, by, through, ref count);
                        }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }
            #endregion

        }
        public static string GenerateToken(int size = 32)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        //public static bool CanUserHandleArchivedData(ClaimsPrincipal source, IPFClientRepository _repository)
        //{
        //    if (SystemStatics.SYS_Default_QP_IncludeArchivedData)
        //        return SystemStatics.SYS_Default_QP_IncludeArchivedData;

        //    bool result = SystemStatics.SYS_Default_QP_IncludeArchivedData;
        //    if (source == null)
        //        return result;
        //    if (!source.Claims.HasData())
        //        return result;

        //    string theOperationCode = LogicHelper.GetClientConfigValue(_repository, ClientConfigItems.OperationCode4ManageArchivedData);
        //    if(theOperationCode.IsNullOrEmptyOrWhiteSpace()) 
        //        return result;

        //    string theClaim = source.GetClaimValue(theOperationCode);
        //    if (theClaim.IsNullOrEmptyOrWhiteSpace())
        //        return result;
           
        //    return theClaim.MeaningTrue();
        //}
        //public static BGWorkRequest PrepareBGWorkRequest(this ClaimsPrincipal source)
        //{
        //    if (source == null)
        //        return null;
        //    if (!source.Claims.HasData())
        //        return null;

        //    string theClientstring = source.GetClaimValue(SystemStatics.SYS_JWT_User_ClientID);
        //    if (theClientstring.IsNullOrEmptyOrWhiteSpace())
        //        return null;
        //    if (!Guid.TryParse(theClientstring, out Guid theClientID))
        //        return null;

        //    //string uid = source.GetClaimValue(SystemStatics.SYS_JWT_User_sid);
        //    //if (uid.IsNullOrEmptyOrWhiteSpace())
        //    //    return null;
        //    //if (!Guid.TryParse(uid, out Guid theUserID))
        //    //    return null;

        //    BGWorkRequest result = new BGWorkRequest();
        //    result.ClientId = theClientID;
        //    result.User = source;

        //    return result;
        //}
        public static string GetClaimValue(this ClaimsPrincipal source, string claimtype)
        {
            if (source == null)
                return string.Empty;
            if (!source.Claims.HasData())
                return string.Empty;
            Claim theClaim = source.Claims.FirstOrDefault(c => c.Type == claimtype);
            if (theClaim == null)
                return string.Empty;
            return theClaim.Value;
        }
       public static bool CanWorkwithClientAccount(this ClaimsPrincipal source, Guid clientid)
        {
            if (!source.Claims.HasData())
                return false;

            string isRoot = source.GetClaimValue(SystemStatics.SYS_JWT_User_IsRoot);
            if (isRoot.MeaningTrue())
                return true;

            return clientid.ToString().ContentEqual(source.GetClaimValue(SystemStatics.SYS_JWT_User_ClientID));
        }
        //public static bool CanUserWorkwithCompany(ClaimsPrincipal source, Guid companyid, IPFClientRepository clientRepository, out string msg, out ZClientCompany theCompany)
        //{
        //    msg = "";
        //    theCompany = null;
        //    if (!source.Claims.HasData())
        //    {
        //        msg = "Failed to retrieve current user's data;";
        //        return false;
        //    }

        //    string uid = source.GetClaimValue(SystemStatics.SYS_JWT_User_sid);
        //    if (uid.IsNullOrEmptyOrWhiteSpace())
        //    {
        //        msg = "Failed to retrieve current user's sid;";
        //        return false;
        //    }

        //    if (!Guid.TryParse(uid, out Guid tuid))
        //    {
        //        msg = "Current user's sid is not a valid Guid;";
        //        return false;
        //    }

        //    ZClientUserCompany theUserCompany = clientRepository.GetQueryable<ZClientUserCompany>().FirstOrDefault(z => z.UserId == tuid && z.CompanyId==companyid);
        //    if (theUserCompany == null)
        //    {
        //        msg = $"Failed to retrieve ZClientUserCompany for current user with id {uid} and companyid {companyid};";
        //        return false;
        //    }

        //    theCompany = clientRepository.GetById<ZClientCompany>(companyid);
        //    if(theCompany==null|| (!theCompany.IsActive))
        //    {
        //        msg = $"There's no active ZClientCompany with companyid {companyid} exists;";
        //        return false;
        //    }

        //    return true;
        //}

        public static string GetUserIDString(ClaimsPrincipal source, IPFClientRepository repository)
        {
            if (!source.Claims.HasData())
                return string.Empty;

            string uid = source.GetClaimValue(SystemStatics.SYS_JWT_User_sid);
            if (uid.IsNullOrEmptyOrWhiteSpace())
                return string.Empty;

            return uid;
        }

        public static string GetUserEmail(ClaimsPrincipal source, IPFClientRepository repository)
        {
            if (!source.Claims.HasData())
                return string.Empty;

            string uid = source.GetClaimValue(SystemStatics.SYS_JWT_User_sid);
            if (uid.IsNullOrEmptyOrWhiteSpace())
                return string.Empty;

            if (!Guid.TryParse(uid, out Guid tuid))
                return string.Empty;

            ZclientUser theUser = repository.GetById<ZclientUser>(tuid);
            if(theUser==null)
                return string.Empty;            

            return theUser.UserName;
        }
        public static bool VerifyAppKey(string theAppKeyString, string theSalt, string theHashedString)
        {
            if (theHashedString.IsNullOrEmptyOrWhiteSpace() || theAppKeyString.IsNullOrEmptyOrWhiteSpace())
                return false;
            return EnCryptionHelper.Decrypt(theHashedString, theSalt) == theAppKeyString;
        }
        public static string GenerateAppKey(string theAppKeyString, string theSalt)
        {
            if (theAppKeyString.IsNullOrEmptyOrWhiteSpace())
                return string.Empty;
            return EnCryptionHelper.Encrypt(theAppKeyString, theSalt);
        }

        //public static D MaptoData_LookupConvert<S, D>(this S sourceData, D destData, List<CommonMapping> theMapping, IPFClientRepository _repository_clientdb, bool checkIsValidSqlDateTime = false, List<string> ExcludedDestFieldNameList = null) where S : class where D : class
        //{
        //    if (sourceData == null || destData == null || !theMapping.HasData())
        //        return destData;

        //    Type From = typeof(S);
        //    PropertyInfo[] fromproperties = From.GetProperties();
        //    Type TO = typeof(D);
        //    PropertyInfo[] toproperties = TO.GetProperties();

        //    if (!toproperties.HasData() || !fromproperties.HasData())
        //        return destData;

        //    PropertyInfo Fpi, Tpi;
        //    object value;

        //    Guid lookupId = Guid.Empty;
        //    LookUpItem theLookUpItem = null;
        //    foreach (CommonMapping curM in theMapping)
        //    {
        //        if (curM == null || curM.FromPropertyName.IsNullOrEmptyOrWhiteSpace() || curM.ToPropertyName.IsNullOrEmptyOrWhiteSpace())
        //            continue;

        //        // handle ExcludedDestFieldNameList
        //        if (ExcludedDestFieldNameList.HasData())
        //        {
        //            string theOneToExclude = ExcludedDestFieldNameList.FirstOrDefault(p => p.EqualsString(curM.ToPropertyName));
        //            if (!theOneToExclude.IsNullOrEmptyOrWhiteSpace())
        //                continue;
        //        }

        //        Tpi = toproperties.FirstOrDefault(p => p.Name.EqualsString(curM.ToPropertyName));   //TO.GetProperty(curM.ToPropertyName);
        //        if (Tpi == null)
        //            continue;
        //        if (!Tpi.CanWrite) continue;

        //        // Read value
        //        if (curM.ForceDefaultValue)
        //            value = curM.DefaultValue;
        //        else
        //        {
        //            Fpi = fromproperties.FirstOrDefault(p => p.Name.EqualsString(curM.FromPropertyName));   // From.GetProperty(curM.FromPropertyName);
        //            if (Fpi == null || !Fpi.CanRead)
        //                continue;

        //            value = Fpi.GetValue(sourceData, null);
        //        }

        //        if (value == null)
        //            continue;

        //        if (curM.ForceDefaultValue == false && curM.FromPropertyType == TSYSPropertyTypes.Lookup && curM.ConvertFromProprty == true)
        //        {
        //            try
        //            {
        //                lookupId = new Guid(value.ToString());
        //                theLookUpItem = _repository_clientdb.GetById<LookUpItem>(lookupId);
        //                if (theLookUpItem != null && theLookUpItem.Id != Guid.Empty)
        //                {
        //                    value = theLookUpItem.Code;
        //                }
        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //        }

        //        var ParsedValue = Extensions.Parse(Tpi.PropertyType, value, checkIsValidSqlDateTime);
        //        if (ParsedValue == null)
        //            continue;

        //        // Write value to target
        //        Tpi.SetValue(destData, ParsedValue, null);
        //    }

        //    return destData;
        //}

        //public static D MaptoData_LookupConvert_Reverse<S, D>(this S sourceData, D destData, List<CommonMapping> theMapping, IPFClientRepository _repository_clientdb, List<string> validSourceFieldList, out List<string> modifiedFieldList, bool checkIsValidSqlDateTime = false) where S : class where D : class
        //{
        //    modifiedFieldList = new List<string>();
        //    if (sourceData == null || destData == null || !theMapping.HasData() || validSourceFieldList==null || validSourceFieldList.Count==0)
        //        return destData;

        //    Type From = typeof(S);
        //    PropertyInfo[] fromproperties = From.GetProperties();
        //    Type TO = typeof(D);
        //    PropertyInfo[] toproperties = TO.GetProperties();

        //    if (!toproperties.HasData() || !fromproperties.HasData())
        //        return destData;

        //    PropertyInfo Fpi, Tpi;
        //    object value, value1;

        //    Guid lookupId = Guid.Empty;
        //    LookUpItem theLookUpItem = null;
        //    foreach (CommonMapping curM in theMapping)
        //    {
        //        if (curM == null || curM.FromPropertyName.IsNullOrEmptyOrWhiteSpace() || curM.ToPropertyName.IsNullOrEmptyOrWhiteSpace())
        //            continue;

        //        if (!validSourceFieldList.Contains(curM.ToPropertyName))
        //            continue;

        //        Tpi = toproperties.FirstOrDefault(p => p.Name.EqualsString(curM.FromPropertyName));   //TO.GetProperty(curM.ToPropertyName);
        //        if (Tpi == null)
        //            continue;
        //        if (!Tpi.CanWrite) continue;

        //        //// Read value
        //        //if (curM.ForceDefaultValue)
        //        //    value = curM.DefaultValue;
        //        //else
        //        {
        //            Fpi = fromproperties.FirstOrDefault(p => p.Name.EqualsString(curM.ToPropertyName));   // From.GetProperty(curM.FromPropertyName);
        //            if (Fpi == null || !Fpi.CanRead)
        //                continue;

        //            value = Fpi.GetValue(sourceData, null);
        //            value1 = Tpi.GetValue(destData, null);
        //            if (value == value1)
        //                continue;
        //        }


        //        if (curM.ForceDefaultValue == false && curM.FromPropertyType == TSYSPropertyTypes.Lookup && curM.ConvertFromProprty == true)
        //        {
        //            try
        //            {
        //                var DestValue = Tpi.GetValue(destData, null);
        //                lookupId = new Guid(DestValue.ToString());
        //                theLookUpItem = _repository_clientdb.GetById<LookUpItem>(lookupId);
        //                if (theLookUpItem != null && theLookUpItem.Code != value.ToString())
        //                {
        //                    // need to update LookupItem
        //                    LookUpItem newLUI = _repository_clientdb.GetQueryable<LookUpItem>().FirstOrDefault(l => l.LookUpId == theLookUpItem.LookUpId && l.Code == value.ToString());
        //                    if (newLUI != null)
        //                    {
        //                        var ParsedValue = Extensions.Parse(Tpi.PropertyType, newLUI.Id, checkIsValidSqlDateTime);
        //                        if (ParsedValue == null)
        //                            continue;

        //                        // Write value to target
        //                        Tpi.SetValue(destData, ParsedValue, null);
        //                        modifiedFieldList.Add(Tpi.Name);
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //        }
        //        else
        //        {
        //            if (value == null && value1 == null)
        //                continue;

        //            if (value == null)
        //            {
        //                Tpi.SetValue(destData, value, null);
        //            }
        //            else
        //            {
        //                var ParsedValue = Extensions.Parse(Tpi.PropertyType, value, checkIsValidSqlDateTime);
        //                if (ParsedValue == null)
        //                    continue;

        //                if (ParsedValue == value1)
        //                    continue;
        //                if(value1!=null && (ParsedValue.ToString().EqualsString(value1.ToString())))
        //                    continue;

        //                // Write value to target
        //                Tpi.SetValue(destData, ParsedValue, null);                        
        //            }
        //            modifiedFieldList.Add(Tpi.Name);
        //        }
        //    }

        //    return destData;
        //}

        //internal static Tuple<bool, APPKeyVerifyResultModel, string> VerifyAppKey(IPFClientRepository _repository, IConfiguration _config, APPKeyVerifyModel model)
        //{
        //    if (model.AppKey.IsNullOrEmptyOrWhiteSpace() || model.AppName.IsNullOrEmptyOrWhiteSpace())
        //        return new Tuple<bool, APPKeyVerifyResultModel, string>(false, null, "Invalid data: AppKey and AppName must be provided.");
        //    int DevideLoc = model.AppKey.IndexOf(SystemStatics.SYS_Devider);
        //    int length = model.AppKey.Length;
        //    if (DevideLoc == -1)
        //        return new Tuple<bool, APPKeyVerifyResultModel, string>(false, null, "Invalid AppKey.");

        //    string theID = model.AppKey.Substring(0, DevideLoc);
        //    string theAppKeyString = model.AppKey.Substring(DevideLoc + 1, length - DevideLoc - 1);
        //    if (theID.IsNullOrEmptyOrWhiteSpace() || theAppKeyString.IsNullOrEmptyOrWhiteSpace())
        //        return new Tuple<bool, APPKeyVerifyResultModel, string>(false, null, "Invalid AppKey.");

        //    Guid id;
        //    if (!Guid.TryParse(theID, out id))
        //        return new Tuple<bool, APPKeyVerifyResultModel, string>(false, null, "Invalid AppKey.");

        //    ZClientAppkey theAppKey = _repository.GetByIdAsync<ZClientAppkey>(id).Result;
        //    if (theAppKey == null)
        //        return new Tuple<bool, APPKeyVerifyResultModel, string>(false, null, "Invalid AppKey.");

        //    //Encription varify
        //    if (!WebAPIExtentions.VerifyAppKey(theAppKeyString, _config["AuthSettings:SecretKey"] + theAppKey.Id.ToString(), theAppKey.HashedAppkey))
        //        return new Tuple<bool, APPKeyVerifyResultModel, string>(false, null, "Invalid AppKey.");

        //    if (!theAppKey.IsActive)
        //        return new Tuple<bool, APPKeyVerifyResultModel, string>(false, null, "AppKey is not active.");

        //    if (theAppKey.AppName.Trim().ToUpper() != model.AppName.Trim().ToUpper())
        //        return new Tuple<bool, APPKeyVerifyResultModel, string>(false, null, "AppName does not match.");

        //    // return data
        //    APPKeyVerifyResultModel theData = new APPKeyVerifyResultModel();
        //    theData.ClientId = theAppKey.ClientId;
        //    theData.AppName = theAppKey.AppName;
        //    theData.Description = theAppKey.Description;
        //    theData.IsActive = theAppKey.IsActive;
        //    theData.ApplytoBusinessLogic = theAppKey.ApplytoBusinessLogic;
        //    theData.ApplytoFile = theAppKey.ApplytoFile;
        //    theData.ApplytoLog = theAppKey.ApplytoLog;
        //    theData.ApplytoNotification = theAppKey.ApplytoNotification;
        //    theData.ApplytoReport = theAppKey.ApplytoReport;
        //    theData.ApplytoFeatureA = theAppKey.ApplytoFeatureA;
        //    theData.ApplytoFeatureB = theAppKey.ApplytoFeatureB;
        //    theData.ApplytoFeatureC = theAppKey.ApplytoFeatureC;
        //    theData.ApplytoFeatureD = theAppKey.ApplytoFeatureD;
        //    theData.ApplytoFeatureE = theAppKey.ApplytoFeatureE;
        //    theData.ApplytoFeatureF = theAppKey.ApplytoFeatureF;
        //    theData.FeatureAname = theAppKey.FeatureAname;
        //    theData.FeatureBname = theAppKey.FeatureBname;
        //    theData.FeatureCname = theAppKey.FeatureCname;
        //    theData.FeatureDname = theAppKey.FeatureDname;
        //    theData.FeatureEname = theAppKey.FeatureEname;
        //    theData.FeatureFname = theAppKey.FeatureFname;

        //    return new Tuple<bool, APPKeyVerifyResultModel, string>(true, theData, string.Empty);
        //}
    }
}
