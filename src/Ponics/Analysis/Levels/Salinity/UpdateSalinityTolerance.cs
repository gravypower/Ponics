﻿using ServiceStack;

namespace Ponics.Analysis.Levels.Salinity
{
    [Api("Updates the salinity tolerance of an organism")]
    [Route("/organisms/{OrganismId}/tolerances/salinity", "PUT")]
    public class UpdateSalinityTolerance : UpdateTolerance<SalinityTolerance>
    {
        public override SalinityTolerance Tolerance { get; set; }
    }
}
