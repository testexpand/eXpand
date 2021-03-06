using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Logic;

namespace FeatureCenter.Module.Web {
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class FeatureCenterAspNetModule : FeatureCenterModuleBase {
        public FeatureCenterAspNetModule() {
            InitializeComponent();
        }


        protected override void AdditionalViewControlsModuleOnRulesCollected(object sender, EventArgs e) {
            foreach (var typeInfo in XafTypesInfo.Instance.PersistentTypes) {
                var additionalViewControlsRules = LogicRuleManager<IAdditionalViewControlsRule>.Instance[typeInfo];
                foreach (var additionalViewControlsRule in additionalViewControlsRules) {
                    if (additionalViewControlsRule.Id.StartsWith(Module.Captions.Header)) {
                        additionalViewControlsRule.FontStyle = FontStyle.Bold;
                        additionalViewControlsRule.FontSize = 18;
                    } else if (additionalViewControlsRule.Id.StartsWith(Module.Captions.ViewMessage))
                        additionalViewControlsRule.FontStyle = FontStyle.Italic;
                }
            }
        }

    }

}
