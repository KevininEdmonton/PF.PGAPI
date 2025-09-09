using KS.Library.EFDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PFAPI.SupportModels
{
    public class ClientUserwithValidOperations
    {
        public Guid ClientAccountID { get; set; }
        public Guid ClientUserID { get; set; }
        public bool IsAccountRoot { get; set; }
        public bool IsAccountAdmin { get; set; }
        public bool IsLocked { get; set; }
        public bool IsEnabled { get; set; }
        public string UserName { get; set; }
        public List<ClientUserOperations> ValidOperations { get; set; }
        public List<string> PermissionCodes { get; set; }
      // public List<ModuleUIInfoModel> ModuleOperationsInfo { get; set; }

        public ClientUserwithValidOperations(ZclientUser theClientUser)
        {
            PermissionCodes = new List<string>();
            ValidOperations = new List<ClientUserOperations>();
            ClientAccountID = theClientUser.ClientId;
            ClientUserID = theClientUser.Id;
            IsAccountRoot = theClientUser.IsAccountRoot;
            IsAccountAdmin = theClientUser.IsAccountAdmin;
            IsLocked = theClientUser.IsLocked;
            IsEnabled = theClientUser.IsEnabled;
            UserName = theClientUser.UserName;
     //      ModuleOperationsInfo = new List<ModuleUIInfoModel>();
        }
    }

    public class ClientUserOperations
    {
        public bool IsValidOperation { get { return IsRoleEnabled && IsRoleLinktoOperationEnabled && IsOperationEnabled && IsModuleEnabled; } }
        public string OperationPermissionCode { get {
                if (!IsValidOperation)
                    return "N/A";
                return (ZZModuleCode + "_" + ZZOperationCode).ToUpper();
            } }

        public Guid UserLinktoRoleID { get; set; }
        public Guid RoleID { get; set; }
        public bool IsRoleEnabled { get; set; }
        public string RoleCode { get; set; }
        public Guid RoleLinktoOperationID { get; set; }
        public bool IsRoleLinktoOperationEnabled { get; set; }
        public Guid OperationID { get; set; }
        public bool IsOperationEnabled { get; set; }
        public string OperationDisplayName { get; set; }
        public Guid ModuleID { get; set; }
        public bool IsModuleEnabled { get; set; }
        public string ModuleDisplayName { get; set; }
        public Guid ZZOperationID { get; set; }
        public string ZZOperationCode { get; set; }
        public Guid ZZModuleID { get; set; }
        public string ZZModuleCode { get; set; }

    }
}
