using System;
using System.Linq;
using System.Web.UI;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Personalization.Impl;

namespace SitefinityWebApp
{
    public partial class PersonalizationFixes : Page
    {
        PersonalizationManager personalizationManager = PersonalizationManager.GetManager();
        PageManager pageManager = PageManager.GetManager();

        protected void Fix_Click(object sender, EventArgs e)
        {
            try
            {
                this.PersonalizationFix();

                this.SuccessMessage.Visible = true;
                this.FailMessage.Visible = false;
            }
            catch (Exception ex)
            {
                this.FailMessage.Text = string.Format("{0} - {1}", ex.GetType().Name, ex.Message);
                this.SuccessMessage.Visible = false;
                this.FailMessage.Visible = true;
            }
        }

        private void PersonalizationFix()
        {
            var pageDataList = pageManager.GetPageDataList().Where(p => p.IsPersonalized).ToList();
            var ids = pageDataList.Select(p => p.Id).ToList();
            var personalizedPages = pageManager.GetPageDataList().Where(p => ids.Contains(p.PersonalizationMasterId)).ToList();

            foreach (PageData pageData in personalizedPages)
            {
                foreach (var culture in pageData.AvailableCultures)
                {
                    if (!pageData.LanguageData.Where(p => p.Language == culture.Name).Any())
                    {
                        var languageData = pageManager.CreateLanguageData(pageData.Id);
                        if (languageData != null)
                        {
                            languageData.Language = culture.Name;
                            languageData.LastModified = DateTime.UtcNow;
                            languageData.PublicationDate = DateTime.UtcNow;

                            pageData.LanguageData.Add(languageData);
                            pageManager.SaveChanges();


                            this.Message.Text +=
                                " | Title: " + pageData.NavigationNode.Title +
                                " | culture.Name: " + culture.Name +
                                " | PersonalizationSegmentId: " + pageData.PersonalizationSegmentId.ToString() +
                                " | PersonalizationMasterId: " + pageData.PersonalizationMasterId.ToString() +
                                "<br />";
                        }
                    }
                }
            }
        }
    }
}