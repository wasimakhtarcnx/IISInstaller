using Microsoft.Web.Administration;
using System;

namespace IISInstaller
{
    class Program
    {
        static ServerManager serverMgr = new ServerManager();
        static void Main(string[] args)
        {
            try
            {

                string strWebsitename = "NCSTC"; // abc
                string strApplicationPool = "DefaultAppPool";  // set your deafultpool :4.0 in IIS
                string strhostname = "NCSTC.com"; //abc.com
                string stripaddress = "*";// ip address
                string bindinginfo = stripaddress + ":80:" + strhostname;

                //check if website name already exists in IIS
                Boolean bWebsite = IsWebsiteExists(strWebsitename);
                if (!bWebsite)
                {
                    Site mySite = serverMgr.Sites.Add(strWebsitename.ToString(), "http", bindinginfo, "C:inetpubwwwrootyourWebsite");
                    mySite.ApplicationDefaults.ApplicationPoolName = strApplicationPool;
                    mySite.TraceFailedRequestsLogging.Enabled = true;
                    mySite.TraceFailedRequestsLogging.Directory = "C:inetpubcustomfoldersite";
                    serverMgr.CommitChanges();
                    Console.WriteLine("New website  " + strWebsitename + " added sucessfully");

                }
                else
                {
                    Console.WriteLine("Name should be unique, " + strWebsitename + "  is already exists. ");

                }
            }
            catch (Exception ae)
            {
                Console.WriteLine(ae.Message);
            }
        }
        public static bool IsWebsiteExists(string strWebsitename)
        {
            Boolean flagset = false;
            SiteCollection sitecollection = serverMgr.Sites;
            foreach (Site site in sitecollection)
            {
                if (site.Name == strWebsitename.ToString())
                {
                    flagset = true;
                    break;
                }
                else
                {
                    flagset = false;
                }
            }
            return flagset;
        }
    }
}
