﻿using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.ModelDifference.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.ExpressApp.ModelDifference.Security;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ModelDifference.DictionaryStores {
    public class XpoUserModelDictionaryDifferenceStore : XpoDictionaryDifferenceStore {

        public XpoUserModelDictionaryDifferenceStore(XafApplication application)
            : base(application) {
        }

        public override DifferenceType DifferenceType {
            get { return DifferenceType.User; }
        }

        protected internal override ModelDifferenceObject GetActiveDifferenceObject(string name) {
            return new QueryUserModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifference(Application.GetType().FullName, Application);
        }

        protected internal IQueryable<ModelDifferenceObject> GetActiveDifferenceObjects() {
            return new QueryUserModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifferences(Application.GetType().FullName, null).Cast<ModelDifferenceObject>();
        }

        protected internal IQueryable<ModelDifferenceObject> GetActiveRoleDifferenceObjects() {
            return new QueryRoleModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifferences(Application.GetType().FullName, null).Cast<ModelDifferenceObject>();
        }

        public override void Load(ModelApplicationBase model) {
            base.Load(model);

            foreach (var roleModel in this.GetActiveRoleDifferenceObjects())
                roleModel.GetModel(base.Application.Model as ModelApplicationBase);

            var modelDifferenceObjects = GetActiveDifferenceObjects().ToList();
            if (modelDifferenceObjects.Count() == 0) {
                SaveDifference(model);
                return;
            }

            CombineWithActiveDifferenceObjects(model, modelDifferenceObjects);
        }

        void CombineWithActiveDifferenceObjects(ModelApplicationBase model, IEnumerable<ModelDifferenceObject> modelDifferenceObjects) {
            var reader = new ModelXmlReader();
            foreach (var modelDifferenceObject in modelDifferenceObjects) {
                foreach (var aspectObject in modelDifferenceObject.AspectObjects) {
                    var xml = aspectObject.Xml;
                    if (!string.IsNullOrEmpty(xml))
                        reader.ReadFromString(model, modelDifferenceObject.GetAspectName(aspectObject), xml);
                }
            }
        }

        protected internal override ModelDifferenceObject GetNewDifferenceObject(IObjectSpace session) {
            var aspectObject = session.CreateObject<UserModelDifferenceObject>();
            aspectObject.AssignToCurrentUser();
            return aspectObject;
        }

        protected internal override void OnDifferenceObjectSaving(ModelDifferenceObject userModelDifferenceObject, ModelApplicationBase model) {
            var userStoreObject = ((UserModelDifferenceObject)userModelDifferenceObject);
            if (!userStoreObject.NonPersistent) {
                userModelDifferenceObject.CreateAspectsCore(model);
                base.OnDifferenceObjectSaving(userModelDifferenceObject, model);
            }
            CombineModelFromPermission(model);
        }

        public override void SaveDifference(ModelApplicationBase model) {
            if (SecuritySystem.CurrentUser != null)
                base.SaveDifference(model);
        }

        void CombineModelFromPermission(ModelApplicationBase model) {
            if (SecuritySystem.Instance is ISecurityComplex && SecuritySystemExtensions.IsGranted(new ModelCombinePermission(ApplicationModelCombineModifier.Allow), false)) {
                var space = Application.CreateObjectSpace();
                ModelDifferenceObject difference = GetDifferenceFromPermission((ObjectSpace)space);
                var master = new ModelLoader(difference.PersistentApplication.ExecutableName).GetMasterModel(true);
                var diffsModel = difference.GetModel(master);
                new ModelXmlReader().ReadFromModel(diffsModel, model);
                difference.CreateAspectsCore(diffsModel);
                space.SetModified(difference);
                space.CommitChanges();
            }
        }

        private ModelDifferenceObject GetDifferenceFromPermission(ObjectSpace space) {
            return new QueryModelDifferenceObject(space.Session).GetModelDifferences(
                ((IUser)SecuritySystem.CurrentUser).Permissions.OfType<ModelCombinePermission>().Select(
                    permission => permission.Difference)).Single();
        }

    }
}