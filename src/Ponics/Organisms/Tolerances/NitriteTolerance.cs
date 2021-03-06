﻿using ServiceStack;

namespace Ponics.Organisms.Tolerances
{
    public class NitriteTolerance:Tolerance
    {
        public NitriteTolerance(double lower, double upper, double desiredLower, double desiredUpper) : base(lower, upper, desiredLower, desiredUpper)
        {
        }

        [ApiMember(ExcludeInSchema = true)]
        public override Scale Scale => Scale.Ppm;
    }
}
