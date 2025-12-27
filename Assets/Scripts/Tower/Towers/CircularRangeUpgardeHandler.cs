using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Assets.Scripts.Tower.Towers
{
    public class CircularRangeUpgardeHandler : UpgradeHandler
    {
        public override void UpdateRangeAndIndicator()
        {
            shootingHandler.SetRange(tileRangeMult * range);
            rangeIndicator.transform.localScale = new Vector3(tileRangeMult * range * 2, tileRangeMult * range * 2, 0);
        }
    }
}
