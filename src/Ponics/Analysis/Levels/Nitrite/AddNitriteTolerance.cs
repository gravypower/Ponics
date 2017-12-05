﻿using ServiceStack;

namespace Ponics.Analysis.Levels.Nitrite
{
    [Api("Add a Nitrite tolerance to an organism")]
    [Route("/organisms/{OrganismId}/tolerances/nitrite", "POST")]
    public class AddNitriteTolerance: AddTolerance<NitriteTolerance>
    {
        public override NitriteTolerance Tolerance { get; set; }
    }
}
