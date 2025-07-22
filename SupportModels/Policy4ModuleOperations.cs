namespace PFAPI.SupportModels
{
    public static class Policy4ModuleOperations
    {
        public static class P_AccountAccessLevel
        {
            public const string AccessLevel_Root = "AL_999";

            public const string AccessLevel_Admin = "AL_50";

            public const string AccessLevel_EveryOne = "AL_0";

            public static List<string> GetOperationList()
            {
                return new List<string> { "AL_999", "AL_50", "AL_0" };
            }
        }

        public static class DAC_DataSecurityLevel
        {
            public const int SecurityLevel5 = 5;

            public const int SecurityLevel4 = 4;

            public const int SecurityLevel3 = 3;

            public const int SecurityLevel2 = 2;

            public const int SecurityLevel1 = 1;

            public const int EveryValidUser = 0;

            public const int Public = -1;
        }

        public static class P_Admin
        {
            public const string ModuleCode = "A";

            private const string OperationPrefix = "A_";

            public const string AdminAccess = "A";

            public const string AdminManage = "M";

            public const string ClientAdmin = "CLA";

            public const string AdminAccessOperation = "A_A";

            public const string AdminManageOperation = "A_M";

            public const string ClientAdminOperation = "A_CLA";

            public static List<string> GetOperationList()
            {
                return new List<string> { "A_A", "A_M", "A_CLA" };
            }
        }

        public const string PermissionSelected = "T";

        public const string ModuleOperationDevider = "_";

        public static List<string> GetAllOperationList()
        {
            List<string> allModuleOperationList = GetAllModuleOperationList();
            allModuleOperationList.AddRange(P_AccountAccessLevel.GetOperationList());
            return allModuleOperationList;
        }

        public static List<string> GetAllModuleOperationList()
        {
            List<string> list = new List<string>();
            list.AddRange(P_Admin.GetOperationList());
            //list.AddRange(P_Report.GetOperationList());
            //list.AddRange(P_Document.GetOperationList());
            //list.AddRange(P_Build.GetOperationList());
            //list.AddRange(P_Invoice.GetOperationList());
            //list.AddRange(P_Production.GetOperationList());
            //list.AddRange(P_Sales.GetOperationList());
            //list.AddRange(P_Spork.GetOperationList());
            //list.AddRange(P_ProjectManagement.GetOperationList());
            return list;
        }
    }
}
