using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Web.Administration;
using Microsoft.Web.Management;

namespace WebSiteManager
{
    public partial class SiteAdmin : System.Web.UI.Page
    {
        ServerManager IISManager = new ServerManager();
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected void AlterWebConfig()
        {
            Configuration appHost = IISManager.GetApplicationHostConfiguration();
            ConfigurationSection configPaths = appHost.GetSection("connectionstring");
            using (ServerManager serverManager = new ServerManager())
            {
                Configuration config = serverManager.GetWebConfiguration("TestSite");

                ConfigurationSection directoryBrowseSection =
                    config.GetSection("system.webServer/directoryBrowse");


                if ((bool)directoryBrowseSection["enabled"] != true)
                {
                    directoryBrowseSection["enabled"] = true;
                }

                serverManager.CommitChanges();
            }
          
        }
        protected void CreateSite(string sitename,string domain,string path)
        { 

            IISManager.Sites.Add(sitename, "http", domain, path);
            IISManager.CommitChanges();
        }
        protected void RemoveSite(string sitename, string appoolname)
        { 
        
         IISManager.Sites[sitename].Applications.Remove(this.IISManager.Sites[sitename].Applications[0]);
         IISManager.Sites.Remove(this.IISManager.Sites[sitename]);
         IISManager.ApplicationPools.Remove(this.IISManager.ApplicationPools[appoolname]);
         IISManager.CommitChanges();
        }
        protected void RoleControl()
        {
            Configuration config = IISManager.GetApplicationHostConfiguration();
            ConfigurationSection anonymousAuthenticationSection = config.GetSection("system.webServer/security/authentication/anonymousAuthentication", "SiteName");
            anonymousAuthenticationSection["enabled"] = true;
            anonymousAuthenticationSection["userName"] = @"IUSR_" + "username";
            anonymousAuthenticationSection["password"] = @"" + "password";
            IISManager.CommitChanges();
        }
        protected void CreatPool(string appoolname)
        {
            ApplicationPool newPool = IISManager.ApplicationPools[appoolname];
            if (newPool == null)
            {
                IISManager.ApplicationPools.Add(appoolname);
                newPool = IISManager.ApplicationPools[appoolname];
                newPool.Name = appoolname;
                newPool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                newPool.ManagedRuntimeVersion = "v4.0";
            }
        }

        protected void AlterPool(string sitename,string appoolname)
        {
            IISManager.Sites[sitename].Applications["/"].ApplicationPoolName = appoolname;
        }
        private void BindURL(string sitename, string domain)
        {
            string str =  IISManager.Sites[sitename].Bindings[0].Host.Split(new char[] { '.' })[0];
            string bindingInformation = "*:80:" + str + "." + domain;
            IISManager.Sites[sitename].Bindings.Add(bindingInformation, "http");
             IISManager.CommitChanges();
        }
        void getListOfIIS()
        {
           // this.listBox1.Items.Clear();
            string StateStr = "";
            for (int i = 0; i < IISManager.Sites.Count; i++)
            {

                switch (IISManager.Sites[i].State)
                {
                    case ObjectState.Started:
                        {
                            StateStr = "正常"; break;
                        }
                    case ObjectState.Starting:
                        {
                            StateStr = "正在启动"; break;
                        }
                    case ObjectState.Stopping:
                        {
                            StateStr = "正在关闭"; break;
                        }
                    case ObjectState.Stopped:
                        {
                            StateStr = "关闭"; break;
                        }
                }
              //  this.listBox1.Items.Add(IISManager.Sites[i].Name + "【" + StateStr + "】");
            }
        }
    
    }
}