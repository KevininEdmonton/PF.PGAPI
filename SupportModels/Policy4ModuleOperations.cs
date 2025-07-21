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

        public static class P_Report
        {
            public const string ModuleCode = "R";

            private const string OperationPrefix = "R_";

            public const string ReportAccess = "A";

            public const string ReportAccessOperation = "R_A";

            public static List<string> GetOperationList()
            {
                return new List<string> { "R_A" };
            }
        }

        public static class P_Document
        {
            public const string ModuleCode = "D";

            private const string OperationPrefix = "D_";

            public const string DocumentDataAccess = "A";

            public const string DocumentDataManage = "M";

            public const string DocumentConfigManage = "C";

            public const string DocuemntDataAccessOperation = "D_A";

            public const string DocuemntDataManageOperation = "D_M";

            public const string DocuemntConfigManageOperation = "D_C";

            public static List<string> GetOperationList()
            {
                return new List<string> { "D_A", "D_M", "D_C" };
            }
        }

        public static class P_Build
        {
            public const string ModuleCode = "B";

            private const string OperationPrefix = "B_";

            public const string BuildAccess = "A";

            public const string Plan = "P";

            public const string Moduleconfig = "M";

            public const string Prepare = "R";

            public const string ManageTemplate = "MT";

            public const string StageDelayNote = "ESDN";

            public const string ViewPlanCalendar = "VPC";

            public const string BuildAccessOperation = "B_A";

            public const string BuildPlanOperation = "B_P";

            public const string BuildModuleconfigOperation = "B_M";

            public const string BuildPrepareOperation = "B_R";

            public const string BuildManageTemplatesOperation = "B_MT";

            public const string StageDelayNoteOperation = "B_ESDN";

            public const string ViewPlanCalendarOperation = "B_VPC";

            public static List<string> GetOperationList()
            {
                return new List<string> { "B_A", "B_P", "B_M", "B_R", "B_MT", "B_ESDN", "B_VPC" };
            }
        }

        public static class P_Invoice
        {
            public const string ModuleCode = "I";

            private const string OperationPrefix = "I_";

            public const string InvoiceAccess = "A";

            public const string InvoiceSubmit = "S";

            public const string POAccess = "P";

            public const string POApproval = "O";

            public const string InvoiceApproval = "I";

            public const string InvoiceCongfig = "C";

            public const string InvoiceAccessOperation = "I_A";

            public const string InvoiceSubmitOperation = "I_S";

            public const string POAccessOperation = "I_P";

            public const string POApprovalOperation = "I_O";

            public const string InvoiceApprovalOperation = "I_I";

            public const string InvoiceConfigOperation = "I_C";

            public static List<string> GetOperationList()
            {
                return new List<string> { "I_A", "I_S", "I_P", "I_O", "I_I", "I_C" };
            }
        }

        public static class P_Production
        {
            public const string ModuleCode = "P";

            private const string OperationPrefix = "P_";

            public const string ProductionAccess = "A";

            public const string ProductionAccessOperation = "P_A";

            public static List<string> GetOperationList()
            {
                return new List<string> { "P_A" };
            }
        }

        public static class P_Sales
        {
            public const string ModuleCode = "S";

            private const string OperationPrefix = "S_";

            public const string SalesAccess = "A";

            public const string SalesTemplateAccess = "TA";

            public const string SalesTemplateManage = "TM";

            public const string SalesTemplateAvailability = "TAV";

            public const string SalesBaseTemplateCreate = "BTC";

            public const string SalesBaseTemplateDelete = "BTD";

            public const string SalesTemplateImageManage = "TIM";

            public const string SalesTemplatePricingManage = "TPM";

            public const string SalesConfigurationAccess = "CA";

            public const string SalesTaxRateManage = "TRM";

            public const string SalesRegionManage = "RM";

            public const string SalesProductLibraryAccess = "PLA";

            public const string SalesProductManage = "PM";

            public const string SalesStyleManage = "SM";

            public const string SalesProductGroupManage = "PGM";

            public const string SalesProductLibraryPricingManage = "PLPM";

            public const string SalesDocumentAccess = "DA";

            public const string SalesDocumentManage = "DM";

            public const string SalesDashboardAccess = "DBA";

            public const string SalesBuildingConfigurationManage = "BCM";

            public const string SalesSaleStatusManage = "SSM";

            public const string SalesLotandUnitJobPricingManage = "LUPM";

            public const string SalesMaxVarianceAccess = "MVA";

            public const string SalesAllowableVarianceAccess = "AVA";

            public const string SalesAdvertisedVarianceAccess = "ADVA";

            public const string SalesAccessOperation = "S_A";

            public const string SalesTemplateAccessOperation = "S_TA";

            public const string SalesTemplateManageOperation = "S_TM";

            public const string SalesTemplateAvailabilityOperation = "S_TAV";

            public const string SalesBaseTemplateCreateOperation = "S_BTC";

            public const string SalesBaseTemplateDeleteOperation = "S_BTD";

            public const string SalesTemplateImageManageOperation = "S_TIM";

            public const string SalesTemplatePricingManageOperation = "S_TPM";

            public const string SalesConfigurationAccessOperation = "S_CA";

            public const string SalesTaxRateManageOperation = "S_TRM";

            public const string SalesRegionManageOperation = "S_RM";

            public const string SalesProductLibraryAccessOperation = "S_PLA";

            public const string SalesProductManageOperation = "S_PM";

            public const string SalesStyleManageOperation = "S_SM";

            public const string SalesProductGroupManageOperation = "S_PGM";

            public const string SalesProductLibraryPricingManageOperation = "S_PLPM";

            public const string SalesDocumentAccessOperation = "S_DA";

            public const string SalesDocumentManageOperation = "S_DM";

            public const string SalesDashboardAccessOperation = "S_DBA";

            public const string SalesSaleStatusManageOperation = "S_SSM";

            public const string SalesBuildingConfigurationManageOperation = "S_BCM";

            public const string SalesLotandUnitJobPricingManageOperation = "S_LUPM";

            public const string SalesMaxVarianceAccessOperation = "S_MVA";

            public const string SalesAllowableVarianceAccessOperation = "S_AVA";

            public const string SalesAdvertisedVarianceAccessOperation = "S_ADVA";

            public static List<string> GetOperationList()
            {
                return new List<string>
            {
                "S_A", "S_TA", "S_TM", "S_TAV", "S_BTC", "S_BTD", "S_TIM", "S_TPM", "S_CA", "S_TRM",
                "S_RM", "S_PLA", "S_PM", "S_SM", "S_PGM", "S_PLPM", "S_DA", "S_DM", "S_DBA", "S_SSM",
                "S_BCM", "S_LUPM", "S_MVA", "S_AVA", "S_ADVA"
            };
            }
        }

        public static class P_Spork
        {
            public const string ModuleCode = "X";

            private const string OperationPrefix = "X_";

            public const string SporkAccess = "A";

            public const string SporkAccessOperation = "X_A";

            public static List<string> GetOperationList()
            {
                return new List<string> { "X_A" };
            }
        }

        public static class P_ProjectManagement
        {
            public const string ModuleCode = "PM";

            private const string OperationPrefix = "PM_";

            public const string ProjectManagementAccess = "A";

            public const string ProjectManagementRead = "R";

            public const string ProjectManagementEdit = "E";

            public const string ProjectManagementBulkUpload = "BU";

            public const string ProjectManagementBulkDownload = "BD";

            public const string ProjectManagementArchiveJob = "AR";

            public const string ProjectManagementAccessOperation = "PM_A";

            public const string ProjectManagementReadOperation = "PM_R";

            public const string ProjectManagementEditOperation = "PM_E";

            public const string ProjectManagementBulkUploadOperation = "PM_BU";

            public const string ProjectManagementBulkDownloadOperation = "PM_BD";

            public const string ProjectManagementArchiveJobOperation = "PM_AR";

            public static List<string> GetOperationList()
            {
                return new List<string> { "PM_A", "PM_R", "PM_E", "PM_BU", "PM_BD", "PM_AR" };
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
            list.AddRange(P_Report.GetOperationList());
            list.AddRange(P_Document.GetOperationList());
            list.AddRange(P_Build.GetOperationList());
            list.AddRange(P_Invoice.GetOperationList());
            list.AddRange(P_Production.GetOperationList());
            list.AddRange(P_Sales.GetOperationList());
            list.AddRange(P_Spork.GetOperationList());
            list.AddRange(P_ProjectManagement.GetOperationList());
            return list;
        }
    }
}
