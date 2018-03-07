namespace Sitecore.Support.Shell.Framework.Commands.UserManager
{
  using Sitecore;
  using Sitecore.Caching;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
  using Sitecore.Security;
  using Sitecore.Security.Accounts;
  using Sitecore.Shell.Framework.Commands;
  using Sitecore.Web.UI.Sheer;
  using Sitecore.Web.UI.WebControls;
  using Sitecore.Web.UI.XamlSharp.Continuations;
  using System;
  using System.Collections.Generic;

  public class ResetMySettings : Command, ISupportsContinuation
  {
    public override void Execute(CommandContext context)
    {
      Assert.ArgumentNotNull(context, "context");
      if (ContinuationManager.Current != null)
      {
        ContinuationManager.Current.Start(this, "Run");
      }
      else
      {
        Context.ClientPage.Start(this, "Run");
      }
    }

    protected void Run(ClientPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      if (args.IsPostBack)
      {
        if (args.Result == "yes")
        {
          User user = Context.User;
          Assert.IsNotNull(user, typeof(User));
          UserProfile profile = user.Profile;
          List<string> customPropertyNames = profile.GetCustomPropertyNames();
          string str = "/" + user.Name + "/";
          foreach (string str2 in customPropertyNames)
          {
            if (str2.StartsWith(str, StringComparison.InvariantCultureIgnoreCase))
            {
              profile.RemoveCustomProperty(str2);
            }
          }
          profile.Save();
          RegistryCache registryCache = CacheManager.GetRegistryCache(Context.Site);
          if (registryCache != null)
          {
            registryCache.Clear();
          }
          string[] parameters = new string[] { user.Name };
          Log.Audit(this, "Reset settings: {0}", parameters);
          SheerResponse.Alert(Translate.Text("Your settings have been reset.\n\nSome changes will first take effect, when the browser is refreshed."), new string[0]);
        }
      }
      else
      {
        SheerResponse.Confirm(Translate.Text("Are you sure you want to reset your settings to default values?"));
        args.WaitForPostBack();
      }
    }
  }
}