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

            eyePos = player.EyePos;
            eyeDir = player.EyeRot.Forward;

            using (Prediction.Off())
            {
                if (GrabbedEntity.IsValid())
                {
                    if (Input.Pressed(InputButton.Attack2))
                    {
                        DropEntity();
                    }
                    else
                    {
                        MoveEntity(player);
                    }
                }
                else
                {
                    if (Input.Pressed((InputButton.Attack1)))
                    {
                        GrabEntity(player);
                    }
                }
            }
        }

        private void MoveEntity(TTTPlayer player)
        {
            var attachment = player.GetAttachment("middle_of_both_hands")!.Value;
            GrabbedEntity.Position = attachment.Position;
            GrabbedEntity.Rotation = attachment.Rotation.RotateAroundAxis(Vector3.Backward, 90);
        }

        private void GrabEntity(TTTPlayer player)
        {
            TraceResult tr = Trace.Ray(eyePos, eyePos + eyeDir * GRAB_DISTANCE)
                .UseHitboxes()
                .Ignore(player)
                .HitLayer(CollisionLayer.Debris)
                .EntitiesOnly()
                .Run();

            if (!tr.Hit || !tr.Entity.IsValid() || tr.Entity is not ModelEntity entity)
            {
                return;
            }

            GrabbedEntity = entity;
            GrabbedEntity.Parent = player;
            GrabbedEntity.EnableAllCollisions = false;
        }

        private void DropEntity()
        {
            GrabbedEntity.EnableAllCollisions = true;
            GrabbedEntity.Parent = null;
            GrabbedEntity = null;
        }

        public override void SimulateAnimator(PawnAnimator anim)
        {
            if (!IsServer)
            {
                return;
            }

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
