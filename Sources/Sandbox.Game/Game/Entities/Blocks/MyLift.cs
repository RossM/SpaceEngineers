using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Engine.Physics;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.GameSystems;
using VRageMath;

namespace Sandbox.Game.Entities.Blocks
{
    [MyCubeBlockType(typeof(MyObjectBuilder_Lift))]
    class MyLift : MyCubeBlock, IMyLevitater
    {
        private bool m_working;
        private bool m_emissivitySet;

        public override void Init(MyObjectBuilder_CubeBlock objectBuilder, MyCubeGrid cubeGrid)
        {
            base.Init(objectBuilder, cubeGrid);
            this.NeedsUpdate = MyEntityUpdateEnum.EACH_100TH_FRAME;
            m_emissivitySet = false;
        }

        public override void UpdateBeforeSimulation100()
        {
            var gravity = MyGravityProviderSystem.CalculateGravityInPoint(LocationForHudMarker);
            var working = gravity.LengthSquared() >= 0.25f;
            if (m_working != working || !m_emissivitySet)
            {
                m_working = working;
                UpdateEmissivity();
            }
        }

        public override void OnRemovedFromScene(object source)
        {
            base.OnRemovedFromScene(source);

            m_emissivitySet = false;
        }

        public override void OnAddedToScene(object source)
        {
            base.OnAddedToScene(source);

            UpdateEmissivity();
        }

        public override void OnModelChange()
        {
            UpdateEmissivity();
        }

        public override void UpdateVisual()
        {
            base.UpdateVisual();

            UpdateEmissivity();
        }

        private void UpdateEmissivity()
        {
            var newColor = m_working ? Color.LightBlue : Color.DarkRed;

            MyCubeBlock.UpdateEmissiveParts(Render.RenderObjectIDs[0], 1.0f, newColor, Color.White);
            m_emissivitySet = true;
        }

        public override bool GetIntersectionWithSphere(ref BoundingSphereD sphere)
        {
            return false;
        }

        internal override bool GetIntersectionWithLine(ref LineD line, out MyIntersectionResultLineTriangleEx? t, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES)
        {
            t = null;
            return false;
        }

        bool IMyLevitater.IsWorking()
        {
            return m_working;
        }
    }
}
