﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Signum.Web.Extensions.Mailing.Views
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using Signum.Entities;
    
    #line 1 "..\..\Mailing\Views\EmailAddress.cshtml"
    using Signum.Entities.Mailing;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Mailing/Views/EmailAddress.cshtml")]
    public partial class EmailAddress : System.Web.Mvc.WebViewPage<dynamic>
    {
        public EmailAddress()
        {
        }
        public override void Execute()
        {

WriteLiteral("\r\n");


            
            #line 3 "..\..\Mailing\Views\EmailAddress.cshtml"
 using (var sc = Html.TypeContext<EmailAddressDN>())
{
    using (Html.FieldInline())
    {
    
            
            #line default
            #line hidden
            
            #line 7 "..\..\Mailing\Views\EmailAddress.cshtml"
Write(Html.EntityLine(sc, ea => ea.EmailOwner));

            
            #line default
            #line hidden
            
            #line 7 "..\..\Mailing\Views\EmailAddress.cshtml"
                                             

    
            
            #line default
            #line hidden
            
            #line 9 "..\..\Mailing\Views\EmailAddress.cshtml"
Write(Html.ValueLine(sc, c => c.EmailAddress, vl => vl.ValueHtmlProps.Add("style", "width:250px")));

            
            #line default
            #line hidden
            
            #line 9 "..\..\Mailing\Views\EmailAddress.cshtml"
                                                                                                 
    
            
            #line default
            #line hidden
            
            #line 10 "..\..\Mailing\Views\EmailAddress.cshtml"
Write(Html.ValueLine(sc, c => c.DisplayName, vl => vl.ValueHtmlProps.Add("style", "width:250px")));

            
            #line default
            #line hidden
            
            #line 10 "..\..\Mailing\Views\EmailAddress.cshtml"
                                                                                                
    }

}

            
            #line default
            #line hidden

        }
    }
}
#pragma warning restore 1591
