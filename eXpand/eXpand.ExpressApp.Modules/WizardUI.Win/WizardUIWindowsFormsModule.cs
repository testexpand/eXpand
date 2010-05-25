//-----------------------------------------------------------------------
// <copyright file="WizardUIWindowsFormsModule.cs" created="03.06.2009" company="VenDoc Software GmbH">
//     Copyright (c) VenDoc Software GmbH. All rights reserved.
// </copyright>
// <author>Martin Praxmarer</author>
//-----------------------------------------------------------------------

using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using System;
using System.Linq;

namespace eXpand.ExpressApp.WizardUI.Win
{

    public interface IModelDetailViewWizard : IModelNode
    {
        IModelDetailViewWizardPages Wizard { get; }
    }

    public interface IModelDetailViewWizardPages : IModelNode, IModelList<IModelDetailViewWizardPage>
    {
        bool ShowInWizard { get; set; }
        bool ShowCompletionWizardPage { get; set; }
    }

    [DisplayProperty("Caption"), KeyProperty("ID"), ModelDisplayName("WizardPage"), ModelPersistentName("WizardPage")]
    public interface IModelDetailViewWizardPage : IModelNode
    {
        [Required()]
        string ID { get; set; }

        [Localizable(true)]
        string Caption { get; set; }

        [Browsable(false)]
        CalculatedModelNodeList<IModelDetailView> DetailViews { get; }

        [Required(), DataSourceProperty("DetailViews"), ModelPersistentName("ViewID")]
        IModelDetailView DetailView { get; set; }

        [Localizable(true)]
        string Description { get; set; }
    }

    [DomainLogic(typeof(IModelDetailViewWizardPage))]
    public static class ModelDetailViewWizardPageLogic
    {
        public static CalculatedModelNodeList<IModelDetailView> Get_DetailViews(IModelDetailViewWizardPage wizardPage)
        {
            CalculatedModelNodeList<IModelDetailView> views = new CalculatedModelNodeList<IModelDetailView>();
            if (wizardPage.Parent == null)
            {
                return views;
            }

            IModelDetailView parentView = wizardPage.Parent.Parent as IModelDetailView;
            if (parentView == null || parentView.ModelClass == null)
            {
                return views;
            }
            
            foreach (var modelView in wizardPage.Application.Views
                .OfType<IModelDetailView>()
                .Where(modelView => modelView.ModelClass != null &&
                    modelView.ModelClass.TypeInfo.IsAssignableFrom(parentView.ModelClass.TypeInfo) &&
                    !modelView.ModelClass.TypeInfo.IsAbstract))
            {
                views.Add(modelView);
            }

            return views;
        }
    }

    /// <summary>
    /// Contains an RibbonDetailView Template with an Wizard Control on it
    /// </summary>
    [ToolboxItem(true)]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxItemFilter("Xaf.Platform.Win")]
    [Description("Contains an RibbonDetailView Template with an Wizard Control on it.")]
    public sealed class WizardUIWindowsFormsModule : ModuleBase
    {
        #region Methods
        /// <summary>
        /// Initializes the Module
        /// </summary>
        /// <param name="application">XafApplication Object</param>
        public override void Setup(XafApplication application)
        {
            base.Setup(application);

            application.CreateCustomTemplate += Application_CreateCustomTemplate;
        }

        public override void ExtendModelInterfaces(DevExpress.ExpressApp.Model.ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelDetailView, IModelDetailViewWizard>();
        }

        /// <summary>
        /// Creates a custom Template
        /// </summary>
        /// <param name="sender">XafApplication Object</param>
        /// <param name="e">CreateCustomTemplate EventArgs</param>
        private void Application_CreateCustomTemplate(object sender, CreateCustomTemplateEventArgs e)
        {
            if (e.Context == "WizardDetailViewForm")
            {
                e.Template = new eXpand.ExpressApp.WizardUI.Win.Templates.WizardDetailViewForm();
            }
        }

        #endregion
    }
}