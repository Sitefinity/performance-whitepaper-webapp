using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Blogs.Model;
using Telerik.Sitefinity.DynamicModules.Data;
using Telerik.Sitefinity.DynamicModules.Builder;
using Telerik.Sitefinity.DynamicModules.Builder.Model;
using Telerik.Sitefinity.Events.Model;
using Telerik.Sitefinity.Frontend.Blogs.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Blogs.Mvc.Models.BlogPost;
using Telerik.Sitefinity.Frontend.ContentBlock.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Events.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Navigation.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Navigation.Mvc.Models;
using Telerik.Sitefinity.Frontend.News.Mvc.Controllers;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.Blogs;
using Telerik.Sitefinity.Modules.Events;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Modules.News;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.RelatedData;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.TestIntegration.Modules.ModuleBuilder;
using Telerik.Sitefinity.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.DynamicContent.Mvc.Controllers;
using Telerik.Sitefinity.Personalization.Impl;

namespace SitefinityWebApp
{
    public partial class LoadTestsDataGenerator : Page
    {
        protected void Generate_Click(object sender, EventArgs e)
        {
            try
            {
                this.Generate();

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

        private void Generate()
        {
            string templateTitle = "LoadTestsPageTemplate";
            string groupPageTitle = "LoadTestsGroupPage";
            string pageTitlePrefix = "LoadTestPage";

            string blogTitle = "LoadTestsBlog";
            string blogPostsTitlePrefix = "LoadTestsBlogPosts";
            string newsTitlePrefix = "LoadTestsNewsItem";
            string eventsTitlePrefix = "LoadTestsEvent";

            string libraryName = "LoadTestsLibrary";

            string userNamePrefix = "MixedUser";
            int userCount = 1000;

            int pagesInChunk = 200;

            int templateItemsCount = 5;
            int contentItemsCount = 10000 * 1;

            // Import dynamic module
            //this.ImportDynamicModule(contentItemsCount);
            //return;

            // Generate Admins
            this.GenerateAdminUsers(userNamePrefix, userCount, 201);

            return;

            // Generate Blogs
            this.GenerateBlogs(blogTitle, blogPostsTitlePrefix, contentItemsCount);

            // Generate Events
            this.CreateEvents(eventsTitlePrefix, contentItemsCount);

            // Generate News
            this.GenerateNews(newsTitlePrefix, contentItemsCount);

            // Import images for dynamic module
            this.ImportImages(libraryName);

            // Generate dynamic module items
            var module = ModuleBuilderManager.GetManager().Provider.GetDynamicModules().Single(m => m.Name == "LoadTestsModule");
            this.GenerateDynamicModuleItems(module, contentItemsCount);

            // Create page template
            var template = this.CreatePageTemplate(templateTitle);
            this.AddNavigationWidgetsToTemplate(template, templateItemsCount);
            this.AddContentBlocksToTemplate(template, templateItemsCount);

            // Create group page
            var groupPageId = this.CreateGroupPage(template, groupPageTitle);

            int startIndex = 1;

            // Create pages based on LoadTestsGroupPage with Content block widget and News widget
            this.GeneratePagesWithContentBlockAndNewsWidget(groupPageId, template, pageTitlePrefix, pagesInChunk, startIndex);
            startIndex += pagesInChunk;

            // Create pages based on LoadTestsGroupPage with Content block widget and Events widget
            this.GeneratePagesWithContentBlockAndEventsWidget(groupPageId, template, pageTitlePrefix, pagesInChunk, startIndex);
            startIndex += pagesInChunk;

            // Create pages based on LoadTestsGroupPage with Content block widget and Blogs widget
            //this.GeneratePagesWithContentBlockAndBlogPostsWidget(groupPageId, template, pageTitlePrefix, pagesInChunk, startIndex);
            //startIndex += pagesInChunk;

            // Create pages based on LoadTestsGroupPage with Content block widget and Dynamic widget
            this.GeneratePagesWithContentBlockAndDynamicWidget(groupPageId, template, pageTitlePrefix, pagesInChunk, startIndex);
            startIndex += pagesInChunk;

            // Create pages based on LoadTestsGroupPage with Content block widget and Dynamic widget
            this.GeneratePagesWithContentBlockNewsEventsAndDynamicWidget(groupPageId, template, pageTitlePrefix, pagesInChunk, startIndex);
            startIndex += pagesInChunk;

            // Create personalized pages based on LoadTestsGroupPage with Content block widget, Events widget and News widget
            this.GeneratePesonalizedPagesWithContentBlockNewsEventsAndDynamicWidget(groupPageId, template, pageTitlePrefix, pagesInChunk, startIndex);
            startIndex += pagesInChunk;
        }

        private void GenerateAdminUsers(string userNamePrefix, int userCount, int startIndex = 1)
        {
            for (int i = startIndex; i <= userCount; i++)
            {
                var email = String.Format("{0}{1}@test.test", userNamePrefix, i);
                var password = "password";
                ServerOperations.Users().CreateUser(email, password, "test", "test", true, userNamePrefix, i.ToString(), "Administrators");         
            }
        }

        private void GeneratePesonalizedPagesWithContentBlockNewsEventsAndDynamicWidget(Guid parentPageId, PageTemplate template, string pageTitlePrefix, int pageCount, int startIndex)
        {
            string role = "Users";
            string name = "Load Tests Segment";
            string description = "Load Tests Segment Description";

            var segmentId = ServerOperations.Personalization().CreateRoleSegment(role, name, description);

            for (int i = startIndex; i <= pageCount + startIndex - 1; i++)
            {
                string pageTitle = string.Format("{0}{1}", pageTitlePrefix, i);
                var pageId = this.CreateChildPage(parentPageId, pageTitle);
                ServerOperations.Templates().SetTemplateToPage(pageId, template.Id);

                var contentBlock = new MvcControllerProxy();
                contentBlock.ControllerName = typeof(ContentBlockController).FullName;
                contentBlock.Settings = new ControllerSettings(new ContentBlockController()
                {
                    Content = "Personalized page"
                });

                var newsWidget = new MvcControllerProxy();
                newsWidget.ControllerName = typeof(NewsController).FullName;

                var eventsWidget = new MvcControllerProxy();
                eventsWidget.ControllerName = typeof(EventController).FullName;

                string moduleName = "LoadTestsModule";
                string typeName = "LoadTestsDynamicItem";
                string contentType = string.Format("Telerik.Sitefinity.DynamicTypes.Model.{0}.{1}", moduleName, typeName);

                var dynamicWidget = new MvcWidgetProxy();
                dynamicWidget.ControllerName = typeof(DynamicContentController).FullName;
                var dynamicController = new DynamicContentController();
                dynamicController.Model.ContentType = TypeResolutionService.ResolveType(contentType);
                dynamicController.Model.ProviderName = DynamicModuleManager.GetManager().Provider.Name;
                dynamicWidget.Settings = new ControllerSettings(dynamicController);
                dynamicWidget.WidgetName = typeName;

                var manager = PageManager.GetManager();
                var pageData = manager.GetPageNode(pageId).GetPageData();
                var draft = manager.EditPage(pageData.Id);

                this.AddControlToPage(contentBlock, draft);
                this.AddControlToPage(newsWidget, draft);
                this.AddControlToPage(eventsWidget, draft);
                this.AddControlToPage(dynamicWidget, draft);

                manager.PublishPageDraft(draft);
                manager.SaveChanges();

                this.PersonalizePage(pageId, segmentId);
            }
        }

        private void GeneratePesonalizedPagesWithContentBlockEventsAndNewsWidget(Guid parentPageId, PageTemplate template, string pageTitlePrefix, int pageCount, int startIndex)
        {
            string role = "Users";
            string name = "Load Tests Segment";
            string description = "Load Tests Segment Description";

            var segmentId = ServerOperations.Personalization().CreateRoleSegment(role, name, description);

            for (int i = startIndex; i <= pageCount + startIndex - 1; i++)
            {
                string pageTitle = string.Format("{0}{1}", pageTitlePrefix, i);
                var pageId = this.CreateChildPage(parentPageId, pageTitle);

                ServerOperations.Templates().SetTemplateToPage(pageId, template.Id);
                var pageData = this.CreatePersonalizedPage(pageId, segmentId);

                var contentBlock = new MvcControllerProxy();
                contentBlock.ControllerName = typeof(ContentBlockController).FullName;
                contentBlock.Settings = new ControllerSettings(new ContentBlockController()
                {
                    Content = "Personalized page"
                });

                string moduleName = "LoadTestsModule";
                string typeName = "LoadTestsDynamicItem";
                string contentType = string.Format("Telerik.Sitefinity.DynamicTypes.Model.{0}.{1}", moduleName, typeName);

                var newsWidget = new MvcControllerProxy();
                newsWidget.ControllerName = typeof(NewsController).FullName;

                var eventsWidget = new MvcControllerProxy();
                eventsWidget.ControllerName = typeof(EventController).FullName;

                var manager = PageManager.GetManager();
                var draft = manager.EditPage(pageData.Id);

                this.AddControlToPage(newsWidget, draft);
                manager.PublishPageDraft(draft);

                pageData = manager.GetPageNode(pageId).GetPageData();
                draft = manager.EditPage(pageData.Id);
                this.AddControlToPage(contentBlock, draft);
                manager.PublishPageDraft(draft);

                pageData = manager.GetPageNode(pageId).GetPageData();
                draft = manager.EditPage(pageData.Id);
                this.AddControlToPage(eventsWidget, draft);
                manager.PublishPageDraft(draft);

                manager.SaveChanges();
            }
        }

        private void PersonalizePage(Guid pageId, Guid segmentId)
        { 
            var personalizationManager = PersonalizationManager.GetManager();
            var pageManager = PageManager.GetManager();
            var pageNode = pageManager.GetPageNode(pageId);
            var pageData = pageNode.GetPageData();

            personalizationManager.CreatePersonalizedPage(pageData.Id, segmentId);
            personalizationManager.SaveChanges();
        }

        private PageData CreatePersonalizedPage(Guid pageId, Guid segmentId)
        {
            var personalizationManager = PersonalizationManager.GetManager();
            var manager = PageManager.GetManager();
            var pageNode = manager.GetPageNode(pageId);
            var pageData = pageNode.GetPageData();

            personalizationManager.CreatePersonalizedPage(pageData.Id, segmentId);
            personalizationManager.SaveChanges();

            pageData = manager
                .GetPageDataList()
                .Where(p => p.PersonalizationMasterId == pageData.Id && p.PersonalizationSegmentId == segmentId)
                .FirstOrDefault();

            return pageData;
        }

        #region Pages

        private PageTemplate CreatePageTemplate(string templateTitle)
        {
            var manager = PageManager.GetManager();
            var template = manager.GetTemplates().SingleOrDefault(t => t.Title == templateTitle);
            if (template != null)
                return template;

            template = manager.CreateTemplate();
            template.Title = templateTitle;
            template.Name = new Lstring(Regex.Replace(template.Title, ArrangementConstants.UrlNameCharsToReplace, string.Empty).ToLower());
            template.Description = template.Title + " descr";
            template.ShowInNavigation = true;
            template.Framework = PageTemplateFramework.Mvc;
            template.Category = SiteInitializer.CustomTemplatesCategoryId;
            template.Visible = true;

            var draft = manager.TemplatesLifecycle.Edit(template);
            manager.TemplatesLifecycle.Publish(draft);
            manager.SaveChanges();

            return template;
        }

        private void AddNavigationWidgetsToTemplate(PageTemplate template, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var mvcProxy = new MvcControllerProxy();
                mvcProxy.ControllerName = typeof(NavigationController).FullName;
                var navigationController = new NavigationController();
                navigationController.TemplateName = "Horizontal";
                navigationController.SelectionMode = PageSelectionMode.CurrentPageChildren;
                mvcProxy.Settings = new ControllerSettings(navigationController);

                this.AddControlToTemplate(template.Id, mvcProxy, "Body", "Caption");
            }
        }

        private void AddContentBlocksToTemplate(PageTemplate template, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var mvcProxy = new MvcControllerProxy();
                mvcProxy.ControllerName = typeof(ContentBlockController).FullName;
                mvcProxy.Settings = new ControllerSettings(new ContentBlockController()
                {
                    Content = "Content block"
                });

                this.AddControlToTemplate(template.Id, mvcProxy, "Body", "Caption");
            }
        }

