using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace WebapiTest
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string fullPath = Path.GetFullPath(this.FileUpload1.PostedFile.FileName);
            PicDeal.MakeThumbnail(fullPath, 100, 100, ThumbnailMod.Cut);
        }
    }
}