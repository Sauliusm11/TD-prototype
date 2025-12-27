using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tower.Towers
{
    public class StraightRangeUpgradeHandler : UpgradeHandler
    {
        public override void UpdateRangeAndIndicator()
        {
            shootingHandler.SetRange(tileRangeMult * range);
            rangeIndicator.transform.localScale = new Vector3(1, tileRangeMult * range, 0);
            rangeIndicator.transform.localPosition = new Vector3(0, (tileRangeMult * range - 1) / 2, 0);
        }
    }
}
