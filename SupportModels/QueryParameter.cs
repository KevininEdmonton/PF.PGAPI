using K.Common;
using KS.Library.Interface.PFAPI.Domain;
using PFAPI.utility;

namespace PFAPI.SupportModels
{
    public class QueryParameter
    {
        public string filter { get; set; }
        public bool includeallchildrendata { get; set; }
        public string includedata { get; set; }
        public int page { get; set; }
        public int pagesize { get; set; }
        public string orderby { get; set; }

        public int Skip { get { return (page - 1) * pagesize; } }
        public int Take { get { return pagesize; } }
        public string theURL { get; set; }

        public QueryParameter(int qp_page, int qp_pagesize, string qp_orderby
                            , bool qp_includeallchildrendata, string qp_includedata, string qp_filter
                            , string thrurl)
        {
            if (qp_page <= 0)
                this.page = PFAPIStatics.SYS_Default_QP_Page;
            else
                this.page = qp_page;

            if (qp_pagesize <= 0)
                this.pagesize = PFAPIStatics.SYS_Default_QP_Pagesize;
            else if (qp_pagesize > PFAPIStatics.SYS_Default_QP_PagesizeMax)
                this.pagesize = PFAPIStatics.SYS_Default_QP_PagesizeMax;
            else
                this.pagesize = qp_pagesize;

            this.orderby = qp_orderby.IsNullOrEmptyOrWhiteSpace() ? PFAPIStatics.SYS_Default_QP_Orderby : qp_orderby;
            this.includeallchildrendata = qp_includeallchildrendata;
            this.includedata = qp_includedata;
            this.filter = qp_filter;
            this.theURL = thrurl;
        }

        public override string ToString()
        {
            string result = string.Empty;

            if (filter != PFAPIStatics.SYS_Default_QP_Filter)
            {
                if (!result.IsNullOrEmptyOrWhiteSpace()
                    && (result[result.Length - 1] != '&'))
                    result = result + "&";
                result = result + SystemStatics.SYS_QueryParameter_Filter + "=" + filter;
            }
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
            if (orderby != PFAPIStatics.SYS_Default_QP_Orderby)
            {
                if (!result.IsNullOrEmptyOrWhiteSpace()
                    && (result[result.Length - 1] != '&'))
                    result = result + "&";
                result = result + SystemStatics.SYS_QueryParameter_Orderby + "=" + orderby;
            }
            if (pagesize != PFAPIStatics.SYS_Default_QP_Pagesize)
            {
                if (!result.IsNullOrEmptyOrWhiteSpace()
                    && (result[result.Length - 1] != '&'))
                    result = result + "&";
                result = result + SystemStatics.SYS_QueryParameter_Pagesize + "=" + pagesize.ToString();
            }

            if (page != PFAPIStatics.SYS_Default_QP_Page)
            {
                if (!result.IsNullOrEmptyOrWhiteSpace()
                    && (result[result.Length - 1] != '&'))
                    result = result + "&";
                result = result + SystemStatics.SYS_QueryParameter_Page + "=" + page.ToString();
            }

            return result;
        }

        public QueryParameter Clone()
        {
            return new QueryParameter(page, pagesize, orderby, includeallchildrendata, includedata, filter, theURL);
        }
    }
}