        private void AddControlToTemplate(Guid templateId, MvcControllerProxy control, string placeHolder, string caption)
        {
            var pageManager = PageManager.GetManager();
            var template = pageManager.GetTemplates().Where(t => t.Id == templateId).SingleOrDefault();

            if (template != null)
            {
                var draft = pageManager.TemplatesLifecycle.Edit(template);

                if (draft != null)
                {
                    var templateControl = pageManager.CreateControl<TemplateDraftControl>(control, placeHolder);
                    templateControl.Caption = caption;
                    pageManager.SetControlDefaultPermissions(templateControl);
                    draft.Controls.Add(templateControl);

                    pageManager.TemplatesLifecycle.Publish(draft);
                    pageManager.SaveChanges();
                }
            }
        }

        private Guid CreateGroupPage(PageTemplate template, string groupPageTitle)
        {
            var manager = PageManager.GetManager();
            var page = manager.GetPageNodes().SingleOrDefault(p => p.Title == groupPageTitle && p.NodeType == NodeType.Group);
            if (page != null)
                return page.Id;

            var parentNode = manager.GetPageNode(SiteInitializer.CurrentFrontendRootNodeId);
            var pageId = Guid.NewGuid();
            var pageNode = manager.CreatePage(parentNode, pageId, NodeType.Group);
            pageNode.Title = groupPageTitle;
            pageNode.Name = groupPageTitle;
            pageNode.ShowInNavigation = true;
            pageNode.DateCreated = DateTime.UtcNow;
            pageNode.LastModified = DateTime.UtcNow;
            pageNode.UrlName = Regex.Replace(groupPageTitle.ToLower(), ArrangementConstants.UrlNameCharsToReplace, ArrangementConstants.UrlNameReplaceString);
            manager.SaveChanges();

            return pageId;
        }

