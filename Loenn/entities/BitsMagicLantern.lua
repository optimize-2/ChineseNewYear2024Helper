local BITSMagicLantern =
{
    name = "ChineseNewYear2024Helper/BITSMagicLantern",
    placements =
    {
        {
            name = "ChineseNewYear2024Helper/BITSMagicLantern",
            data = {
                radius = 20.0,
                holdable = false
            },
        },
    },
};

function BITSMagicLantern.texture(room, entity)
    local holdable = entity.holdable or false;

    return holdable and "objects/ChineseNewYear2024Helper/BitsMagicLanternLight/holdable0" or "objects/ChineseNewYear2024Helper/BitsMagicLanternLight/static0";
end

return BITSMagicLantern;