using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;

namespace BaseWebSite
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
                SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
                ankDB.SurveyleriKapat();
                gnlDB.UyelikBitisBilgilendirmeNotuGonder();
            }
        }
    }
}
