using K.Common;
using KS.Library.Interface.PFAPI.Domain;
using PFAPI.utility;

namespace PFAPI.SupportModels
{
    public class QueryParameterMin
    {
        public bool includeallchildrendata { get; set; }
        public string includedata { get; set; }

        public QueryParameterMin(bool qp_includeallchildrendata, string qp_includedata)
        {
            this.includeallchildrendata = qp_includeallchildrendata;
            this.includedata = qp_includedata;
        }

        public override string ToString()
        {
            string result = string.Empty;

            if (includeallchildrendata != PFAPIStatics.SYS_Default_QP_IncludeAllChildrenData)
            {
                if (!result.IsNullOrEmptyOrWhiteSpace()
                    && (result[result.Length - 1] != '&'))
                    result = result + "&";
                result = result + SystemStatics.SYS_QueryParameter_IncludeAllChildrenData + "=true";
            }
            else if (includedata != PFAPIStatics.SYS_Default_QP_IncludeData)
            {
                if (!result.IsNullOrEmptyOrWhiteSpace()
                    && (result[result.Length - 1] != '&'))
                    result = result + "&";
                result = result + SystemStatics.SYS_QueryParameter_IncludeData + "=" + includedata;
            }

            return result;
        }

        public QueryParameterMin Clone()
        {
            return new QueryParameterMin(includeallchildrendata, includedata);
        }
    }
}
