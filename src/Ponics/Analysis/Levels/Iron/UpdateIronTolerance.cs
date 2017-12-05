﻿using ServiceStack;

namespace Ponics.Analysis.Levels.Iron
{
    [Api("Updates the Iron tolerance of an organism")]
    [Route("/organisms/{OrganismId}/tolerances/iron", "PUT")]
    public class UpdateIronTolerance: UpdateTolerance<IronTolerance>
    {
        public override IronTolerance Tolerance { get; set; }
    }
}
