using System.IO;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win.SystemModule;

namespace Xpand.ExpressApp.ModelDifference.Win {
    public sealed class ModelDifferenceWindowsFormsModule : ModelDifferenceBaseModule {
        public ModelDifferenceWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(ModelDifferenceModule));
            RequiredModuleTypes.Add(typeof(SystemWindowsFormsModule));
        }
        public static ModelApplicationCreator ApplicationCreator { get; set; }
        private bool? _modelsLoaded = false;

        public override string GetPath() {
            return Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
        }

        public override bool? ModelsLoaded {
            get { return _modelsLoaded; }
            set { _modelsLoaded = value; }
        }
    }
}