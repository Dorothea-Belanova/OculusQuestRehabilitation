using System.Collections.Generic;
using UnityEngine;

namespace ErrorCorrection
{

    public abstract class Filter
    {

        public abstract Vector3 Filtrate(List<Vector3> positions);

    }
}