﻿using Xpand.ExpressApp.ArtifactState.Logic;

namespace Xpand.ExpressApp.ConditionalActionState.Logic {
    public class ActionStateRuleAttribute : ArtifactStateRuleAttribute, IActionStateRule {
        public ActionStateRuleAttribute(string id, string actionId, string normalCriteria, string emptyCriteria, ActionState actionState) : base(id, normalCriteria, emptyCriteria) {
            ActionState = actionState;
            ActionId = actionId;
        }

        public string ActionId { get; set; }
        public ActionState ActionState { get; set; }
    }
}