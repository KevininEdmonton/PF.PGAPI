using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PFAPI.utility
{
    public static class SystemStatics
    {
        public const string
            SYS_Quote = "\"",
            SYS_Devider = ".",

            SYS_EFPeoperty_ID = "Id",
            SYS_EFPeoperty_CreatedByUserID = "CreatedByUserId",
            SYS_EFPeoperty_CreatedAtUTC = "CreatedAtUtc",
            SYS_EFPeoperty_CreatedThrough = "CreatedThrough",
            SYS_EFPeoperty_LastUpdatedByUserID = "LastUpdatedByUserId",
            SYS_EFPeoperty_LastUpdatedAtUTC = "LastUpdatedAtUtc",
            SYS_EFPeoperty_LastUpdatedThrough = "LastUpdatedThrough",
            SYS_EFPeoperty_ValidFrom = "ValidFrom",
            SYS_EFPeoperty_ValidTo = "ValidTo",
            SYS_SoftwareName = "PF API",

            SYS_JWT_User_ClientID = "UClientID",
            SYS_JWT_User_IsRoot = "UIsRoot",
            SYS_JWT_User_IsAdmin = "UIsAdmin",
            SYS_JWT_User_sid = "sid",

            SYS_QueryParameter_Filter = "qp_filter",     // string: 
            SYS_QueryParameter_IncludeAllChildrenData = "qp_includeallchildrendata",     // bool: true or false    default false
            SYS_QueryParameter_IncludeData = "qp_includedata",                           // string : {includechildrenname}:{theninclude}:{theninclude}, {includechildrenname}:{theninclude}:{theninclude}
            SYS_QueryParameter_Page = "qp_page",   // int: 1,2,3 .... default 1
            SYS_QueryParameter_Pagesize = "qp_pagesize",   // int: ...default 10
            SYS_QueryParameter_Orderby = "qp_orderby";   //  string: default "ValidFrom"; {orderby fieldname}, {thenorderby fieldname}, {thenorderby fieldname}, {thenorderby fieldname}
                                                         //SYS_QueryParameter_Ascending = "qp_ascending"   // bool: true or false    default false


        public const string SYS_ApplicationName = "PF API"
            , Route_GetAll_KTopic = "Route_GetAll_KTopic"
            , Route_GetOne_KTopic = "Route_GetOne_KTopic"

            , Route_GetAll_KTopicComment = "Route_GetAll_KTopicComment"
            , Route_GetOne_KTopicComment = "Route_GetOne_KTopicComment"

            ;
        public const string SYS_Default_QP_Filter = "";

        public const string SYS_Default_QP_IncludeData = "";

        public const string SYS_Default_QP_Orderby = "";

        public const int SYS_Default_QP_Page = 1;

        public const int SYS_Default_QP_Pagesize = 10;

        public const int SYS_Default_QP_PagesizeMax = 100;

        public const bool SYS_Default_QP_IncludeAllChildrenData = false;

        public const bool SYS_Default_QP_IncludeArchivedData = false;

        public const string NT_Entity_FrontWrap = "{[|";

        public const string NT_Entity_EndWrap = "|]}";

        public const string NT_Entity_Divider = ":";

        public const string NT_System_FrontWrap = "{[~";

        public const string NT_System_EndWrap = "~]}";
    }

    public static class EnumHelper<T>
    {
        public static IDictionary<string, T> GetValues(bool ignoreCase = true)
        {
            Dictionary<string, T> dictionary = new Dictionary<string, T>();
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo fieldInfo in fields)
            {
                string text = fieldInfo.Name;
                if (fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), inherit: false) is DisplayAttribute[] array)
                {
                    text = ((array.Length != 0) ? array[0].Name : fieldInfo.Name);
                }

                if (ignoreCase)
                {
                    text = text.ToLower();
                }

                if (!dictionary.ContainsKey(text))
                {
                    dictionary[text] = (T)fieldInfo.GetRawConstantValue();
                }
            }

            return dictionary;
        }

        public static T GetDefaultValue()
        {
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public);
            return (T)fields[0].GetRawConstantValue();
        }

        public static T Parse(string value, bool ignoreCase = true)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value, ignoreCase);
            }
            catch (Exception)
            {
                return ParseDisplayValues(value, ignoreCase);
            }
        }

        public static bool TryParse(string value, out T result, bool ignoreCase = true)
        {
            try
            {
                result = (T)Enum.Parse(typeof(T), value, ignoreCase);
            }
            catch
            {
                try
                {
                    result = ParseDisplayValues(value, ignoreCase);
                }
                catch
                {
                    result = GetDefaultValue();
                    return false;
                }
            }

            return true;
        }

        public static T ParseDisplayValues(string value, bool ignoreCase = true)
        {
            IDictionary<string, T> values = GetValues(ignoreCase);
            string text = null;
            text = ((!ignoreCase) ? value : value.ToLower());
            if (values.ContainsKey(text))
            {
                return values[text];
            }

            throw new ArgumentException(value);
        }
    }
}
