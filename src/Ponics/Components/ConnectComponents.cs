﻿using System;
using Ponics.Commands;
using Ponics.Kernel.Data;
using ServiceStack;

namespace Ponics.Components
{
    [Api("Connects two a components")]
    [Route("/systems/{SystemsId}/components/connections", "POST")]
    public class ConnectComponents : Command, IDataCommand
    {
        [ApiMember(Name = "SystemId", Description = "The id of a system",
            ParameterType = "path", DataType = "string", IsRequired = true)]
        [ApiAllowableValues("SystemId", typeof(Guid))]
        public Guid SystemId { get; set; }

        public ComponentConnection ComponentConnection { get; set; }
    }
}