        private void GeneratePagesWithContentBlockAndNewsWidget(Guid parentPageId, PageTemplate template, string pageTitlePrefix, int pageCount, int startIndex)
        {
            for (int i = startIndex; i <= pageCount + startIndex - 1; i++)
            {
                string pageTitle = string.Format("{0}{1}", pageTitlePrefix, i);
                var pageId = this.CreateChildPage(parentPageId, pageTitle);
                ServerOperations.Templates().SetTemplateToPage(pageId, template.Id);
                
                var contentBlock = new MvcControllerProxy();
                contentBlock.ControllerName = typeof(ContentBlockController).FullName;
                contentBlock.Settings = new ControllerSettings(new ContentBlockController()
                {
                    Content = "News page"
                });

                var newsWidget = new MvcControllerProxy();
                newsWidget.ControllerName = typeof(NewsController).FullName;

                var manager = PageManager.GetManager();
                var pageData = manager.GetPageNode(pageId).GetPageData();
                var draft = manager.EditPage(pageData.Id);

                this.AddControlToPage(contentBlock, draft);
                this.AddControlToPage(newsWidget, draft);

                manager.PublishPageDraft(draft);
                manager.SaveChanges();
            }
        }

        private void GeneratePagesWithContentBlockAndEventsWidget(Guid parentPageId, PageTemplate template, string pageTitlePrefix, int pageCount, int startIndex)
        {
            for (int i = startIndex; i <= pageCount + startIndex - 1; i++)
            {
                string pageTitle = string.Format("{0}{1}", pageTitlePrefix, i);
                var pageId = this.CreateChildPage(parentPageId, pageTitle);
                ServerOperations.Templates().SetTemplateToPage(pageId, template.Id);

                var contentBlock = new MvcControllerProxy();
                contentBlock.ControllerName = typeof(ContentBlockController).FullName;
                contentBlock.Settings = new ControllerSettings(new ContentBlockController()
                {
                    Content = "Events page"
                });

                var eventsWidget = new MvcControllerProxy();
                eventsWidget.ControllerName = typeof(EventController).FullName;

                var manager = PageManager.GetManager();
                var pageData = manager.GetPageNode(pageId).GetPageData();
                var draft = manager.EditPage(pageData.Id);

                this.AddControlToPage(contentBlock, draft);
                this.AddControlToPage(eventsWidget, draft);

                manager.PublishPageDraft(draft);
                manager.SaveChanges();
            }
        }

