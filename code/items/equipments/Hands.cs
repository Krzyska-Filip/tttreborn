using Sandbox;
using Sandbox.Joints;

using TTTReborn.Player;

// Credits to the PhysGun: https://github.com/Facepunch/sandbox/blob/master/code/tools/PhysGun.cs

namespace TTTReborn.Items
{
    [Library("ttt_hands")]
    partial class Hands : TTTEquipment
    {
        public override bool CanDrop() => false;

        public ModelEntity GrabbedEntity { get; set; }

        private static int GRAB_DISTANCE => 80;
        private Rotation eyeRot;
        private Vector3 eyePos;
        private Vector3 eyeDir;

        public override void Simulate(Client client)
        {
            if (!IsServer)
            {
                return;
            }

            if (Owner is not TTTPlayer player)
            {
                return;
            }

            eyeRot = Rotation.From(new Angles(0.0f, player.EyeRot.Angles().yaw, 0.0f));
            eyePos = player.EyePos;
            eyeDir = player.EyeRot.Forward;

            using (Prediction.Off())
            {
                if (Input.Down(InputButton.Attack1) && !GrabbedEntity.IsValid())
                {
                    TryStartGrab(player);
                } else if (Input.Down(InputButton.Attack2) && GrabbedEntity.IsValid())
                {
                    GrabbedEntity.EnableAllCollisions = true;
                    GrabbedEntity = null;
                } else if (GrabbedEntity.IsValid())
                {
                    MoveEntity(player);
                }
            }
        }

        private void MoveEntity(TTTPlayer player)
        {
            var attachment = player.GetAttachment("middle_of_both_hands")!.Value;
            GrabbedEntity.Position = attachment.Position.
        }

        private void TryStartGrab(TTTPlayer player)
        {
            TraceResult tr = Trace.Ray(eyePos, eyePos + eyeDir * GRAB_DISTANCE)
                .UseHitboxes()
                .Ignore(player)
                .HitLayer(CollisionLayer.Debris)
                .EntitiesOnly()
                .Run();

            DebugOverlay.Line(eyePos, eyePos + eyeDir * GRAB_DISTANCE, Color.Yellow);

            if (!tr.Hit || !tr.Entity.IsValid() || tr.Entity is not ModelEntity entity)
            {
                return;
            }

            GrabbedEntity = entity;
            GrabbedEntity.EnableAllCollisions = false;
        }

        public override void SimulateAnimator(PawnAnimator anim)
        {
            if (!IsServer)
            {
                return;
            }

            // TODO: Convert to event?
            if (GrabbedEntity.IsValid())
            {
                anim.SetParam("holdtype", 4);
                anim.SetParam("holdtype_handedness", 0);
                anim.SetParam("holdtype_pose", 0);
            }
            else
            {
                anim.SetParam("holdtype", 0);
            }
        }
    }
}
