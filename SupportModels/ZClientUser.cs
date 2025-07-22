namespace PFAPI.SupportModels
{
    public class ZClientUser
    {
        public Guid Id { get; set; }

        public Guid ClientId { get; set; }

        public string IdentityId { get; set; } = null;


        public string UserName { get; set; } = null;


        public string FirstName { get; set; } = null;


        public string LastName { get; set; } = null;


        public string? MiddleName { get; set; }

        public bool IsAccountRoot { get; set; }

        public bool IsAccountAdmin { get; set; }

        public bool IsLocked { get; set; }

        public bool IsEnabled { get; set; }

        public string CreatedByUserId { get; set; } = null;


        public DateTime CreatedAtUtc { get; set; }

        public string CreatedThrough { get; set; } = null;


        public string LastUpdatedByUserId { get; set; } = null;


        public DateTime LastUpdatedAtUtc { get; set; }

        public string LastUpdatedThrough { get; set; } = null;


        public string UserType { get; set; } = null;


        //public virtual ZClientAccount Client { get; set; } = null;


        //public virtual AspNetUser Identity { get; set; } = null;


        //public virtual ICollection<ProjectJob> ProjectJobProjectMangers { get; set; }

        //public virtual ICollection<ProjectJob> ProjectJobSalesLeads { get; set; }

        //public virtual ICollection<ProjectJob> ProjectJobSiteSupervisors { get; set; }

        //public virtual ICollection<ProjectStakeHolder> ProjectStakeHolders { get; set; }

        //public virtual ICollection<TfileLinkUser> TfileLinkUsers { get; set; }

        //public virtual ICollection<TjobUserRelation> TjobUserRelations { get; set; }

        //public virtual ICollection<UserTag> UserTags { get; set; }

        //public virtual ICollection<WfqueueUser> WfqueueUsers { get; set; }

        //public virtual ICollection<WorkforVendor> WorkforVendors { get; set; }

        //public virtual ICollection<ZClientUserCompany> ZClientUserCompanies { get; set; }

        //public virtual ICollection<ZClientUserOrgStructure> ZClientUserOrgStructures { get; set; }

        //public virtual ICollection<ZClientUserRefreshToken> ZClientUserRefreshTokens { get; set; }

        //public virtual ICollection<ZClientUserRole> ZClientUserRoles { get; set; }

        //public ZClientUser()
        //{
        //    ProjectJobProjectMangers = new HashSet<ProjectJob>();
        //    ProjectJobSalesLeads = new HashSet<ProjectJob>();
        //    ProjectJobSiteSupervisors = new HashSet<ProjectJob>();
        //    ProjectStakeHolders = new HashSet<ProjectStakeHolder>();
        //    TfileLinkUsers = new HashSet<TfileLinkUser>();
        //    TjobUserRelations = new HashSet<TjobUserRelation>();
        //    UserTags = new HashSet<UserTag>();
        //    WfqueueUsers = new HashSet<WfqueueUser>();
        //    WorkforVendors = new HashSet<WorkforVendor>();
        //    ZClientUserCompanies = new HashSet<ZClientUserCompany>();
        //    ZClientUserOrgStructures = new HashSet<ZClientUserOrgStructure>();
        //    ZClientUserRefreshTokens = new HashSet<ZClientUserRefreshToken>();
        //    ZClientUserRoles = new HashSet<ZClientUserRole>();
        //}
    }
}
