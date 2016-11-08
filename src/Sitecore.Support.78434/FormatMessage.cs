using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Form.Core.Configuration;
using Sitecore.Form.Core.Pipelines.FormSubmit;
using Sitecore.StringExtensions;
using Sitecore.WFFM.Abstractions.Dependencies;
using Sitecore.WFFM.Abstractions.Actions;
using System;
using System.Linq;

namespace Sitecore.Support.Form.Core.Pipelines.FormSubmit
{
    public class FormatMessage
    {
        protected virtual string ClienMessage
        {
            get
            {
                return DependenciesManager.ResourceManager.Localize("FAILED_SUBMIT");
            }
        }

        public void Process(SubmittedFormFailuresArgs failedArgs)
        {
            Assert.IsNotNull(failedArgs, "args");
            Assert.IsNotNull(failedArgs.FormID, "FormID");
            DependenciesManager.Logger.Warn("Web Forms for Marketers: an exception: {0} has occured while trying to execute an action.".FormatWith(new object[]
            {
                failedArgs.ErrorMessage
            }), this);
            for (int i = 0; i < failedArgs.Failures.Count<ExecuteResult.Failure>(); i++)
            {
                if (Sitecore.Form.Core.Configuration.Settings.HideInnerError && !failedArgs.Failures[i].IsCustom)
                {
                    string text = failedArgs.ErrorMessage.Between("System.Exception: ", "at Sitecore.Forms.Core.Dependencies.DefaultImplActionExecutor.ExecuteChecking");
                    if (!string.IsNullOrEmpty(text))
                    {
                        failedArgs.Failures[i].ErrorMessage = text;
                        return;
                    }
                    Database database = Factory.GetDatabase(failedArgs.Database, false);
                    if (database != null)
                    {
                        Item item = database.GetItem(failedArgs.FormID);
                        if (item != null)
                        {
                            string text2 = item[FormIDs.SaveActionFailedMessage];
                            if (!string.IsNullOrEmpty(text2))
                            {
                                failedArgs.Failures[i].ErrorMessage = text2;
                                return;
                            }
                        }
                        Item item2 = database.GetItem(FormIDs.SubmitErrorId);
                        if (item2 != null)
                        {
                            string text3 = item2["Value"];
                            if (!string.IsNullOrEmpty(text3))
                            {
                                failedArgs.Failures[i].ErrorMessage = text3;
                                return;
                            }
                        }
                    }
                    failedArgs.Failures[i].ErrorMessage = this.ClienMessage;
                }
            }
        }
    }
}