        private void GeneratePagesWithContentBlockAndBlogPostsWidget(Guid parentPageId, PageTemplate template, string pageTitlePrefix, int pageCount, int startIndex)
        {
            for (int i = startIndex; i <= pageCount + startIndex - 1; i++)
            {
                string pageTitle = string.Format("{0}{1}", pageTitlePrefix, i);
                var pageId = this.CreateChildPage(parentPageId, pageTitle);
                ServerOperations.Templates().SetTemplateToPage(pageId, template.Id);

                var contentBlock = new MvcControllerProxy();
                contentBlock.ControllerName = typeof(ContentBlockController).FullName;
                contentBlock.Settings = new ControllerSettings(new ContentBlockController()
                {
                    Content = "Blog posts page"
                });

                var blogPostsWidget = new MvcControllerProxy();
                blogPostsWidget.ControllerName = typeof(BlogPostController).FullName;
                var controller = new BlogPostController();
                controller.Model.ParentFilterMode = ParentFilterMode.All;
                blogPostsWidget.Settings = new ControllerSettings(controller);

                var manager = PageManager.GetManager();
                var pageData = manager.GetPageNode(pageId).GetPageData();
                var draft = manager.EditPage(pageData.Id);

                this.AddControlToPage(contentBlock, draft);
                this.AddControlToPage(blogPostsWidget, draft);

                manager.PublishPageDraft(draft);
                manager.SaveChanges();
            }
        }

