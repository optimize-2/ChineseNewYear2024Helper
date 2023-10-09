using Celeste.Mod.ChronoHelper.Entities;
using Monocle;
using Celeste.Mod.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.ChineseNewYear2024Helper.RD.Entities.Entities;

[CustomEntity("ChineseNewYear2024Helper/RDSpaceController")]
public class RDSpaceController : Entity
{
    public Level level;
    private bool warpPlayer;
    public RDSpaceController(Vector2 position, bool warpPlayer)
    {
        Position = position;
        base.Collider = new Hitbox(16f, 16f, -8f, -8f);

        this.warpPlayer = warpPlayer;
    }

    public RDSpaceController(EntityData data, Vector2 offset) : this(data.Position + offset, data.Bool("warpPlayer"))
    {

    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();
    }

    public override void Update()
    {
        base.Update();
        GravityFallingBlockUpdate();
        if (warpPlayer)
        {
            PLayerUpdate();
        }
    }

    private void PLayerUpdate()
    {
        Player player = Scene.Tracker.GetEntity<Player>();
        if (player == null)
            return;

        if (player.Top > level.Bounds.Bottom)
        {
            player.Bottom = level.Bounds.Top;
        }
        else if (player.Bottom < level.Bounds.Top)
        {
            player.Top = level.Bounds.Bottom;
        }

    }

    private void GravityFallingBlockUpdate()
    {
        var entities = Scene.Tracker.GetEntities<GravityFallingBlock>();
        if (entities == null)
            return;

        foreach (var item in entities)
        {
            if (item.Top > level.Bounds.Bottom)
            {
                item.Bottom = level.Bounds.Top;
            }
            else if (item.Bottom < level.Bounds.Top)
            {
                item.Top = level.Bounds.Bottom;
            }
        }
    }
}
