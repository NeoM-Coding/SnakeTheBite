// 耶梦加得 - 先古之民
using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using SnakeTheBite.Scripts.Relics;

namespace SnakeTheBite.Scripts.Ancients;

public class JormungandrAncient : CustomAncientModel
{
    public override Color ButtonColor => new Color(0.05f, 0.25f, 0.1f, 0.6f);

    public override Color DialogueColor => new Color("1A3D2E");

    public override string? CustomScenePath => "res://SnakeTheBite/images/ancients/jormungandr_ancient_bg.tscn";

    public override string? CustomMapIconPath => "res://SnakeTheBite/images/ancients/jormungandr_ancient.png";

    public override string? CustomMapIconOutlinePath => "res://SnakeTheBite/images/ancients/jormungandr_ancient_outline.png";

    public override string? CustomRunHistoryIconOutlinePath => "res://SnakeTheBite/images/ancients/jormungandr_ancient_outline.png";

    public override string? CustomRunHistoryIconPath => "res://SnakeTheBite/images/ancients/jormungandr_ancient.png";

    protected override OptionPools MakeOptionPools => new(
        MakePool(
            AncientOption<OuroborosRelic>(),
            AncientOption<VenomSacRelic>(),
            AncientOption<LastTwilightRelic>()
        ),
        MakePool(
            AncientOption<SnakeSkinRelic>(),
            AncientOption<WitheredWoodRelic>(),
            AncientOption<SnakeHeartRelic>(),
            AncientOption<SnakeScalesRelic>()
        ),
        MakePool(
            AncientOption<VenomFangRelic>(),
            AncientOption<SnakeIdolRelic>(),
            AncientOption<FrozenBloodRelic>()
        )
    );

    public override bool IsValidForAct(ActModel act) => false;
}