        private void GeneratePagesWithContentBlockAndDynamicWidget(Guid parentPageId, PageTemplate template, string pageTitlePrefix, int pageCount, int startIndex)
        {
            for (int i = startIndex; i <= pageCount + startIndex - 1; i++)
            {
                string pageTitle = string.Format("{0}{1}", pageTitlePrefix, i);
                var pageId = this.CreateChildPage(parentPageId, pageTitle);
                ServerOperations.Templates().SetTemplateToPage(pageId, template.Id);

                var contentBlock = new MvcControllerProxy();
                contentBlock.ControllerName = typeof(ContentBlockController).FullName;
                contentBlock.Settings = new ControllerSettings(new ContentBlockController()
                {
                    Content = "Dynamic widget page"
                });

                string moduleName = "LoadTestsModule";
                string typeName = "LoadTestsDynamicItem";
                string contentType = string.Format("Telerik.Sitefinity.DynamicTypes.Model.{0}.{1}", moduleName, typeName);

                var dynamicWidget = new MvcWidgetProxy();
                dynamicWidget.ControllerName = typeof(DynamicContentController).FullName;
                var dynamicController = new DynamicContentController();
                dynamicController.Model.ContentType = TypeResolutionService.ResolveType(contentType);
                dynamicController.Model.ProviderName = DynamicModuleManager.GetManager().Provider.Name;
                dynamicWidget.Settings = new ControllerSettings(dynamicController);
                dynamicWidget.WidgetName = typeName;

                var manager = PageManager.GetManager();
                var pageData = manager.GetPageNode(pageId).GetPageData();
                var draft = manager.EditPage(pageData.Id);

                this.AddControlToPage(contentBlock, draft);
                this.AddControlToPage(dynamicWidget, draft);

                manager.PublishPageDraft(draft);
                manager.SaveChanges();
            }
        }

