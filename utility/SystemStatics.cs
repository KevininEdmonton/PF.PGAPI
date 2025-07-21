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
            SYS_SoftwareName = "HoBO API",

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
    }
}
