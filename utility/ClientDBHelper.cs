using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using K.Common;
using KS.Library.EFDB;

namespace PFAPI.utility
{
    public static class ClientDBHelper
    {
        public static PfsandBoxContext GetClientDBContext(this ClaimsPrincipal source, string connectionString, string clientDBName)
        {
            if (source == null|| connectionString.IsNullOrEmptyOrWhiteSpace()|| clientDBName.IsNullOrEmptyOrWhiteSpace())
                return null;
            string thev = source.GetClaimValue(SystemStatics.SYS_JWT_User_ClientID);
            if (thev.IsNullOrEmptyOrWhiteSpace())
                return null;

            PfsandBoxContext result = GetClientDBContext(thev, connectionString, clientDBName);
            // set timeout 3 mins
            result.Database.SetCommandTimeout(180);
            return result;
        }

        public static PfsandBoxContext GetClientDBContext(this string connectionString)
        {
            if (connectionString.IsNullOrEmptyOrWhiteSpace() )
                return null;

            PfsandBoxContext result = CreateClientDBContext(connectionString);
            // set timeout 3 mins
            result.Database.SetCommandTimeout(180);
            return result;
        }

        public static PfsandBoxContext GetClientDBContext(string clientID, string connectionString, string clientDBName)
        {
            string theConnectionString = connectionString.Replace(clientDBName, clientID);
            return CreateClientDBContext(theConnectionString);
        }
        public static PfsandBoxContext CreateClientDBContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PfsandBoxContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new PfsandBoxContext(optionsBuilder.Options);
        }

    }
}