        private void GeneratePagesWithContentBlockNewsEventsAndDynamicWidget(Guid parentPageId, PageTemplate template, string pageTitlePrefix, int pageCount, int startIndex)
        {
            for (int i = startIndex; i <= pageCount + startIndex - 1; i++)
            {
                string pageTitle = string.Format("{0}{1}", pageTitlePrefix, i);
                var pageId = this.CreateChildPage(parentPageId, pageTitle);
                ServerOperations.Templates().SetTemplateToPage(pageId, template.Id);

                var contentBlock = new MvcControllerProxy();
                contentBlock.ControllerName = typeof(ContentBlockController).FullName;
                contentBlock.Settings = new ControllerSettings(new ContentBlockController()
                {
                    Content = "Dynamic widget page"
                });

                string moduleName = "LoadTestsModule";
                string typeName = "LoadTestsDynamicItem";
                string contentType = string.Format("Telerik.Sitefinity.DynamicTypes.Model.{0}.{1}", moduleName, typeName);

                var dynamicWidget = new MvcWidgetProxy();
                dynamicWidget.ControllerName = typeof(DynamicContentController).FullName;
                var dynamicController = new DynamicContentController();
                dynamicController.Model.ContentType = TypeResolutionService.ResolveType(contentType);
                dynamicController.Model.ProviderName = DynamicModuleManager.GetManager().Provider.Name;
                dynamicWidget.Settings = new ControllerSettings(dynamicController);
                dynamicWidget.WidgetName = typeName;

                var newsWidget = new MvcControllerProxy();
                newsWidget.ControllerName = typeof(NewsController).FullName;

                var eventsWidget = new MvcControllerProxy();
                eventsWidget.ControllerName = typeof(EventController).FullName;

                var manager = PageManager.GetManager();
                var pageData = manager.GetPageNode(pageId).GetPageData();
                var draft = manager.EditPage(pageData.Id);

                this.AddControlToPage(contentBlock, draft);
                this.AddControlToPage(dynamicWidget, draft);
                this.AddControlToPage(newsWidget, draft);
                this.AddControlToPage(eventsWidget, draft);

                manager.PublishPageDraft(draft);
                manager.SaveChanges();
            }
        }

        private Guid CreateChildPage(Guid parentPageId, string pageTitle)
        {
            var pageId = Guid.NewGuid();
            var manager = PageManager.GetManager();
            var parentNode = manager.GetPageNode(parentPageId);
            var pageNode = manager.CreatePage(parentNode, pageId, NodeType.Standard);
            var pageData = pageNode.GetPageData();

            pageNode.Title = pageTitle;
            pageNode.Name = pageTitle;
            pageNode.ShowInNavigation = true;
            pageNode.DateCreated = DateTime.UtcNow;
            pageNode.LastModified = DateTime.UtcNow;
            pageNode.UrlName = Regex.Replace(pageTitle.ToLower(), ArrangementConstants.UrlNameCharsToReplace, ArrangementConstants.UrlNameReplaceString);
            manager.SaveChanges();

            var draft = manager.EditPage(pageData.Id);
            manager.PublishPageDraft(draft);
            manager.SaveChanges();

            return pageId;
        }

        private void AddControlToPage(MvcControllerProxy mvcControl, PageDraft page)
        {
            var pageManager = PageManager.GetManager();
            var draftControlDefault = pageManager.CreateControl<PageDraftControl>(mvcControl, "Body");
            draftControlDefault.Caption = "Widget caption";
            pageManager.SetControlDefaultPermissions(draftControlDefault);
            page.Controls.Add(draftControlDefault);
        }

        #endregion

        #region Dynamic modules

        private DynamicModule ImportDynamicModule(int itemsCount)
        {
            DynamicModule module = null;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SitefinityWebApp.LoadTestsModule.zip"))
            {
                module = ModuleBuilderTestHelper.ImportModule(stream, activate: true);
            }

            return module;
        }

        private void GenerateDynamicModuleItems(DynamicModule module, int itemsCount)
        {
            var moduleManager = ModuleBuilderManager.GetManager();
            var typeName = moduleManager.Provider.GetDynamicModuleTypes().Single(t => t.ParentModuleId == module.Id).TypeName;
            var moduleType = TypeResolutionService.ResolveType(string.Format("Telerik.Sitefinity.DynamicTypes.Model.{0}.{1}", module.Name, typeName));

            var manager = DynamicModuleManager.GetManager();

            for (var i = 1; i <= itemsCount; i++)
            {
                string title = string.Format("{0}{1}", typeName, i);
                int imageCount = 6;
                int imageNumber = i < imageCount ? i : (i % imageCount + 1);
                var imageTitle = string.Format("LoadTestImage{0}", imageNumber);
                var image = LibrariesManager.GetManager().GetImages().FirstOrDefault(im => im.Title.ToString() == imageTitle);

                if (image == null)
                    throw new ArgumentNullException(String.Format("Image with name {0} was not found", imageTitle));

                var item = manager.CreateDataItem(moduleType);
                item.UrlName = title;
                item.SetString("Title", title);
                item.CreateRelation(image, "Image");
                manager.Lifecycle.Publish(item);

                if (i % FlushItemsCount == 0)
                    manager.SaveChanges();
            }

            manager.SaveChanges();
        }

        private void ImportImages(string albumName)
        {
            var manager = LibrariesManager.GetManager();
            //var album = manager.CreateAlbum();
            //album.Title = albumName;
            //album.UrlName = albumName;
            //manager.RecompileItemUrls(album);
            //manager.SaveChanges();

            var albumId = ServerOperations.Images().CreateLibrary(albumName);
            var album = manager.GetAlbum(albumId);

            for (int i = 1; i <= 6; i++)
            {
                string imageTitle = string.Format("LoadTestImage{0}", i);
                var image = manager.CreateImage();
                image.Parent = album;
                image.Title = imageTitle;
                image.UrlName = imageTitle.ToLower().Replace(' ', '-');

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("SitefinityWebApp.number{0}.jpg", i)))
                {
                    manager.Upload(image, stream, ".jpeg");
                }

                manager.Lifecycle.Publish(image);
                manager.SaveChanges();
            }
        }

        #endregion

        #region Events

        private void GenerateEvents(string calendarsTitlePrefix, int calendarsCount, string eventsTitlePrefix, int eventsCount)
        {
            var calendars = this.CreateCalendars(calendarsTitlePrefix, calendarsCount);
            foreach (var calendar in calendars)
                this.CreateEvents(calendar, eventsTitlePrefix, eventsCount);
        }

        private IEnumerable<Calendar> CreateCalendars(string calendarsTitlePrefix, int calendarsCount)
        {
            var manager = EventsManager.GetManager();
            for (int i = 1; i <= calendarsCount; i++)
            {
                string calendarName = string.Format("{0}{1}", calendarsTitlePrefix, i);
                var calendar = manager.CreateCalendar();
                calendar.Title = calendarName;

                if (i % FlushItemsCount == 0)
                    manager.SaveChanges();

                yield return calendar;
            }

            manager.SaveChanges();
        }

        private void CreateEvents(Calendar calendar, string eventsTitlePrefix, int eventsCount)
        {
            var manager = EventsManager.GetManager();
            for (int j = 1; j <= eventsCount; j++)
            {
                string eventTitle = string.Format("{0}_{1}{2}", calendar.Title, eventsTitlePrefix, j);
                var @event = manager.CreateEvent();
                @event.Title = eventTitle;
                @event.Parent = calendar;
                @event.UrlName = eventTitle;
                @event.SetWorkflowStatus(manager.Provider.ApplicationName, "Published");
                manager.Lifecycle.Publish(@event);

                if (j % FlushItemsCount == 0)
                    manager.SaveChanges();
            }

            manager.SaveChanges();
        }

        private void CreateEvents(string eventsTitlePrefix, int eventsCount)
        {
            var manager = EventsManager.GetManager();
            for (int j = 1; j <= eventsCount; j++)
            {
                string eventTitle = string.Format("{0}{1}", eventsTitlePrefix, j);
                var @event = manager.CreateEvent();
                @event.Title = eventTitle;
                @event.UrlName = eventTitle;
                @event.SetWorkflowStatus(manager.Provider.ApplicationName, "Published");
                manager.Lifecycle.Publish(@event);

                if (j % FlushItemsCount == 0)
                    manager.SaveChanges();
            }

            manager.SaveChanges();
        }

        private void FixCreatedEvents()
        {
            var manager = EventsManager.GetManager();
            var events = manager.GetEvents().Where(x => x.ApprovalWorkflowState == "Published").ToList();
            foreach (var @event in events)
            {
                if (String.IsNullOrEmpty(@event.UrlName))
                {
                    @event.UrlName = @event.Title;
                    manager.SaveChanges();
                }
            }
        }

        #endregion

        #region News

        private void GenerateNews(string titlePrefix, int count)
        {
            var manager = NewsManager.GetManager();
            for (int index = 1; index <= count; index++)
            {
                string title = string.Format("{0}{1}", titlePrefix, index);
                var newsItem = manager.CreateNewsItem();

                newsItem.Title = title;
                newsItem.DateCreated = DateTime.UtcNow;
                newsItem.PublicationDate = DateTime.UtcNow;
                newsItem.LastModified = DateTime.UtcNow;
                newsItem.UrlName = title.ToLower();

                var taxonomyManager = TaxonomyManager.GetManager();
                var category = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(t => t.Taxonomy.Name == "Categories").FirstOrDefault();
                if (category != null)
                {
                    newsItem.Organizer.AddTaxa("Category", category.Id);
                }

                var tag = taxonomyManager.GetTaxa<FlatTaxon>().Where(t => t.Taxonomy.Name == "Tags").FirstOrDefault();
                if (tag != null)
                {
                    newsItem.Organizer.AddTaxa("Tags", tag.Id);
                }

                newsItem.SetWorkflowStatus(manager.Provider.ApplicationName, "Published");
                manager.Lifecycle.Publish(newsItem);

                if (index % FlushItemsCount == 0)
                    manager.SaveChanges();
            }

            manager.SaveChanges();
        }

        #endregion

        #region Blogs

        private void GenerateBlogs(string blogTitle, string blogPostsTitlePrefix, int blogPostsCount)
        {
            var blog = this.CreateBlog(blogTitle);
            this.CreateBlogPosts(blog, blogPostsTitlePrefix, blogPostsCount);
        }

        private Blog CreateBlog(string blogTitle)
        {
            var manager = BlogsManager.GetManager();
            var blog = manager.CreateBlog();
            blog.Title = blogTitle;
            manager.SaveChanges();

            return blog;
        }

        private void CreateBlogPosts(Blog blog, string blogPostsTitlePrefix, int blogPostsCount)
        {
            var manager = BlogsManager.GetManager();
            for (int index = 1; index <= blogPostsCount; index++)
            {
                string blogPostTitle = string.Format("{0}{1}", blogPostsTitlePrefix, index);
                var blogPost = manager.CreateBlogPost();
                blogPost.Parent = blog;
                blogPost.Title = blogPostTitle;

                manager.Lifecycle.Publish(blogPost);

                if (index % FlushItemsCount == 0)
                    manager.SaveChanges();
            }

            manager.SaveChanges();
        }

        #endregion

        #region Constants

        private const int FlushItemsCount = 1000;

        #endregion
    }
}